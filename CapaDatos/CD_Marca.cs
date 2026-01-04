using CapaEntidad;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Marca(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");

        //listar marcas
        public async Task<List<Marca>> ListarMarcas()
        {
            var lista = new List<Marca>();
            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                string query = "SELECT IdMarca, Descripcion, Activo FROM Marca";
                using var cmd = new SqlCommand(query, oconexion);
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();
                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Marca
                    {
                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                        Descripcion = dr["Descripcion"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar marcas: " + ex.Message);
            }
            return lista;
        }
        
        // Registrar marca
        public async Task<(int idAutogenerado, string mensaje)> RegistrarMarca(Marca obj)
        {
            int idAutogenerado = 0;
            string mensaje = "";
            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_RegistrarMarca", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                cmd.Parameters.AddWithValue("Activo", obj.Activo);
                cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                idAutogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar marca: " + ex.Message);
            }

            return (idAutogenerado, mensaje);
        }

        // Editar marca
        public async Task<(bool resultado, string mensaje)> EditarMarca(Marca obj)
        {
            bool resultado = false;
            string mensaje = "";

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EditarMarca", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("IdMarca", obj.IdMarca);
                cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                cmd.Parameters.AddWithValue("Activo", obj.Activo);

                cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar marca: " + ex.Message);
            }

            return (resultado, mensaje);
        }

        //eliminar marca
        public async Task<(bool exito, string mensaje)> EliminarMarca(int id)
        {
            bool resultado = false;
            string mensaje = string.Empty;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EliminarMarca", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMarca", id);

                var paramResultado = cmd.Parameters.Add("@Resultado", SqlDbType.Bit);
                paramResultado.Direction = ParameterDirection.Output;

                var paramMensaje = cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 500);
                paramMensaje.Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(paramResultado.Value);
                mensaje = paramMensaje.Value?.ToString()!;
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar marca: " + ex.Message;
            }

            return (resultado, mensaje);
        }

        //filtro marcas por categoria
        public async Task<List<Marca>> ListarMarcasPorCategoria(int idCategoria)
        {
            var lista = new List<Marca>();

            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
        SELECT DISTINCT m.IdMarca, m.Descripcion
        FROM Producto p
        INNER JOIN Marca m ON m.IdMarca = p.IdMarca
        WHERE p.IdCategoria = @idCategoria
          AND p.Activo = 1
        ORDER BY m.Descripcion
    ", cn);

            cmd.Parameters.Add("@idCategoria", SqlDbType.Int).Value = idCategoria;

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Marca
                {
                    IdMarca = Convert.ToInt32(dr["IdMarca"]),
                    Descripcion = dr["Descripcion"].ToString()!
                });
            }

            return lista;
        }

    }
}
