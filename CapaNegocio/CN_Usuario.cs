using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Usuario(CD_Usuario datos, CN_Recursos recursos)
    {
        private readonly CD_Usuario _datos = datos;
        private readonly CN_Recursos _recursos = recursos;

        // ============================
        // LISTAR USUARIOS
        // ============================
        public async Task<List<Usuario>> ListarUsuarios()
        {
            return await _datos.ListarUsuarios();
        }
        // ============================
        // LISTAR USUARIO POR ID 
        // ============================
        public async Task<Usuario?> ListarPorIdAsync(int idUsuario)
        {
            return await _datos.ListarUsuarioPorId(idUsuario);
        }


        // ============================
        // REGISTRAR USUARIO
        // ============================
        public async Task<(int idGenerado, string mensaje)> RegistrarUsuario(Usuario obj)
        {
            string mensaje = string.Empty;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(obj.Nombres))
                mensaje = "El nombre del usuario no puede estar vacío.";
            else if (string.IsNullOrWhiteSpace(obj.Apellidos))
                mensaje = "El apellido del usuario no puede estar vacío.";
            else if (string.IsNullOrWhiteSpace(obj.Correo))
                mensaje = "El correo del usuario no puede estar vacío.";

            if (!string.IsNullOrEmpty(mensaje))
                return (0, mensaje);

            // Generar clave aleatoria
            string clave = CN_Recursos.GenerarClave();
            string asunto = "Creación de cuenta de usuario";
            string mensajeCorreo = "<h3>Su cuenta fue creada correctamente</h3></br><p>Su contraseña es: !clave! Al ingresar sus credenciales debera reestablecer su contraseña por una propia para ingresar al sistema</p>";
            mensajeCorreo = mensajeCorreo.Replace("!clave!", clave);

            bool correoEnviado = CN_Recursos.EnviarCorreo(obj.Correo, asunto, mensajeCorreo);

            if (!correoEnviado)
                return (0, "No se pudo enviar el correo al usuario.");

            // Guardar usuario
            obj.Clave = CN_Recursos.ConvertirSha256(clave);

            try
            {
                int idGenerado = await _datos.RegistrarUsuario(obj);
                return (idGenerado, idGenerado > 0 ? "Usuario registrado correctamente." : "No se pudo registrar el usuario, verifique que no exista otro usuario con el mismo correo.");
            }
            catch (Exception ex)
            {
                return (0, "Error al registrar usuario: " + ex.Message);
            }
        }

        // ============================
        // EDITAR USUARIO
        // ============================
        public async Task<(bool exito, string mensaje)> EditarUsuario(Usuario obj)
        {
            string mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.Nombres))
                mensaje = "El nombre del usuario no puede estar vacío.";
            else if (string.IsNullOrWhiteSpace(obj.Apellidos))
                mensaje = "El apellido del usuario no puede estar vacío.";
            else if (string.IsNullOrWhiteSpace(obj.Correo))
                mensaje = "El correo del usuario no puede estar vacío.";

            if (!string.IsNullOrEmpty(mensaje))
                return (false, mensaje);

            try
            {
                bool exito = await _datos.EditarUsuario(obj);
                return (exito, exito ? "Usuario actualizado correctamente." : "No se pudo actualizar el usuario.");
            }
            catch (Exception ex)
            {
                return (false, "Error al editar usuario: " + ex.Message);
            }
        }

        // ============================
        // ELIMINAR USUARIO
        // ============================
        public async Task<(bool exito, string mensaje)> EliminarUsuario(int id)
        {
            try
            {
                return await _datos.EliminarUsuario(id);
            }
            catch (Exception ex)
            {
                return (false, "Error al eliminar usuario: " + ex.Message);
            }
        }

        // ============================
        // REESTABLECER CLAVE
        // ============================
        public async Task<(bool exito, string mensaje)> ReestablecerClave(int idUsuario, string nuevaClave)
        {
            try
            {
                bool exito = await _datos.ReestablecerClave(idUsuario, CN_Recursos.ConvertirSha256(nuevaClave));
                string mensaje = exito ? "Clave actualizada correctamente." : "No se pudo actualizar la contraseña.";
                return (exito, mensaje);
            }
            catch (Exception ex)
            {
                return (false, "Error al cambiar clave: " + ex.Message);
            }
        }

        
        

    }
}
