using Oracle.ManagedDataAccess.Client;
using Oracle.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace ZapateriaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ClienteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> GetVistaCliente()
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM cliente";

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
                                    APELLIDO = reader["APELLIDO"],
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
        public async Task<IActionResult> InsertarCliente(
         [FromForm] string nombre,
         [FromForm] string apellido,
         [FromForm] string direccion,
         [FromForm] int idUsuario)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("INSERTAR_CLIENTE", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre;
                        cmd.Parameters.Add("p_apellido", OracleDbType.Varchar2).Value = apellido;
                        cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = direccion;
                        cmd.Parameters.Add("p_usuario", OracleDbType.Int32).Value = idUsuario;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Cliente insertado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar cliente: {ex.Message}");
            }
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarCliente(
        [FromForm] int idCLiente,
        [FromForm] string nombre,
        [FromForm] string apellido,
        [FromForm] string direccion,
        [FromForm] int idUsuario)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("ACTUALIZAR_CLIENTE", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCLiente;
                        cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre;
                        cmd.Parameters.Add("p_apellido", OracleDbType.Varchar2).Value = apellido;
                        cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = direccion;
                        cmd.Parameters.Add("p_usuario", OracleDbType.Int32).Value = idUsuario;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Cliente actualizado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar cliente: {ex.Message}");
            }
        }

        [HttpPut("cambiar-estado")]
        public async Task<IActionResult> CambiarEstadoProducto([FromForm] int idCliente)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("CAMBIAR_ESTADO_CLIENTE", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_id_cliente", OracleDbType.Int32).Value = idCliente;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Estado del cliente cambiado a 'I'" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cambiar estado: {ex.Message}");
            }
        }
    }
}