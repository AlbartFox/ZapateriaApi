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
    public class CuentasPorCobrarController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CuentasPorCobrarController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> GetVistaCuentasPorCobrar()
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM VW_CREDITO_DETALLE_CLIENTE";

                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {
                        using (OracleDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var resultados = new List<object>();

                            while (await reader.ReadAsync())
                            {
                                var item = new
                                {
                                    ID_ENCABEZADO_VENTA = reader["ID_ENCABEZADO_VENTA"],
                                    CLIENTE = reader["CLIENTE"],
                                    USUARIO = reader["USUARIO"],
                                    CREDITO_TOTAL = reader["CREDITO_TOTAL"],
                                    MONTO_PAGADO = reader["MONTO_PAGADO"],
                                    SALDO_PENDIENTE = reader["SALDO_PENDIENTE"],
                                    ESTADO = reader["ESTADO"],
                                    FECHA_EMISION = reader["FECHA_EMISION"],
                                    FECHA_VENCIMIENTO = reader["FECHA_VENCIMIENTO"]
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
