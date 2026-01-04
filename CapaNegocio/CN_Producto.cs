using CapaDatos;
using CapaEntidad;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Producto(CD_Producto capaDatos)
    {
        private readonly CD_Producto _objCapaDatos = capaDatos;

        // ============================
        // LISTAR PRODUCTOS
        // ============================
        public async Task<List<Producto>> ListarProductos()
        {
            return await _objCapaDatos.ListarProductos();
        }
        public async Task<List<Producto>> ListarProductosActivos()
        {
            
            return (await _objCapaDatos.ListarProductos())
       .Where(p => p.Activo )
       .ToList();
        }



        // ============================
        // REGISTRAR PRODUCTO
        // ============================
        public async Task<(int idGenerado, string mensaje)> RegistrarProducto(Producto obj)
        {
            ValidarProducto(obj);
            return await _objCapaDatos.RegistrarProducto(obj);
        }


        // ============================
        // EDITAR PRODUCTO
        // ============================
        public async Task<(bool Resultado, string Mensaje)> EditarProducto(Producto obj)
        {
            if (obj.IdProducto <= 0)
                throw new ArgumentException("El ID del producto no es válido");

            ValidarProducto(obj);

            return await _objCapaDatos.EditarProducto(obj);
        }
        public async Task<string?> ObtenerRutaImagenPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del producto no es válido");

            return await _objCapaDatos.ObtenerRutaImagenPorId(id);
        }


        // ============================
        // GUARDAR RUTA DE IMAGEN
        // ============================
        public async Task<bool> GuardarRutaImagen(int idProducto, string rutaImagen)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto no es válido");

            if (string.IsNullOrWhiteSpace(rutaImagen))
                throw new ArgumentException("La ruta de la imagen no puede estar vacía");

            return await _objCapaDatos.GuardarRutaImagen(idProducto, rutaImagen);
        }

        // ============================
        // ELIMINAR PRODUCTO 
        // ============================
        public async Task<(bool exito, string? rutaImagen, string mensaje)> EliminarProducto(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del producto no es válido");

            try
            {
                string? rutaImagen = await _objCapaDatos.ObtenerRutaImagenPorId(id);

                var (exito, mensaje) = await _objCapaDatos.EliminarProducto(id);

                return (exito, rutaImagen, mensaje);
            }
            catch (Exception ex)
            {
                return (false, null, "Error al eliminar producto: " + ex.Message);
            }
        }
        // ============================
        // FILTRO PRODUCTO POR CATEGORIA 
        // ============================
        public async Task<List<Producto>> FiltrarProductosPorCategoria(int idCategoria)
        {
            if (idCategoria <= 0)
                throw new ArgumentException("El ID de la categoría no es válido");

            return await _objCapaDatos.FiltroProductosCategoria(idCategoria);
        }
        // ============================
        // FILTRO PRODUCTO POR MARCA 
        // ============================
        public async Task<List<Producto>> FiltrarProductosPorMarca(int idMarca)
        {
            if (idMarca <= 0)
                throw new ArgumentException("El ID de la marca no es válido");

            return await _objCapaDatos.FiltroProductosMarca(idMarca);
        }

        // ============================
        // FILTRO PRODUCTOS POR CATEGORIA Y MARCA
        // ============================
        public async Task<List<Producto>> FiltrarProductosPorCategoriaMarca(int idCategoria, int idMarca)
        {
            if (idCategoria <= 0)
                throw new ArgumentException("El ID de la categoría no es válido");

            if (idMarca <= 0)
                throw new ArgumentException("El ID de la marca no es válido");

            return await _objCapaDatos.FiltroProductosCategoriaMarca(idCategoria, idMarca);
        }



        // ============================
        // MÉTODO PRIVADO DE VALIDACIÓN
        // ============================
        private static void ValidarProducto(Producto obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Nombre))
                throw new ArgumentException("El nombre del producto no puede estar vacío");

            if (string.IsNullOrWhiteSpace(obj.Descripcion))
                throw new ArgumentException("La descripción del producto no puede estar vacía");

            if (obj.oMarca == null || obj.oMarca.IdMarca <= 0)
                throw new ArgumentException("Debe seleccionar una marca válida");

            if (obj.oCategoria == null || obj.oCategoria.IdCategoria <= 0)
                throw new ArgumentException("Debe seleccionar una categoría válida");

            if (obj.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero");

            if (obj.Stock < 0)
                throw new ArgumentException("El stock no puede ser negativo");
        }
    }
}
