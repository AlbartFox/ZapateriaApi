using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
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

        [HttpPost("insertar")]
        public async Task<IActionResult> InsertarProveedor(
        [FromForm] string nombre,
        [FromForm] string direccion,
        [FromForm] int idUsuario)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("INSERTAR_PROVEEDOR", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre;
                        cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = direccion;
                        cmd.Parameters.Add("p_usuario", OracleDbType.Int32).Value = idUsuario;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Proveedor insertado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar proveedor: {ex.Message}");
            }
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarProveedor(
        [FromForm] int idProveedor,
        [FromForm] string nombre,
        [FromForm] string direccion,
        [FromForm] int idUsuario)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("ACTUALIZAR_PROVEEDOR", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_id_proveedor", OracleDbType.Int32).Value = idProveedor;
                        cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre;
                        cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = direccion;
                        cmd.Parameters.Add("p_usuario", OracleDbType.Int32).Value = idUsuario;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Proveedor actualizado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar proveedor: {ex.Message}");
            }
        }

        [HttpPut("cambiar-estado")]
        public async Task<IActionResult> CambiarEstadoProveedor([FromForm] int idProveedor)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("CAMBIAR_ESTADO_PROVEEDOR", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_id_proveedor", OracleDbType.Int32).Value = idProveedor;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Estado del proveedor cambiado a 'I'" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cambiar estado: {ex.Message}");
            }
        }
    }

}