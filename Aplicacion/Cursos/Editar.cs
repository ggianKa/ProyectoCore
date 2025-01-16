using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Cursos
{
    // La clase editar tiene la logica para modificar un curso existente en el sistemas
    public class Editar
    {
        // Representa la solicitud para editar un curso, contiene las propiedades para actualizar un curso
        public class Ejecuta : IRequest
        {
            public Guid CursoId { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }

            // El signo de interrogacion es para que acepte valores nulos
            public DateTime? FechaPublicacion { get; set; }
            public List<Guid> ListaInstructor {get; set;}
            public decimal? Precio {get; set;}
            public decimal? Promocion {get; set;}
        }

        // Regla para validar los datos
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }


        // La clase Manejador es responsable de manejar la solicitud para editar un curso.
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            // Utiliza el contexto de la base de datos CursosOnlineContext para actualizar la información del curso.
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Busca el curso en la base de datos según el ID proporcionado
                var curso = await _context.Curso.FindAsync(request.CursoId);

                // Verifica si el curso existe
                if (curso == null)
                {
                    // Aqui va devolver un status de error y tambien va enviar un mensaje 
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el curso" });
                }


                // Actualiza las propiedades del curso solo si se han proporcionado nuevos valores
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion = DateTime.UtcNow;

                /*Actualizar el precio del curso*/
                var precioEntidad = _context.Precio.Where(x => x.CursoId == curso.CursoId).FirstOrDefault();
                if(precioEntidad!=null){
                    precioEntidad.Promocion = request.Promocion ?? precioEntidad.Promocion;
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;
                }else{
                    precioEntidad = new Precio{
                        PrecioId = Guid.NewGuid(),
                        PrecioActual = request.Precio ?? 0,
                        Promocion = request.Promocion ?? 0,
                        CursoId = curso.CursoId
                    };
                    await _context.Precio.AddAsync(precioEntidad);
                }


                if(request.ListaInstructor != null){
                    if(request.ListaInstructor.Count>0){
                        /*Eliminar los instructores actuales del curso en la base de datos*/
                        var instructoresBD = _context.CursoInstructor.Where( x => x.CursoId == request.CursoId);
                        foreach(var instructorEliminar in instructoresBD){
                            _context.CursoInstructor.Remove(instructorEliminar);
                        }
                        /*Fin del procedimiento para eliminar instructores*/

                        /*Procedimiento para agregar instructores que provienen del cliente*/
                        foreach(var id in request.ListaInstructor){
                            var nuevoInstructor = new CursoInstructor {
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            _context.CursoInstructor.Add(nuevoInstructor);
                        }
                        /*Fin del procedimiento*/
                    }
                }

                // Guarda los cambios en la base de datos
                var resultado = await _context.SaveChangesAsync();

                // Verifica si los cambios fueron guardados exitosamente
                if (resultado > 0)
                    return Unit.Value;

                // Lanza una excepción si los cambios no pudieron guardarse
                throw new Exception("No se guardaron los cambios en el curso");
            }
        }
    }
}
