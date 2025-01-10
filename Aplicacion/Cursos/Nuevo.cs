using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest {
            public string Titulo {get;set;}
            public string Descripcion {get;set;}
            public DateTime FechaPublicacion {get;set;}
            public byte[] FotoPortada{get;set;}
            public List<Guid> ListaInstructor {get;set;}
            public decimal Precio {get;set;}
            public decimal Promocion {get;set;}
        }



        public class EjecutaValidacion : AbstractValidator<Ejecuta>{
            // Regla de validacion para el titulo
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }


        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Codigo para ingresar un nuevo curso
                Guid _cursoId = Guid.NewGuid();
                var curso = new Curso
                {
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion
                };

                _context.Curso.Add(curso);

                //Codigo para agregar lista de instructores
                if(request.ListaInstructor!=null){
                    foreach(var id in request.ListaInstructor){
                        var cursoInstructor = new CursoInstructor{
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        _context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                /*Agregar logica para insertar precio de curso*/
                var precioEntidad = new Precio {
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid()
                };
                
                _context.Precio.Add(precioEntidad);

                var valor = await _context.SaveChangesAsync();
                if (valor > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}