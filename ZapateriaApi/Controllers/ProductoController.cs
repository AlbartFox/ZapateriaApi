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

        [HttpPost("insertar")]
        public async Task<IActionResult> InsertarProducto(
        [FromForm] string nombre,
        [FromForm] string descripcion,
        [FromForm] decimal precio,
        [FromForm] int idUsuario)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("INSERTAR_PRODUCTO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = nombre;
                        cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = descripcion;
                        cmd.Parameters.Add("p_precio", OracleDbType.Decimal).Value = precio;
                        cmd.Parameters.Add("p_usuario", OracleDbType.Int32).Value = idUsuario;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Producto insertado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar producto: {ex.Message}");
            }
        }

        [HttpPost("cambiar-estado")]
        public async Task<IActionResult> CambiarEstadoProducto([FromForm] int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("connectionDB");

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (OracleCommand cmd = new OracleCommand("CAMBIAR_ESTADO_PRODUCTO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_id_producto", OracleDbType.Int32).Value = idProducto;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return Ok(new { message = "Estado del producto cambiado a 'I'" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cambiar estado: {ex.Message}");
            }
        }
    }
}
