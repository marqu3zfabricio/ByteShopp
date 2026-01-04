using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Usuario
    {
        
        public int IdUsuario { get; set; }
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public required string Correo { get; set; }
        public required string  Clave { get; set; }
        public  bool Reestablecer { get; set; }
        public required bool Activo { get; set; }
        public required string Rol { get; set; }    
    }
}
