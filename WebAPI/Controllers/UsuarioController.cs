
using Aplicacion.Seguridad;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers

{
    public class UsuarioController : MiControllerBase
    {
        // http://localhost:5000/api/Usuario/login
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioData>> Login(Login.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);

        }
    }
}