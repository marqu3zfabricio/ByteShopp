using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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
            // 1. Validación básica de parámetros
            if (!pagoRealizado || string.IsNullOrWhiteSpace(idTransaccion))
            {
                return RedirectToAction("Pago", "Pagos");
            }

            // 2. Consultar la factura en la capa de negocio
            var lista = await _negocioVenta.ListarFacturaPorTransaccion(idTransaccion);

            // 3. Validar si la transacción realmente existe
            if (lista == null || !lista.Any())
            {
                // Si no existe, redirigir a un error o al inicio de pagos
                TempData["Error"] = "La transacción no es válida o no existe.";
                return RedirectToAction("Pago", "Pagos");
            }

            // 4. Si todo está bien, preparar la vista
            ViewBag.IdTransaccion = idTransaccion;
            ViewBag.Mensaje = "Pago registrado correctamente";

            return View(lista);
        }

        //descargar factura pdf
        public async Task<IActionResult> DescargarFacturaPDF(string idTransaccion)
        {
            // 1. Traer la data
            var lista = await _negocioVenta.ListarFacturaPorTransaccion(idTransaccion);
            if (lista == null || !lista.Any()) return NotFound();

            // 2. Configurar la licencia (QuestPDF requiere esta línea para uso comunitario)
            QuestPDF.Settings.License = LicenseType.Community;

            // 3. Crear el documento
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.Header().Text($"Factura: {idTransaccion}").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Producto
                            columns.RelativeColumn(1); // Precio
                            columns.RelativeColumn(1); // Cantidad
                            columns.RelativeColumn(1); // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Producto");
                            header.Cell().Text("Precio");
                            header.Cell().Text("Cant.");
                            header.Cell().Text("Total");
                        });

                        foreach (var item in lista)
                        {
                            table.Cell().Text(item.oProducto.Nombre);
                            table.Cell().Text($"${item.oProducto.Precio}");
                            table.Cell().Text($"{item.Cantidad}");
                            table.Cell().Text($"${item.oProducto.Precio * item.Cantidad}");
                        }
                    });

                    page.Footer().AlignCenter().Text(x => {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            // 4. Convertir a Bytes y enviar al navegador
            byte[] pdfBytes = documento.GeneratePdf();

            // Al usar "inline", el navegador intentará abrirlo en una pestaña nueva en lugar de descargarlo
            return File(pdfBytes, "application/pdf");
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
                string idTransaccion,
                string contacto)
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
                    idTransaccion,
                    contacto
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
