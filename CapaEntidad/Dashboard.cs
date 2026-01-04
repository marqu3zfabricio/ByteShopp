using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class DashBoard
    {
        public int TotalCliente { get; set; }
        public int TotalVenta { get; set; }
        public int TotalProducto { get; set; }
        public decimal TotalIngresosHoy { get; set; }
        public int PedidosPendientes { get; set; }
        public int PedidosHoy { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalIngresosMesActual { get; set; }
        public List<IngresoDiario> IngresosDiarios { get; set; } = [];
        public List<IngresoMensual> IngresosMensuales { get; set; } = [];
    }
}
