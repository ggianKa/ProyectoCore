using Aplicacion.Cursos;
using Dominio;
using iTextSharp.text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    // Controlador de API que maneja todas las operaciones relacionadas con los cursos.
    // La ruta base de este controlador es: http://localhost:5000/api/Cursos
    // Se utiliza la anotación [ApiController] para mejorar la validación y el manejo de solicitudes HTTP.
    [Route("api/[controller]")]
    [ApiController]
   
    public class CursosController : MiControllerBase
    {

        // Obtiene una lista de objetos cursos
        [HttpGet]
        public async Task<ActionResult<List<Curso>>> Get()
        {
            return await Mediator.Send(new Consulta.ListaCursos());
        }

        // Obtiene el detalle de un curso específico, dado su Id
        // http://localhost:5000/api/Cursos/{id}
        // http://localhost:5000/api/Cursos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Curso>> Detalle(int id)
        {
            // Envía una solicitud para obtener el curso con el ID proporcionado
            return await Mediator.Send(new ConsultaId.CursoUnico { Id = id });
        }

        // Crea un nuevo curso en la base de datos, los datos estan encapsulados en el objeto Nuevo.Ejecuta
        // retorna una respuesta de tipo Unit indicando el exito o fallo de operación
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            // Envia una solicitud para crear un nuevo curso utilizando los datos proporcionados
            return await Mediator.Send(data);
        }

        // Endpoint PUT para editar un curso existente.
        // El ID del curso se pasa como parámetro en la URL.
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(int id, Editar.Ejecuta data)
        {
            data.CursoId = id;
            return await Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(int id)
        {
            return await Mediator.Send(new Eliminar.Ejecuta { Id = id });
        }
    }
}
