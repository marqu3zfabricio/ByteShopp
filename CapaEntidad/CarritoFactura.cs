using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CarritoFactura
    {
        public int Cantidad { get; set; }
#pragma warning disable IDE1006 // Estilos de nombres
        public ProductoFactura oProducto { get; set; } = new ();
#pragma warning restore IDE1006 // Estilos de nombres
    }
}
