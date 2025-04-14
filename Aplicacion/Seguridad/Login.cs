// Importamos todas las librerías necesarias para la funcionalidad
using Aplicacion.Contratos; // Interfaces y contratos que hemos definido para la aplicación
using Aplicacion.ManejadorError; // Clase personalizada para manejar errores
using Dominio; // Modelos del dominio, como Usuario
using Persistencia; // Acceso a base de datos (DbContext)
using FluentValidation; // Para validar campos obligatorios
using MediatR; // Para manejar peticiones con el patrón Mediator
using Microsoft.AspNetCore.Identity; // Para usar Identity (usuarios y contraseñas)
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    // Clase que representa el caso de uso de "Login"
    public class Login
    {
        // Esta clase representa la solicitud de login (los datos que se envían desde el frontend)
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string Email { get; set; }      // Correo del usuario
            public string Password { get; set; }   // Contraseña del usuario
        }

        // Validaciones para asegurarnos de que no se envíen datos vacíos
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Email).NotEmpty();    // El email no puede estar vacío
                RuleFor(x => x.Password).NotEmpty(); // La contraseña no puede estar vacía
            }
        }

        // Clase que maneja la lógica cuando se realiza el login
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> _userManager;       // Gestiona usuarios
            private readonly SignInManager<Usuario> _signInManager;   // Gestiona inicios de sesión
            private readonly IJwtGenerador _jwtGenerador;             // Genera el token JWT
            private readonly CursosOnlineContext _context;            // Acceso a la base de datos

            // Constructor: recibe las dependencias necesarias
            public Manejador(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IJwtGenerador jwtGenerador, CursosOnlineContext context)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _jwtGenerador = jwtGenerador;
                _context = context;
            }

            // Método que maneja la solicitud de login
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Buscamos al usuario por su email
                var usuario = await _userManager.FindByEmailAsync(request.Email);
                
                if (usuario == null)
                {
                    // Si no existe el usuario, devolvemos error 401 (no autorizado)
                    throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
                }

                // Verificamos si la contraseña ingresada es correcta
                var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);

                // Obtenemos los roles que tiene este usuario (por ejemplo, admin o user)
                var resultadoRoles = await _userManager.GetRolesAsync(usuario);
                var listaRoles = new List<string>(resultadoRoles);

                // Buscamos si el usuario tiene una imagen de perfil asociada
                var imagenPerfil = await _context.Documento
                    .Where(x => x.ObjetoReferencia == new Guid(usuario.Id))
                    .FirstAsync();

                // Si la contraseña es correcta:
                if (resultado.Succeeded)
                {
                    // Si tiene imagen de perfil, la convertimos a base64 para poder enviarla como texto
                    if(imagenPerfil != null){
                        var imagenCliente = new ImagenGeneral {
                            Data = Convert.ToBase64String(imagenPerfil.Contenido), // Convertimos la imagen a texto
                            Extension = imagenPerfil.Extension, // Por ejemplo ".jpg"
                            Nombre = imagenPerfil.Nombre
                        };

                        // Retornamos todos los datos del usuario, incluyendo la imagen
                        return new UsuarioData{
                            NombreCompleto = usuario.NombreCompleto,
                            Token = _jwtGenerador.CrearToken(usuario, listaRoles), // Generamos el token JWT
                            Username = usuario.UserName,
                            Email = usuario.Email,
                            ImagenPerfil = imagenCliente
                        };
                    } 
                    else 
                    {
                        // Si no tiene imagen, igual retornamos los demás datos
                        return new UsuarioData {
                            NombreCompleto = usuario.NombreCompleto,
                            Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                            Username = usuario.UserName,
                            Email = usuario.Email,
                            Imagen = null
                        };
                    }            
                }

                // Si la contraseña es incorrecta, lanzamos error 401
                throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
            }
        }
    }
}
