using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public required int IdVenta { get; set; }
        
        public int TotalProducto { get; set; }
        public decimal MontoTotal { get; set; }

        public Distrito oDistrito { get; set; } = null!;
        public required string Telefono { get; set; }
        public required string Direccion { get; set; }
        public required string FechaTexto { get; set; }
        public required string IdTransaccion { get; set; }
        public required string Estado { get; set; }
        public required List<DetalleVenta> oDetalleVenta { get; set; }
    }
}
