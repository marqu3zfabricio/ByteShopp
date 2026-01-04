using CapaEntidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CapaDatos
{
    public class CD_Usuario(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString()
                ?? throw new Exception("Cadena de conexión no encontrada.");

        // ============================
        // LISTAR USUARIOS
        // ============================
        public async Task<List<Usuario>> ListarUsuarios()
        {
            var lista = new List<Usuario>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT IdUsuario, Nombres, Apellidos, Correo, Clave, Reestablecer, Activo, Rol FROM Usuario", oconexion);
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Usuario
                    {
                        IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        Nombres = dr["Nombres"].ToString()!,
                        Apellidos = dr["Apellidos"].ToString()!,
                        Correo = dr["Correo"].ToString()!,
                        Clave = dr["Clave"].ToString()!,
                        Reestablecer = Convert.ToBoolean(dr["Reestablecer"]),
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        Rol = dr["Rol"].ToString()!

                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar usuarios: " + ex.Message);
            }

            return lista;
        }
        // ============================
        // LISTAR USUARIO POR ID
        // ============================
        public async Task<Usuario?> ListarUsuarioPorId(int idUsuario)
        {
            Usuario? usuario = null;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT IdUsuario, Nombres, Apellidos, Correo, Clave, Reestablecer, Activo, Rol FROM Usuario WHERE IdUsuario = @IdUsuario", oconexion);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                if (await dr.ReadAsync())
                {
                    usuario = new Usuario
                    {
                        IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        Nombres = dr["Nombres"].ToString()!,
                        Apellidos = dr["Apellidos"].ToString()!,
                        Correo = dr["Correo"].ToString()!,
                        Clave = dr["Clave"].ToString()!,
                        Reestablecer = Convert.ToBoolean(dr["Reestablecer"]),
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        Rol = dr["Rol"].ToString()!
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuario por ID: " + ex.Message);
            }

            return usuario;
        }


        // ============================
        // REGISTRAR USUARIO
        // ============================
        public async Task<int> RegistrarUsuario(Usuario obj)
        {
            int idAutogenerado = 0;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_RegistrarUsuario", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                cmd.Parameters.AddWithValue("Correo", obj.Correo);
                cmd.Parameters.AddWithValue("Clave", obj.Clave);
                cmd.Parameters.AddWithValue("Activo", obj.Activo);
                cmd.Parameters.AddWithValue("Rol", obj.Rol ?? "Usuario");

                var paramResultado = cmd.Parameters.Add("Resultado", SqlDbType.Int);
                paramResultado.Direction = ParameterDirection.Output;

                var paramMensaje = cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500);
                paramMensaje.Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                idAutogenerado = Convert.ToInt32(paramResultado.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar usuario: " + ex.Message);
            }

            return idAutogenerado;
        }

        // ============================
        // EDITAR USUARIO
        // ============================
        public async Task<bool> EditarUsuario(Usuario obj)
        {
            bool resultado = false;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EditarUsuario", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                cmd.Parameters.AddWithValue("Correo", obj.Correo);
                cmd.Parameters.AddWithValue("Activo", obj.Activo);
                cmd.Parameters.AddWithValue("Rol", obj.Rol);

                var paramResultado = cmd.Parameters.Add("Resultado", SqlDbType.Bit);
                paramResultado.Direction = ParameterDirection.Output;

                var paramMensaje = cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500);
                paramMensaje.Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(paramResultado.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar usuario: " + ex.Message);
            }

            return resultado;
        }

        // ============================
        // ELIMINAR USUARIO
        // ============================
        public async Task<(bool exito, string mensaje)> EliminarUsuario(int id)
        {
            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    "DELETE TOP (1) FROM Usuario WHERE IdUsuario = @id",
                    oconexion
                );

                cmd.Parameters.AddWithValue("@id", id);

                await oconexion.OpenAsync();
                int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                if (filasAfectadas > 0)
                {
                    return (true, "Usuario eliminado correctamente");
                }

                return (false, "No se encontró el usuario a eliminar");
            }
            catch (Exception)
            {
                return (false, "Ocurrió un error al eliminar el usuario");
            }
        }


        // ============================
        // REESTABLECER CLAVE
        // ============================
        public async Task<bool> ReestablecerClave(int idUsuario, string nuevaClave)
        {
            bool resultado = false;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("UPDATE Usuario SET Clave = @nuevaClave, Reestablecer = 0 WHERE IdUsuario = @id", oconexion);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", idUsuario);
                cmd.Parameters.AddWithValue("@nuevaClave", nuevaClave);

                await oconexion.OpenAsync();
                resultado = await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar clave: " + ex.Message);
            }

            return resultado;
        }

        
    }
}
