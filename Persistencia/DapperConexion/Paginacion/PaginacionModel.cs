using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionModel
    {
        public List<IDictionary<string, object>> ListaRecords {get; set;}
        // [{cursoId : "123123", "titulo" : "aspnet"}, {"cursoId" : "12322, "titulo" : "react"}]
        public int TotalRecords {get; set;}
        public int NumeroPaginas {get; set;} 
    }
}