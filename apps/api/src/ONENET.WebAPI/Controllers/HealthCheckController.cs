// QUAN-20260601-1634
using Microsoft.AspNetCore.Mvc;
using ONENET.WebAPI.Common;
using System.Net;

namespace ONENET.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check endpoint accessed.");
            return Ok(ApiResponse.Success("API is healthy."));
        }

        [HttpGet("error")]
        public IActionResult TriggerError()
        {
            _logger.LogError("Test error endpoint hit. Throwing a dummy exception.");
            throw new Exception("This is a test exception from HealthCheckController.");
        }
    }
}