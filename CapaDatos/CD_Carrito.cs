using CapaEntidad;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Carrito (MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");


        //listar los productos del carrito 
        public async Task<List<Carrito>> ListarCarrito()
        {
            var lista = new List<Carrito>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);

                using var cmd = new SqlCommand(@"
            SELECT
                c.IdCarrito,
                c.Cantidad,

                p.IdProducto,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Stock,
                p.RutaImagen,
                p.Activo,

                m.IdMarca,
                m.Descripcion AS DesMarca,

                cat.IdCategoria,
                cat.Descripcion AS DesCategoria
            FROM CARRITO c
            INNER JOIN PRODUCTO p ON p.IdProducto = c.IdProducto
            INNER JOIN MARCA m ON m.IdMarca = p.IdMarca
            INNER JOIN CATEGORIA cat ON cat.IdCategoria = p.IdCategoria
        ", oconexion);

                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Carrito
                    {
                        IdCarrito = Convert.ToInt32(dr["IdCarrito"]),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),

                        oProducto = new Producto
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Nombre = dr["Nombre"].ToString()!,
                            Descripcion = dr["Descripcion"].ToString()!,
                            Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-SV")),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            RutaImagen = dr["RutaImagen"].ToString()!,
                            Activo = Convert.ToBoolean(dr["Activo"]),

                            IdMarca = Convert.ToInt32(dr["IdMarca"]),
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),

                            oMarca = new Marca
                            {
                                Descripcion = dr["DesMarca"].ToString()!
                            },

                            oCategoria = new Categoria
                            {
                                IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                                Descripcion = dr["DesCategoria"].ToString()!,
                                Activo = true
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar carrito: " + ex.Message);
            }

            return lista;
        }
        public async Task<(bool Resultado, string Mensaje)> OperacionCarrito(int idProducto, bool sumar)
        {
            bool resultado = false;
            string mensaje = string.Empty;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_OperacionCarrito", oconexion);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                cmd.Parameters.AddWithValue("@Sumar", sumar);

                cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(cmd.Parameters["@Resultado"].Value);
                mensaje = cmd.Parameters["@Mensaje"].Value?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = ex.Message;
            }

            return (resultado, mensaje);
        }
        // Verificar si un producto ya existe en el carrito
        public async Task<bool> ExisteCarrito(int idProducto)
        {
            bool resultado = false;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_ExisteCarrito", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                var paramResultado = new SqlParameter("@Resultado", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramResultado);

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(paramResultado.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar existencia en carrito: " + ex.Message);
            }

            return resultado;
        }
        //eliminar producto del carrito 
        public async Task<bool> EliminarCarrito(int idProducto)
        {
            bool resultado = false;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EliminarCarrito", oconexion);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                var paramResultado = new SqlParameter("@Resultado", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramResultado);

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(paramResultado.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar producto del carrito: " + ex.Message);
            }

            return resultado;
        }





    }
}
