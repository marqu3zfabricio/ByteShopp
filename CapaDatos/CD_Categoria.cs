using CapaEntidad;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapaDatos
{
    public class CD_Categoria(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");

        // Listar categorías
        public async Task<List<Categoria>> ListarCategorias()
        {
            var lista = new List<Categoria>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT IdCategoria, Descripcion, Activo FROM CATEGORIA", oconexion);
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Categoria
                    {
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        Descripcion = dr["Descripcion"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar categorías: " + ex.Message);
            }

            return lista;
        }

        // Registrar categoría
        public async Task<(int idAutogenerado, string mensaje)> RegistrarCategoria(Categoria obj)
        {
            int idAutogenerado = 0;
            string mensaje ="";
            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_RegistrarCategoria", oconexion);
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
                throw new Exception("Error al registrar categoría: " + ex.Message);
            }

            return (idAutogenerado, mensaje);
        }

        // Editar categoría
        public async Task<(bool resultado, string mensaje)> EditarCategoria(Categoria obj)
        {
            bool resultado = false;
            string mensaje = "";

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EditarCategoria", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("IdCategoria", obj.IdCategoria);
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
                throw new Exception("Error al editar categoría: " + ex.Message);
            }

            return (resultado, mensaje);
        }


        // Eliminar categoría
        public async Task<(bool exito, string mensaje)> EliminarCategoria(int id)
        {
            bool resultado = false;
            string mensaje = string.Empty;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EliminarCategoria", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCategoria", id);

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
                mensaje = "Error al eliminar categoría: " + ex.Message;
            }

            return (resultado, mensaje);
        }
    }
}
