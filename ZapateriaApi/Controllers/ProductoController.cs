using Oracle.ManagedDataAccess.Client;
using Oracle.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ZapateriaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProductoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> GetVistaProducto()
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM VW_PRODUCTO";

                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {
                        using (OracleDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var resultados = new List<object>();

                            while (await reader.ReadAsync())
                            {
                                var item = new
                                {
                                    NOMBRE = reader["NOMBRE"],
                                    DESCRIPCION = reader["DESCRIPCION"],
                                    PRECIO = reader["PRECIO"],
                                    CANTIDAD = reader["CANTIDAD"]
                                };

                                resultados.Add(item);
                            }

                            return Ok(resultados);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al acceder a Oracle: {ex.Message}");
            }
        }
    }
}
