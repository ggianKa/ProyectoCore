using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Controllers
{
    // http://localhost:5000/WeatherForecast

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get(){
            string[] nombres = new[]{"Fabian", "Rolando", "Maria", "Rebeca"};
            return nombres;
        }
    }
}
