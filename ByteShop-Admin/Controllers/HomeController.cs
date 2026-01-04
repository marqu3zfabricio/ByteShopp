using ByteShop_Admin.Models;
using CapaNegocio;
using CapaEntidad;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ByteShop_Admin.Helpers;

namespace ByteShop_Admin.Controllers
{
    
    public class HomeController(ILogger<HomeController> logger, CN_Reporte negocioReporte) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly CN_Reporte _negocioReporte = negocioReporte;

        // ============================
        // VISTA PRINCIPAL
        // ============================
        public IActionResult Index()
        {
            return View();
        }





        // ============================
        // OBTENER DASHBOARD COMO JSON
        // ============================
        [HttpGet]
        public async Task<JsonResult> VistaDashBoard()
        {

            try
            {
                DashBoard objeto = await _negocioReporte.VerDashBoard();
                return Json(new { data = objeto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard");
                return Json(new { data = new DashBoard(), mensaje = ex.Message });
            }
        }

        

        // ============================
        // ERROR
        // ============================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        

    }
}
