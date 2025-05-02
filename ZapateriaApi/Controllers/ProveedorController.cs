using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZapateriaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProveedorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> GetVistaPlanilla()
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM proveedor";

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
                                    DIRECCION = reader["DIRECCION"],
                                    COD_EMPRESA = reader["COD_EMPRESA"]
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