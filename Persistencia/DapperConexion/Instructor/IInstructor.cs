using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
        Task<IEnumerable<InstructorModel>> ObtenerLista();
        Task<InstructorModel> ObtenerPorId(Guid id);
        Task<int> Nuevo(string nombre, string apellidos, string titulo);
        Task<int> Actualiza(Guid instructorId, string nombre, string apellidos, string titulo);
        Task<int> Elimina(Guid id);

    }
}