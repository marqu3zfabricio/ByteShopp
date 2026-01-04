using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Municipio
    {
        public int IdMunicipio { get; set; }
        public required string Nombre { get; set; }
        public  Departamento? oDepartamento { get; set; }
    }
}
