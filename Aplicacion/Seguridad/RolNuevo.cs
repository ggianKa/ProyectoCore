using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace Aplicacion.Seguridad
{
    /// Clase que encapsula la lógica para la creación de un nuevo rol en el sistema.
    public class RolNuevo
    {
        /// Comando para la creación de un nuevo rol.
        /// Implementa IRequest de MediatR para su manejo en el patrón CQRS.
        public class Ejecuta : IRequest {
            /// Nombre del nuevo rol que se desea crear.
            public string Nombre { get; set; }
        }

        /// Validador para el comando Ejecuta.
        /// Utiliza FluentValidation para aplicar reglas de validación.
        public class ValidaEjecuta : AbstractValidator<Ejecuta>{
            /// Constructor que define las reglas de validación.
            public ValidaEjecuta(){
                RuleFor(x => x.Nombre)
                    .NotEmpty() // Verifica que el nombre no esté vacío.
                    .WithMessage("El nombre del rol es obligatorio");
            }
        }

        /// Clase manejadora que implementa la lógica para crear un nuevo rol.
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly RoleManager<IdentityRole> _roleManager;

            /// Constructor que inyecta la dependencia RoleManager para gestionar roles.
            public Manejador(RoleManager<IdentityRole> roleManager){
                _roleManager = roleManager;
            }

            /// Maneja la creación del rol, validando si ya existe y registrándolo si no.
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Verifica si el rol ya existe en el sistema.
                var role = await _roleManager.FindByNameAsync(request.Nombre);
                if(role != null){
                    // Lanza una excepción personalizada si el rol ya existe.
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new { mensaje = "Ya existe el rol" });
                }
                
                // Crea un nuevo rol en el sistema.
                var resultado = await _roleManager.CreateAsync(new IdentityRole(request.Nombre));
                if(resultado.Succeeded){
                    return Unit.Value; // Indica que la operación fue exitosa.
                }
                
                // Lanza una excepción si la creación del rol falla.
                throw new Exception("No se puede guardar el rol");
            }
        }
    }
}
