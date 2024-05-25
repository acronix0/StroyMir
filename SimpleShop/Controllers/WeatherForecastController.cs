using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Model;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;

namespace SimpleShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private RepositoryManager _repositoryManager;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, RepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var x = new Category() { Description = "123", Name = "123" };
            _repositoryManager.CategoryRepository.AddCategory(x);
            var xx =  _repositoryManager.SaveAsync();
            // toddo long operation
            await xx;
             //var xx = _repositoryManager.OrderRepository.FindByCondition(o => (o as Order).TotalPrice > 0, false); 
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromForm] string login, [FromForm] string password)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            return Ok(login == "1" && password == "1" ? true : false);
        }
    }
}
