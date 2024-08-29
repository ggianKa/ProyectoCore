using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Persistencia;
using Dominio;

namespace WebAPI.Controllers
{
    // http://localhost:5000/WeatherForecast

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //Inyección de dependencias
        private readonly CursosOnlineContext context;
        public WeatherForecastController(CursosOnlineContext _context){ 
            this.context = _context;
        }

        [HttpGet]
        public IEnumerable<Curso> Get(){
            return context.Curso.ToList();
        }
    }
}
