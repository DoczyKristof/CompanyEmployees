using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if (result.Succeeded)
                await AddToRolesIfExists(user, userForRegistration.Roles);
                
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
    }
}
