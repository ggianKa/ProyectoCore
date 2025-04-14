// Importamos los paquetes necesarios
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    // Esta clase contiene la lógica para actualizar la información de un usuario
    public class UsuarioActualizar
    {
        // Esta clase representa los datos que se enviarán para actualizar el usuario
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string NombreCompleto { get; set; }     // Nuevo nombre completo del usuario
            public string Email { get; set; }              // Nuevo email
            public string Password { get; set; }           // Nueva contraseña
            public string Username { get; set; }           // Nombre de usuario (clave para encontrar al usuario)
            public ImagenGeneral ImagenPerfil { get; set; } // Imagen de perfil (no usada aquí, pero puede usarse después)
        }

        // Esta clase valida que todos los campos necesarios estén completos
        public class EjecutaValidator : AbstractValidator<Ejecuta>
        {
            public EjecutaValidator()
            {
                RuleFor(x => x.NombreCompleto).NotEmpty(); // El nombre no debe estar vacío
                RuleFor(x => x.Email).NotEmpty();          // El email no debe estar vacío
                RuleFor(x => x.Password).NotEmpty();       // La contraseña no debe estar vacía
                RuleFor(x => x.Username).NotEmpty();       // El nombre de usuario no debe estar vacío
            }
        }

        // Esta clase se encarga de procesar la petición (Handler en MediatR)
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            // Inyectamos las dependencias necesarias
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IPasswordHasher<Usuario> _passwordHasher;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IPasswordHasher<Usuario> passwordHasher)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _passwordHasher = passwordHasher;
            }

            // Este método contiene la lógica para actualizar un usuario
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Buscamos al usuario por su nombre de usuario
                var usuarioIden = await _userManager.FindByNameAsync(request.Username);
                if (usuarioIden == null)
                {
                    // Si no existe, lanzamos una excepción personalizada
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No existe un usuario con este username" });
                }

                // Verificamos que el nuevo email no esté siendo usado por otro usuario
                var resultado = await _context.Users
                    .Where(x => x.Email == request.Email && x.UserName != request.Username)
                    .AnyAsync();

                if (resultado)
                {
                    // Si otro usuario ya tiene ese email, lanzamos error
                    throw new ManejadorExcepcion(HttpStatusCode.InternalServerError, new { mensajes = "Este email pertenece a otro usuario" });
                }

                // Buscamos la imagen de perfil del usuario
                if (request.ImagenPerfil != null){
                    var resultadoImagen = await _context.Documento
                    .Where(x => x.ObjetoReferencia == new Guid(usuarioIden.Id))
                    .FirstAsync();
                    if(resultadoImagen == null){
                        var imagen = new Documento {
                            Contenido = System.Convert.FromBase64String(request.ImagenPerfil.Data),
                            Nombre = request.ImagenPerfil.Nombre,
                            Extension = request.ImagenPerfil.Extension,
                            ObjetoReferencia = new Guid(usuarioIden.Id),
                            DocumentoId = Guid.NewGuid(),
                            FechaCreacion = DateTime.UtcNow
                        };
                        _context.Documento.Add(imagen);
                    }else {
                        resultadoImagen.Contenido = System.Convert.FromBase64String(request.ImagenPerfil.Data);
                        resultadoImagen.Nombre = request.ImagenPerfil.Nombre;
                        resultadoImagen.Extension = request.ImagenPerfil.Extension;
                    }
                }
                


                // Actualizamos los datos del usuario
                usuarioIden.NombreCompleto = request.NombreCompleto;
                usuarioIden.PasswordHash = _passwordHasher.HashPassword(usuarioIden, request.Password); // Hasheamos la nueva contraseña
                usuarioIden.Email = request.Email;

                // Guardamos los cambios en el sistema de identidad
                var resultadoUpdate = await _userManager.UpdateAsync(usuarioIden);

                // Obtenemos los roles del usuario para generar el nuevo token
                var resultadoRoles = await _userManager.GetRolesAsync(usuarioIden);
                var listRoles = new List<string>(resultadoRoles);

                // Buscamos la imagen de perfil del usuario en la base de datos, según su ID
                var imagenPerfil = await _context.Document
                    .Where(x => x.ObjetoReferencia == new Guid(usuarioIden.Id)) // ObjetoReferencia guarda el ID del usuario relacionado
                    .FirstAsync();

                // Inicializamos un objeto para la imagen, por si no existe aún
                ImagenGeneral imagenGeneral = null;

                // Si existe una imagen de perfil, la convertimos a formato base64 para enviarla como texto
                if(imagenPerfil != null){
                    imagenGeneral = new ImagenGeneral{
                        Data = Convert.ToBase64String(imagenPerfil.Contenido), // Convertimos el contenido binario a texto Base64
                        Nombre = imagenPerfil.Nombre,        // Nombre del archivo (ej. "perfil.jpg")
                        Extension = imagenPerfil.Extension   // Tipo de archivo (ej. ".jpg", ".png")
                    };
                }


                // Si la actualización del usuario fue exitosa
                if (resultadoUpdate.Succeeded)
                {
                    // Retornamos un nuevo objeto UsuarioData con la información actualizada
                    return new UsuarioData
                    {
                        NombreCompleto = usuarioIden.NombreCompleto, // Nombre del usuario
                        Username = usuarioIden.UserName,             // Nombre de usuario
                        Email = usuarioIden.Email,                   // Correo electrónico
                        Token = _jwtGenerador.CrearToken(usuarioIden, listRoles), // Generamos un nuevo token JWT
                        ImagenPerfil = imagenGeneral                 // Enviamos la imagen convertida (si existe)
                    };
                }


                // Si algo salió mal, lanzamos una excepción
                throw new System.Exception("No se pudo actualizar el usuario");
            }
        }
    }
}
