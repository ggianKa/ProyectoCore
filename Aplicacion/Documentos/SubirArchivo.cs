using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace Aplicacion.Documentos
{
    // Esta clase representa la funcionalidad para subir un archivo (imagen, PDF, etc.)
    public class SubirArchivo
    {
        // Esta clase contiene los datos que se reciben al momento de subir un archivo
        public class Ejecuta : IRequest
        {
            public Guid? ObjetoReferencia { get; set; } // ID del objeto al que está relacionado el archivo (ej. un usuario)
            public string Data { get; set; }            // El contenido del archivo codificado en Base64
            public string Nombre { get; set; }          // El nombre del archivo (ej. "foto_perfil.jpg")
            public string Extension { get; set; }       // La extensión del archivo (ej. ".jpg", ".pdf")
        }

        // Esta clase maneja la lógica de guardar o actualizar el archivo en la base de datos
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;

            // Constructor que recibe el contexto de la base de datos
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            // Método principal que se ejecuta cuando se sube el archivo
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                // Buscar si ya existe un archivo relacionado con ese objeto (por ID)
                var documento = await _context.Document
                    .Where(x => x.ObjetoReferencia == request.ObjetoReferencia)
                    .FirstOrDefaultAsync();

                // Si NO existe, se crea un nuevo archivo
                if (documento == null)
                {
                    var doc = new Documento
                    {
                        Contenido = Convert.FromBase64String(request.Data), // Convertir el texto Base64 a bytes reales del archivo
                        Nombre = request.Nombre,
                        Extension = request.Extension,
                        DocumentoId = Guid.NewGuid(), // Crear un nuevo ID único
                        FechaCreacion = DateTime.UtcNow // Fecha actual (en UTC)
                    };

                    _context.Documento.Add(doc); // Agregar el nuevo archivo a la base de datos
                }
                else
                {
                    // Si ya existe un archivo, se actualiza su contenido
                    documento.Contenido = Convert.FromBase64String(request.Data);
                    documento.Nombre = request.Nombre;
                    documento.Extension = request.Extension;
                    documento.FechaCreacion = DateTime.UtcNow;
                }

                // Guardar los cambios en la base de datos
                var resultado = await _context.SaveChangesAsync();

                // Si se guardó correctamente, devolvemos Unit.Value (indica éxito)
                if (resultado > 0)
                {
                    return Unit.Value;
                }

                // Si ocurrió un error, lanzamos una excepción
                throw new Exception("No se pudo guardar el archivo");
            }
        }
    }
}
