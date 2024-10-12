using FluentValidation;
using MediatR;
using Persistencia;
using System;
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
            public int CursoId { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }

            // El signo de interrogacion es para que acepte valores nulos
            public DateTime? FechaPublicacion { get; set; }
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
                    throw new Exception("El curso no existe");
                }

                // Actualiza las propiedades del curso solo si se han proporcionado nuevos valores
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;

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
