using CapaEntidad;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Producto(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");

        // ============================
        // LISTAR PRODUCTOS
        // ============================
        public async Task<List<Producto>> ListarProductos()
        {
            var lista = new List<Producto>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT 
                        p.IdProducto, p.Nombre, p.Descripcion,
                        m.IdMarca, m.Descripcion AS DesMarca,
                        c.IdCategoria, c.Descripcion AS DesCategoria,
                        p.Precio, p.Stock, p.RutaImagen, p.Activo
                    FROM PRODUCTO p
                    INNER JOIN MARCA m ON m.IdMarca = p.IdMarca
                    INNER JOIN CATEGORIA c ON c.IdCategoria = p.IdCategoria", oconexion);
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!,
                        oMarca = new Marca
                        {
                            IdMarca = Convert.ToInt32(dr["IdMarca"]),
                            Descripcion = dr["DesMarca"].ToString()!
                        },
                        oCategoria = new Categoria
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Descripcion = dr["DesCategoria"].ToString()!
                        },
                        Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-SV")),
                        Stock = Convert.ToInt32(dr["Stock"]),
                        RutaImagen = dr["RutaImagen"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos: " + ex.Message);
            }

            return lista;
        }

        // ============================
        // REGISTRAR PRODUCTO
        // ============================
        public async Task<(int idGenerado, string mensaje)> RegistrarProducto(Producto obj)
        {
            int idGenerado = 0;
            string mensaje = string.Empty;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_RegistrarProducto", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);
                cmd.Parameters.AddWithValue("Precio", obj.Precio);
                cmd.Parameters.AddWithValue("Stock", obj.Stock);
                cmd.Parameters.AddWithValue("Activo", obj.Activo);

                cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                idGenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString()!;
            }
            catch (Exception ex)
            {
                mensaje = "Error al registrar producto: " + ex.Message;
            }

            return (idGenerado, mensaje);
        }


        // ============================
        // EDITAR PRODUCTO
        // ============================
        public async Task<(bool Resultado, string Mensaje)> EditarProducto(Producto obj)
        {
            bool resultado = false;
            string mensaje = "";

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EditarProducto", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);
                cmd.Parameters.AddWithValue("Precio", obj.Precio);
                cmd.Parameters.AddWithValue("Stock", obj.Stock);
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
                mensaje = "Error al editar producto: " + ex.Message;
            }

            return (resultado, mensaje);
        }


        // ============================
        // GUARDAR IMAGEN (Cloudinary URL)
        // ============================
        public async Task<bool> GuardarRutaImagen(int idProducto, string rutaImagen)
        {
            bool resultado = false;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("UPDATE PRODUCTO SET RutaImagen = @ruta WHERE IdProducto = @id", oconexion);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ruta", rutaImagen);
                cmd.Parameters.AddWithValue("@id", idProducto);

                await oconexion.OpenAsync();
                resultado = await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la ruta de la imagen: " + ex.Message);
            }

            return resultado;
        }

        // ============================
        // ELIMINAR PRODUCTO
        // ============================
        public async Task<(bool exito, string mensaje)> EliminarProducto(int id)
        {
            bool resultado = false;
            string mensaje = string.Empty;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_EliminarProducto", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("IdProducto", id);
                cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                await oconexion.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value?.ToString()!;
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar producto: " + ex.Message;
            }

            return (resultado, mensaje);
        }

        // ============================
        // OBTENER RUTA DE IMAGEN POR ID
        // ============================
        public async Task<string?> ObtenerRutaImagenPorId(int id)
        {
            string? ruta = null;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT RutaImagen FROM PRODUCTO WHERE IdProducto = @id", oconexion);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);

                await oconexion.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                if (result != DBNull.Value && result != null)
                    ruta = result.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la ruta de imagen: " + ex.Message);
            }

            return ruta;
        }
        // ============================
        // FILTRO PRODUCTOS POR CATEGORIA
        // ============================
        public async Task<List<Producto>> FiltroProductosCategoria(int idCategoria)
        {
            var lista = new List<Producto>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
            SELECT 
                p.IdProducto,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Stock,
                p.RutaImagen,
                p.Activo,
                p.IdCategoria,
                c.Descripcion AS CategoriaDescripcion,
                p.IdMarca,
                m.Descripcion AS MarcaDescripcion
            FROM Producto p
            INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
            INNER JOIN Marca m ON m.IdMarca = p.IdMarca
            WHERE p.IdCategoria = @idcategoria
              AND p.Activo = 1
        ", oconexion);

                cmd.Parameters.Add("@idcategoria", SqlDbType.Int).Value = idCategoria;

                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!,
                        Precio = Convert.ToDecimal(dr["Precio"]),
                        Stock = Convert.ToInt32(dr["Stock"]),
                        RutaImagen = dr["RutaImagen"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                        oCategoria = new Categoria
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Descripcion = dr["CategoriaDescripcion"].ToString()!
                        },
                        oMarca = new Marca
                        {
                            IdMarca = Convert.ToInt32(dr["IdMarca"]),
                            Descripcion = dr["MarcaDescripcion"].ToString()!
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos por categoría: " + ex.Message);
            }

            return lista;
        }
        // ============================
        // FILTRO PRODUCTOS POR MARCA
        // ============================
        public async Task<List<Producto>> FiltroProductosMarca(int idMarca)
        {
            var lista = new List<Producto>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
    SELECT 
        p.IdProducto,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.Stock,
        p.RutaImagen,
        p.Activo,
        p.IdCategoria,
        c.Descripcion AS CategoriaDescripcion,
        p.IdMarca,
        m.Descripcion AS MarcaDescripcion
    FROM Producto p
    INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
    INNER JOIN Marca m ON m.IdMarca = p.IdMarca
    WHERE p.IdMarca = @idmarca
      AND p.Activo = 1
", oconexion);

                cmd.Parameters.Add("@idmarca", SqlDbType.Int).Value = idMarca;

                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!,
                        Precio = Convert.ToDecimal(dr["Precio"]),
                        Stock = Convert.ToInt32(dr["Stock"]),
                        RutaImagen = dr["RutaImagen"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                        oCategoria = new Categoria
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Descripcion = dr["CategoriaDescripcion"].ToString()!
                        },
                        oMarca = new Marca
                        {
                            IdMarca = Convert.ToInt32(dr["IdMarca"]),
                            Descripcion = dr["MarcaDescripcion"].ToString()!
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos por marca: " + ex.Message);
            }

            return lista;
        }

        // ============================
        // FILTRO PRODUCTOS POR CATEGORIA Y MARCA
        // ============================
        public async Task<List<Producto>> FiltroProductosCategoriaMarca(int idCategoria, int idMarca)
        {
            var lista = new List<Producto>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
            SELECT 
                p.IdProducto,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Stock,
                p.RutaImagen,
                p.Activo,
                p.IdCategoria,
                c.Descripcion AS CategoriaDescripcion,
                p.IdMarca,
                m.Descripcion AS MarcaDescripcion
            FROM Producto p
            INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
            INNER JOIN Marca m ON m.IdMarca = p.IdMarca
            WHERE p.IdCategoria = @idcategoria
              AND p.IdMarca = @idmarca
              AND p.Activo = 1
        ", oconexion);

                cmd.Parameters.Add("@idcategoria", SqlDbType.Int).Value = idCategoria;
                cmd.Parameters.Add("@idmarca", SqlDbType.Int).Value = idMarca;

                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!,
                        Precio = Convert.ToDecimal(dr["Precio"]),
                        Stock = Convert.ToInt32(dr["Stock"]),
                        RutaImagen = dr["RutaImagen"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        IdMarca = Convert.ToInt32(dr["IdMarca"]),
                        oCategoria = new Categoria
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Descripcion = dr["CategoriaDescripcion"].ToString()!
                        },
                        oMarca = new Marca
                        {
                            IdMarca = Convert.ToInt32(dr["IdMarca"]),
                            Descripcion = dr["MarcaDescripcion"].ToString()!
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos por categoría y marca: " + ex.Message);
            }

            return lista;
        }




    }
}
