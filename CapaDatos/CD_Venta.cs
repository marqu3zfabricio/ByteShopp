using CapaEntidad;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Venta(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");
        // Listar pedidos para administración
        public async Task<List<Venta>> ListarPedidos()
        {
            var lista = new List<Venta>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                await oconexion.OpenAsync();

                // 1. Obtener ventas con distrito
                using var cmdVentas = new SqlCommand(@"
            SELECT 
                v.IdVenta,
                v.TotalProducto,
                v.MontoTotal,
                v.Telefono,
                v.Direccion,
                CONVERT(VARCHAR, v.FechaVenta, 103) AS FechaTexto,
                v.IdTransaccion,
                v.Estado,
                d.IdDistrito,
                d.Nombre AS Distrito
            FROM VENTA v
            INNER JOIN DISTRITO d ON d.IdDistrito = v.IdDistrito
            ORDER BY v.FechaVenta DESC", oconexion);

                using var drVentas = await cmdVentas.ExecuteReaderAsync();

                while (await drVentas.ReadAsync())
                {
                    lista.Add(new Venta
                    {
                        IdVenta = Convert.ToInt32(drVentas["IdVenta"]),
                        TotalProducto = Convert.ToInt32(drVentas["TotalProducto"]),
                        MontoTotal = Convert.ToDecimal(drVentas["MontoTotal"]),
                        Telefono = drVentas["Telefono"].ToString()!,
                        Direccion = drVentas["Direccion"].ToString()!,
                        FechaTexto = drVentas["FechaTexto"].ToString()!,
                        IdTransaccion = drVentas["IdTransaccion"].ToString()!,
                        Estado = drVentas["Estado"].ToString()!,
                        oDistrito = new Distrito
                        {
                            IdDistrito = Convert.ToInt32(drVentas["IdDistrito"]),
                            Nombre = drVentas["Distrito"].ToString()!
                        },
                        oDetalleVenta = []
                    });
                }

                drVentas.Close();

                // 2. Obtener detalle por cada venta (sin cambios)
                foreach (var venta in lista)
                {
                    using var cmdDetalle = new SqlCommand(@"
                SELECT 
                    dv.IdDetalleVenta,
                    dv.IdVenta,
                    dv.IdProducto,
                    dv.Cantidad,
                    dv.Total,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.Stock,
                    p.RutaImagen,
                    p.Activo,
                    p.IdMarca,
                    p.IdCategoria,
                    m.Descripcion AS Marca,
                    c.Descripcion AS Categoria
                FROM DETALLE_VENTA dv
                INNER JOIN Producto p ON p.IdProducto = dv.IdProducto
                INNER JOIN Marca m ON m.IdMarca = p.IdMarca
                INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
                WHERE dv.IdVenta = @IdVenta", oconexion);

                    cmdDetalle.Parameters.AddWithValue("@IdVenta", venta.IdVenta);

                    using var drDetalle = await cmdDetalle.ExecuteReaderAsync();
                    while (await drDetalle.ReadAsync())
                    {
                        venta.oDetalleVenta.Add(new DetalleVenta
                        {
                            IdDetalleVenta = Convert.ToInt32(drDetalle["IdDetalleVenta"]),
                            IdVenta = Convert.ToInt32(drDetalle["IdVenta"]),
                            Cantidad = Convert.ToInt32(drDetalle["Cantidad"]),
                            Total = Convert.ToDecimal(drDetalle["Total"]),
                            oProducto = new Producto
                            {
                                IdProducto = Convert.ToInt32(drDetalle["IdProducto"]),
                                Nombre = drDetalle["Nombre"].ToString()!,
                                Descripcion = drDetalle["Descripcion"].ToString()!,
                                Precio = Convert.ToDecimal(drDetalle["Precio"]),
                                Stock = Convert.ToInt32(drDetalle["Stock"]),
                                RutaImagen = drDetalle["RutaImagen"].ToString()!,
                                Activo = Convert.ToBoolean(drDetalle["Activo"]),
                                IdMarca = Convert.ToInt32(drDetalle["IdMarca"]),
                                IdCategoria = Convert.ToInt32(drDetalle["IdCategoria"]),
                                oMarca = new Marca
                                {
                                    IdMarca = Convert.ToInt32(drDetalle["IdMarca"]),
                                    Descripcion = drDetalle["Marca"].ToString()!
                                },
                                oCategoria = new Categoria
                                {
                                    IdCategoria = Convert.ToInt32(drDetalle["IdCategoria"]),
                                    Descripcion = drDetalle["Categoria"].ToString()!
                                }
                            }
                        });
                    }

                    drDetalle.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar pedidos: " + ex.Message);
            }

            return lista;
        }
        public async Task<(bool Resultado, string Mensaje)> RegistrarVenta(
        int totalProducto,
        decimal montoTotal,
        int idDistrito,
        string telefono,
        string direccion,
        string idTransaccion,
        string contacto)
        {
            bool resultado = false;
            string mensaje = string.Empty;

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("usp_RegistrarVenta", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                cmd.Parameters.AddWithValue("@TotalProducto", totalProducto);
                cmd.Parameters.AddWithValue("@MontoTotal", montoTotal);
                cmd.Parameters.AddWithValue("@IdDistrito", idDistrito);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                cmd.Parameters.AddWithValue("@IdTransaccion", idTransaccion);
                cmd.Parameters.AddWithValue("@Contacto", contacto);

                // Parámetros de salida
                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

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

        public async Task<List<CarritoFactura>> ListarFacturaPorTransaccion(string idTransaccion)
        {
            List<CarritoFactura> lista = [];

            using (SqlConnection cn = new(_connectionString))
            {
                SqlCommand cmd = new("SP_ListarFacturaPorTransaccion", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@IdTransaccion", idTransaccion);

                cn.Open();
                SqlDataReader dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    lista.Add(new CarritoFactura
                    {
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        oProducto = new ProductoFactura
                        {
                            Nombre = dr["Nombre"].ToString()!,
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            RutaImagen = dr["RutaImagen"].ToString()!
                        }
                    });
                }
            }
            return lista;
        }

        public async Task<(bool resultado, string mensaje)> MarcarPedidoEntregado(int idVenta)
        {
            bool resultado;
            string mensaje;

            using var conexion = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_MarcarPedidoEntregado", conexion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdVenta", idVenta);
            cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 300).Direction = ParameterDirection.Output;

            await conexion.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            resultado = Convert.ToBoolean(cmd.Parameters["@Resultado"].Value);
            mensaje = cmd.Parameters["@Mensaje"].Value.ToString()!;

            return (resultado, mensaje);
        }


    }
}
