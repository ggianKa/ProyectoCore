using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Paginacion
{
    /// Repositorio para manejar la paginación de datos utilizando procedimientos almacenados con Dapper.
    public class PaginacionRepositorio : IPaginacion
    {
        /// Objeto para manejar la conexión a la base de datos.
        private readonly IFactoryConnection _factoryConnection;
        
        /// Constructor que inicializa la conexión a la base de datos.
        public PaginacionRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }
        
        /// Método para obtener una paginación de registros desde un procedimiento almacenado.
        public async Task<PaginacionModel> devolverPaginacion(string storeProcedure, int numeroPagina, int cantidadElementos, IDictionary<string, object> parametrosFiltro, string ordenamientoColumna)
        {
            PaginacionModel paginacionModel = new PaginacionModel();
            List<IDictionary<string, object>> listaReporte = null;
            int totalRecords  = 0; // Total de registros en la base de datos.
            int totalPaginas = 0;  // Número total de páginas disponibles.
            
            try
            {
                // Obtiene una conexión a la base de datos.
                var connection = _factoryConnection.GetConnection();
                
                // Crea una instancia de DynamicParameters para manejar los parámetros del procedimiento almacenado.
                DynamicParameters parametros = new DynamicParameters();

                // Agrega los parámetros de filtro al procedimiento almacenado.
                foreach(var param in parametrosFiltro)
                {
                    parametros.Add("@" + param.Key, param.Value);
                }
                
                // Agrega los parámetros de paginación y ordenamiento.
                parametros.Add("@NumeroPagina", numeroPagina);
                parametros.Add("@CantidadElementos", cantidadElementos);
                parametros.Add("@Ordenamiento", ordenamientoColumna);
                
                // Agrega parámetros de salida para obtener el total de registros y páginas.
                parametros.Add("@TotalRecords", totalRecords, DbType.Int32, ParameterDirection.Output);
                parametros.Add("@TotalPaginas", totalPaginas, DbType.Int32, ParameterDirection.Output);
                
                // Ejecuta el procedimiento almacenado y obtiene los resultados.
                var result = await connection.QueryAsync(storeProcedure, parametros, commandType: CommandType.StoredProcedure);
                
                // Convierte los resultados en una lista de diccionarios.
                listaReporte = result.Select(x => (IDictionary<string, object>)x).ToList();
                
                // Asigna los resultados al modelo de paginación.
                paginacionModel.ListaRecords = listaReporte;
                paginacionModel.NumeroPaginas = parametros.Get<int>("@TotalPaginas");
                paginacionModel.TotalRecords = parametros.Get<int>("@TotalRecords");
            }
            catch (Exception e)
            {
                // Lanza una excepción si ocurre un error en la ejecución del procedimiento almacenado.
                throw new Exception("No se pudo ejecutar el procedimiento almacenado", e);
            }
            finally
            {
                // Cierra la conexión a la base de datos.
                _factoryConnection.CloseConnection();
            }
            
            return paginacionModel;
        }
    }
}
