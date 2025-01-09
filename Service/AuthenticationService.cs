using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtConfiguration> _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtConfiguration _jwtConfiguration;

        private User? _user;
        
        //Ioptionsmonitor for live reloading
        public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IOptions<JwtConfiguration> configuration, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _jwtConfiguration = _configuration.Value;
        }

        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signInCred = GetSignInCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signInCred, claims);

            var refreshToken = GenerateRefreshToken();

            _user.RefreshToken = refreshToken;

            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(accessToken, refreshToken);
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user is null 
                || user.RefreshToken != tokenDto.RefreshToken
                || user.RefreshTokenExpiryTime <= DateTime.Now) 
            {
                throw new RefreshTokenBadRequestException();
            }

            _user = user;

            return await CreateToken(populateExp: false);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signInCred, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signInCred
            );

            return tokenOptions;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSignInCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("LONGSECRET"));
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var result = await _userManager.Users.ToListAsync();
            return result;
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (result.Succeeded)
                await AddToRolesIfExists(user, userForRegistration.Roles);
                
            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuthentication)
        {
            _user = await _userManager.FindByNameAsync(userForAuthentication.UserName);

            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuthentication.Password));

            if (!result)
                _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");

            return result;
        }

        private async Task AddToRolesIfExists(User user, ICollection<string> roles)
        {
            if (roles == null || !roles.Any())
                return;

            foreach (var role in roles)
            {
                var exists = await _roleManager.RoleExistsAsync(role);
                if (!exists)
                    throw new RoleNotExistsException(role);
                
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        private string GenerateRefreshToken()
        {
            var rndNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(rndNumber);
                return Convert.ToBase64String(rndNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("LONGSECRET"))),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
