using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ProductoFactura
    {
        public string Nombre { get; set; } = string.Empty;  
        public decimal Precio { get; set; }
        public string RutaImagen { get; set; } = string.Empty;  
    }
}
