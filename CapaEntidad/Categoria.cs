using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class Categoria

    {
        
        public int IdCategoria { get; set; }
        public required string Descripcion { get; set; }
        public bool Activo { get; set; }

    }
}
