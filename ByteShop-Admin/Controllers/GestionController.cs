using ByteShop_Admin.Helpers;
using CapaEntidad;
using CapaNegocio;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ByteShop_Admin.Controllers
{
    
    public class GestionController(
        CN_Marca negocioMarca,
        CN_Categoria negocioCategoria,
        CN_Producto negocioProducto,
        CN_Usuario negocioUsuario,
        CN_Venta negocioVenta,
        Cloudinary cloudinary) : Controller
    {
        private readonly CN_Usuario _negocioUsuario = negocioUsuario;    
        private readonly CN_Marca _negocioMarca = negocioMarca;
        private readonly CN_Categoria _negocioCategoria = negocioCategoria;
        private readonly CN_Producto _negocioProducto = negocioProducto;
        private readonly CN_Venta _negocioVenta = negocioVenta;
        private readonly Cloudinary _cloudinary = cloudinary;

        // Vistas
        public IActionResult Categoria() => View();
        public IActionResult Marca() => View();
        public IActionResult Producto() => View();
        public IActionResult Usuarios() => View();
        public IActionResult Pedidos() => View();

        
        // ===========================
        // ACCIONES DE CATEGORÍA
        // ===========================

        // Listar categorías como JSON
        [HttpGet]
        public async Task<JsonResult> ListarCategorias()
        {
            var lista = await _negocioCategoria.ListarCategorias();
            return Json(new { data = lista });
        }

        // Registrar o editar categoría
        [HttpPost]
        public async Task<JsonResult> GuardarCategoria([FromBody] Categoria categoria)
        {
            try
            {
                if (categoria.IdCategoria == 0)
                {
                    var (id, mensaje) = await _negocioCategoria.RegistrarCategoria(categoria);

                    bool exito = id != 0;

                    return Json(new
                    {
                        exito,
                        mensaje = exito ? "Categoría guardada correctamente" : mensaje
                    });
                }
                else
                {
                    var (resultado, mensaje) = await _negocioCategoria.EditarCategoria(categoria);

                    return Json(new
                    {
                        exito = resultado,
                        mensaje = resultado
                            ? "Categoría actualizada correctamente"
                            : mensaje // ← aquí aparece “La categoría ya existe”
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }


        // Eliminar categoría
        [HttpDelete]
        public async Task<JsonResult> EliminarCategoria(int id)
        {
            try
            {
                var (exito, mensaje) = await _negocioCategoria.EliminarCategoria(id);
                return Json(new { exito, mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
        // ===========================
        // ACCIONES DE MARCA (ya existentes)
        // ===========================

        // Acción para listar marcas como JSON
        [HttpGet]
        public async Task<JsonResult> ListarMarcas()
        {
            var lista = await _negocioMarca.ListarMarcas();
            return Json(new { data = lista });
        }

        // Registrar o editar marca
        [HttpPost]
        public async Task<JsonResult> GuardarMarca([FromBody] Marca marca)
        {
            try
            {
                if (marca.IdMarca == 0)
                {
                    var (id, mensaje) = await _negocioMarca.RegistrarMarca(marca);

                    bool exito = id != 0;

                    return Json(new
                    {
                        exito,
                        mensaje = exito ? "Marca guardada correctamente" : mensaje
                    });
                }
                else
                {
                    var (resultado, mensaje) = await _negocioMarca.EditarMarca(marca);

                    return Json(new
                    {
                        exito = resultado,
                        mensaje = resultado
                            ? "Marca actualizada correctamente"
                            : mensaje // ← Mensaje desde el SP (p. ej. "La marca ya existe")
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }



        // Acción para eliminar marca
        [HttpDelete]
        public async Task<JsonResult> EliminarMarca(int id)
        {
            try
            {
                var (exito, mensaje) = await _negocioMarca.EliminarMarca(id);
                return Json(new { exito, mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // ===========================
        // ACCIONES DE PRODUCTO
        // ===========================

        // Listar productos
        [HttpGet]
        public async Task<JsonResult> ListarProductos()
        {
            var lista = await _negocioProducto.ListarProductos();
            return Json(new { data = lista });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarProducto(string objeto, IFormFile archivoImagen)
        {
            try
            {
                var oProducto = JsonConvert.DeserializeObject<Producto>(objeto);
                if (oProducto == null)
                    return Json(new { exito = false, mensaje = "Producto inválido" });

                bool esNuevo = oProducto.IdProducto == 0;

                if (esNuevo && (archivoImagen == null || archivoImagen.Length == 0))
                    return Json(new { exito = false, mensaje = "Debe seleccionar una imagen para el producto nuevo." });

                string? rutaAnterior = null;

                if (!esNuevo)
                {
                    if (archivoImagen != null && archivoImagen.Length > 0)
                        rutaAnterior = await _negocioProducto.ObtenerRutaImagenPorId(oProducto.IdProducto);

                    var (ok, mensajeEditar) = await _negocioProducto.EditarProducto(oProducto);
                    if (!ok)
                        return Json(new { exito = false, mensaje = mensajeEditar });
                }
                else
                {
                    var (idGenerado, mensaje) = await _negocioProducto.RegistrarProducto(oProducto);
                    if (idGenerado == 0)
                        return Json(new { exito = false, mensaje });

                    oProducto.IdProducto = idGenerado;
                }

                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(rutaAnterior))
                    {
                        try
                        {
                            var parts = rutaAnterior.Split('/');
                            var nombreArchivo = parts.Last().Split('.').First();
                            var publicId = $"ByteShop/{nombreArchivo}";
                            await _cloudinary.DeleteResourcesAsync(publicId);
                        }
                        catch (Exception) { }
                    }

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(archivoImagen.FileName, archivoImagen.OpenReadStream()),
                        Folder = "ByteShop",
                        PublicId = $"producto_{oProducto.IdProducto}_{Guid.NewGuid()}"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await _negocioProducto.GuardarRutaImagen(oProducto.IdProducto, uploadResult.SecureUrl.AbsoluteUri);
                    }
                    else
                    {
                        return Json(new { exito = false, mensaje = "Producto guardado, pero la imagen no se pudo subir" });
                    }
                }

                return Json(new
                {
                    exito = true,
                    mensaje = esNuevo ? "Producto registrado correctamente" : "Producto actualizado correctamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }




        // Eliminar producto
        [HttpDelete]
        public async Task<JsonResult> EliminarProducto(int id)
        {
            var (exito, rutaImagen, mensaje) = await _negocioProducto.EliminarProducto(id);

            if (exito && !string.IsNullOrEmpty(rutaImagen))
            {
                try
                {
                    var parts = rutaImagen.Split('/');
                    var nombreArchivo = parts.Last().Split('.').First();
                    var publicId = $"ByteShop/{nombreArchivo}";

                    await _cloudinary.DeleteResourcesAsync(publicId);
                }
                catch { /* puedes loggear si quieres */ }
            }

            return Json(new { exito, mensaje });
        }

        // ===========================
        // Acciones de USUARIOS
        // ===========================

        //Listar usuarios
        [HttpGet]
        public async Task<JsonResult> ListarUsuarios()
        {
            try
            {
                var lista = await _negocioUsuario.ListarUsuarios();
                return Json(new { data = lista }); // <- aquí el "data" es obligatorio
            }
            catch (Exception ex)
            {
                // Para que DataTables no se rompa
                return Json(new { data = new List<Usuario>(), error = ex.Message });
            }
        }

        //Guardar usuario (registrar o editar)
        [HttpPost]
        public async Task<JsonResult> GuardarUsuario([FromBody] Usuario usuario)
        {

            try
            {
                if (usuario.IdUsuario == 0)
                {
                    var (idGenerado, mensaje) = await _negocioUsuario.RegistrarUsuario(usuario);
                    return Json(new { exito = idGenerado > 0, mensaje, id = idGenerado });
                }
                else
                {
                    var (exito, mensaje) = await _negocioUsuario.EditarUsuario(usuario);
                    return Json(new { exito, mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }

        // Eliminar usuario
        [HttpDelete]
        public async Task<JsonResult> EliminarUsuario(int id)
        {
            try
            {
                var (exito, mensaje) = await _negocioUsuario.EliminarUsuario(id);
                return Json(new { exito, mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
        // ===========================
        // ACCIONES DE VENTA
        // ===========================
        // Listar pedidos para administración
        [HttpGet]
        public async Task<JsonResult> ListarPedidos()
        {
            var lista = await _negocioVenta.ListarPedidos();
            return Json(new { data = lista });
        }
        // Marcar pedido como entregado
        [HttpPost]
        public async Task<JsonResult> MarcarPedidoEntregado(int idVenta)
        {
            var (resultado, mensaje) = await _negocioVenta.MarcarPedidoEntregado(idVenta);

            return Json(new
            {
                resultado,
                mensaje
            });
        }



    }
}
