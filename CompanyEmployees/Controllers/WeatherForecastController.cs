using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ILoggerManager _logger;

        public WeatherForecastController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Info vagyok lol");
            _logger.LogDebug("Debug vagyok lol");
            _logger.LogWarn("Warn vagyok lol");
            _logger.LogError("Error vagyok lol");

            return new string[] { "anya", "banya" };
        }
    }
}
