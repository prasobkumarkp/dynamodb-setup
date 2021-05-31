using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dynamodb_setup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] Cities = new[]
        {
            "Newark", "Trenton", "Edison", "Paterson", "Hoboken", "Wayne", "Clifton", "Morristown", "Tarrytown", "Ithaca"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDynamoDBContext _dynamoDBContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDynamoDBContext dynamoDBContext)
        {
            _logger = logger;
            _dynamoDBContext = dynamoDBContext;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(string city = "Clifton")
        {
            return await _dynamoDBContext
                .QueryAsync<WeatherForecast>(city)
                .GetRemainingAsync();

        }

        [HttpPost]
        public async Task Post(string city = "Clifton")
        {
            //var data = Data();
            //foreach (var item in data)
            //{
            //    await _dynamoDBContext.SaveAsync(item);
            //}

            var item = await _dynamoDBContext.LoadAsync<WeatherForecast>(city, DateTime.Now.Date.AddDays(1));
            item.Summary = "Test";
            await _dynamoDBContext.SaveAsync(item);
        }


        [HttpDelete]
        public async Task Delete(string city = "Clifton")
        {
            var item = await _dynamoDBContext.LoadAsync<WeatherForecast>(city, DateTime.Now.Date.AddDays(1));
            await _dynamoDBContext.DeleteAsync<WeatherForecast>(item);
        }

        public static IEnumerable<WeatherForecast> Data()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                City = Cities[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
