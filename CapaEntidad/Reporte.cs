using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Reporte
    {
        public required string FechaVenta { get; set; }
        
        public required string Producto { get; set; }
        public required decimal Precio { get; set; }
        public required int Cantidad { get; set; }
        public required decimal Total { get; set; }
        public required string IdTransaccion { get; set; }
    }
}
