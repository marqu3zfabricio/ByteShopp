using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ByteShop_Tienda.Controllers
{
    public class PagosController(
        CN_Carrito negocioCarrito,
        CN_Distrito negocioDistrito,
        CN_Municipio negocioMunicipio,
        CN_Venta negocioVenta,
        CN_Departamento negocioDepartamento) : Controller
    {
        private readonly CN_Carrito _negocioCarrito = negocioCarrito;
        private readonly CN_Distrito _negocioDistrito = negocioDistrito;    
        private readonly CN_Municipio _negocioMunicipio = negocioMunicipio; 
        private readonly CN_Departamento _negocioDepartamento = negocioDepartamento;
        private readonly CN_Venta _negocioVenta = negocioVenta;
        public async Task<IActionResult> Pago()
        {
            try
            {
                var culture = new CultureInfo("es-SV");

                
                List<Carrito> lista = await _negocioCarrito.ListarCarrito();

                
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
        public async Task<IActionResult> PagoEfectivo(bool pagoRealizado = false, string idTransaccion = "")
        {
            if (!pagoRealizado || string.IsNullOrEmpty(idTransaccion))
                return RedirectToAction("Pago", "Pagos");

            var lista = await _negocioVenta.ListarFacturaPorTransaccion(idTransaccion);

            ViewBag.IdTransaccion = idTransaccion;
            ViewBag.Mensaje = "Pago registrado correctamente";

            return View(lista);
        }





        //listar distritos
        public async Task<JsonResult> ListarDistritos()
        {
            try
            {
                List<Distrito> lista = await _negocioDistrito.ListarDistritos();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<Distrito>(), mensaje = ex.Message });
            }
        }
        //listar municipios
        public async Task<JsonResult> ListarMunicipios()
        {
            try
            {
                List<Municipio> lista = await _negocioMunicipio.ListarMunicipios();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<Municipio>(), mensaje = ex.Message });
            }
        }
        //listar departamentos  
        public async Task<JsonResult> ListarDepartamentos()
        {
            try
            {
                List<Departamento> lista = await _negocioDepartamento.ListarDepartamentos();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<Departamento>(), mensaje = ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> RegistrarVenta(
                int totalProducto,
                decimal montoTotal,
                int idDistrito,
                string telefono,
                string direccion,
                string idTransaccion)
        {
            try
            {
                // Suponiendo que tienes CN_Venta instanciado como _negocioVenta
                var (resultado, mensaje) = await _negocioVenta.RegistrarVenta(
                    totalProducto,
                    montoTotal,
                    idDistrito,
                    telefono,
                    direccion,
                    idTransaccion
                );

                return Json(new { resultado, mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

    }
}
