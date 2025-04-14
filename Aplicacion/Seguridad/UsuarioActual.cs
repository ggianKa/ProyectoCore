// Importamos los paquetes necesarios
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    // Clase principal para obtener los datos del usuario que está actualmente logueado
    public class UsuarioActual
    {
        // Clase que representa la petición (no necesita parámetros porque buscamos al usuario actual)
        public class Ejecutar : IRequest<UsuarioData> {}

        // Clase que maneja la lógica cuando se ejecuta la petición
        public class Manejador : IRequestHandler<Ejecutar, UsuarioData>
        {
            // Inyectamos las dependencias necesarias
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IUsuarioSesion _usuarioSesion;
            private readonly CursosOnlineContext _context;

            // Constructor: se usa para inicializar las dependencias
            public Manejador(UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IUsuarioSesion usuarioSesion, CursosOnlineContext context){
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _usuarioSesion = usuarioSesion;
                _context = context;
            }

            // Este método se ejecuta cuando alguien quiere obtener la información del usuario actual
            public async Task<UsuarioData> Handle(Ejecutar request, CancellationToken cancellationToken)
            {
                // Obtenemos el nombre de usuario que está actualmente logueado (desde el token)
                var usuario = await _userManager.FindByNameAsync(_usuarioSesion.ObtenerUsuarioSesion());

                // Obtenemos los roles que tiene este usuario (por ejemplo: admin, estudiante, etc.)
                var resultadoRoles = await _userManager.GetRolesAsync(usuario);
                var listaRoles = new List<string>(resultadoRoles);

                // Buscamos la imagen de perfil del usuario (si tiene una)
                var imagenPerfil = await _context.Documento
                    .Where(x => x.ObjetoReferencia == new Guid(usuario.Id))
                    .FirstAsync();

                // Si el usuario tiene imagen de perfil:
                if(imagenPerfil != null) {
                    // Creamos un objeto con los datos de la imagen
                    var imagenCliente = new ImagenGeneral{
                        Data = Convert.ToBase64String(imagenPerfil.Contenido), // Convertimos la imagen a texto (base64)
                        Extension = imagenPerfil.Extension,                    // Ej: .jpg, .png
                        Nombre = imagenPerfil.Nombre                           // Nombre del archivo
                    };

                    // Retornamos los datos del usuario, incluyendo su imagen
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        Username = usuario.UserName,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles), // Generamos un nuevo token JWT
                        Email = usuario.Email,
                        ImagenPerfil = imagenCliente
                    };
                }
                else {
                    // Si el usuario no tiene imagen, devolvemos los demás datos igual
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        Username = usuario.UserName,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                        Email = usuario.Email
                    };
                }
            }
        }
    }
}
