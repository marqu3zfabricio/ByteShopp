using ByteShop_Tienda.Models;
using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;


namespace ByteShop_Tienda.Controllers
{
    public class ByteShopTiendaController (
        CN_Marca negocioMarca,
        CN_Categoria negocioCategoria,
        CN_Producto negocioProducto,
        CN_Carrito negocioCarrito) : Controller

    {
        private readonly CN_Marca _negocioMarca = negocioMarca;
        private readonly CN_Categoria _negocioCategoria = negocioCategoria;
        private readonly CN_Producto _negocioProducto = negocioProducto;
        private readonly CN_Carrito _negocioCarrito = negocioCarrito;

        public IActionResult Index()
        {
            return View();
        }

        //======================//
        //VISTA DE PRODUCTOS
        //======================//
        public async Task<IActionResult> ProductDetails(int id)
        {
            var lista = await _negocioProducto.ListarProductosActivos();

            var producto = lista.FirstOrDefault(p => p.IdProducto == id);

            if (producto == null)
                return RedirectToAction("Index", "ByteShopTienda");

            return View(producto);
        }


        //======================//
        //OPRECIONES DE CARRITO
        //======================//
        //Listar Carrito
        public async Task<IActionResult> Carrito()
        {
            try
            {
                var culture = new CultureInfo("es-SV");

                var lista = await _negocioCarrito.ListarCarrito();

                foreach (var item in lista)
                {
                    item.oProducto.Precio = decimal.Parse(
                        item.oProducto.Precio.ToString("0", culture),
                        culture
                    );
                }

                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<Carrito>());
            }
        }
        //contador de carrito para el icono 
        [HttpGet]
        public async Task<JsonResult> ContadorCarrito()
        {
            var lista = await _negocioCarrito.ListarCarrito();

            int totalItems = lista.Sum(x => x.Cantidad);

            return Json(new { total = totalItems });
        }

        //opreacion de carrito para agregar productos, sumar y restar stock
        [HttpPost]
        public async Task<JsonResult> OperacionCarrito(int idProducto, bool sumar, string origen)
        {
            try
            {
                // Validación exclusiva para INDEX
                if (origen == "index" || origen=="details" && sumar)
                {
                    bool existe = await _negocioCarrito.ExisteCarrito(idProducto);
                    if (existe)
                    {
                        return Json(new
                        {
                            resultado = false,
                            mensaje = "El producto ya se encuentra en el carrito"
                        });
                    }
                }

                var (Resultado, Mensaje) =
                    await _negocioCarrito.OperacionCarrito(idProducto, sumar);

                return Json(new
                {
                    resultado = Resultado,
                    mensaje = Mensaje
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    resultado = false,
                    mensaje = ex.Message
                });
            }
        }


        // Eliminar producto del carrito
        [HttpPost]
        public async Task<JsonResult> EliminarCarrito(int idProducto)
        {
            try
            {
                bool resultado = await _negocioCarrito.EliminarCarrito(idProducto);

                return Json(new
                {
                    resultado,
                    mensaje = resultado
                        ? "Producto eliminado del carrito"
                        : "No se pudo eliminar el producto del carrito"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    resultado = false,
                    mensaje = ex.Message
                });
            }
        }




        //======================//
        //OPRECIONES DE CATEGORIAS
        //======================//

        //listar Categorias
        [HttpGet]
        public async Task<JsonResult> ListarCategorias()
        {
            var lista = await _negocioCategoria.ListarCategorias();
            return Json(new { data = lista });
        }

        //======================//
        //OPRECIONES DE MARCAS
        //======================//

        // Listar marcas (todas o filtradas por categoría)
        [HttpGet]
        public async Task<JsonResult> ListarMarcas(int? idCategoria)
        {
            List<Marca> lista;

            if (idCategoria.HasValue && idCategoria > 0)
            {
                lista = await _negocioMarca.ListarMarcasPorCategoria(idCategoria.Value);
            }
            else
            {
                lista = await _negocioMarca.ListarMarcas();
            }

            return Json(new { data = lista });
        }

        //======================//
        //OPRECIONES DE PRODUCTOS
        //======================//

        //listar Productos   
        [HttpGet]
        public async Task<JsonResult> ListarProductos(int? idCategoria, int? idMarca)
        {
            List<Producto> lista;

            // Categoría + Marca
            if (idCategoria.HasValue && idCategoria > 0 &&
                idMarca.HasValue && idMarca > 0)
            {
                lista = await _negocioProducto
                    .FiltrarProductosPorCategoriaMarca(idCategoria.Value, idMarca.Value);
            }
            // Solo Categoría
            else if (idCategoria.HasValue && idCategoria > 0)
            {
                lista = await _negocioProducto
                    .FiltrarProductosPorCategoria(idCategoria.Value);
            }
            // Solo Marca (todas las categorías)
            else if (idMarca.HasValue && idMarca > 0)
            {
                lista = await _negocioProducto
                    .FiltrarProductosPorMarca(idMarca.Value);
            }
            // Sin filtros
            else
            {
                lista = await _negocioProducto.ListarProductosActivos();
            }

            return Json(new { data = lista });
        }


        //======================//
        //VISTA DE ERROR
        //======================//

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
