using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Titulo { get; set; }
        }

        public class EjecutaValida : AbstractValidator<Ejecuta>
        {
            public EjecutaValida()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Titulo).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepository;
            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var resultado = await _instructorRepository.Nuevo(request.Nombre, request.Apellidos, request.Titulo);

                if (resultado > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("Nose pudo insertar el instructor");
            }

        }
    }

}