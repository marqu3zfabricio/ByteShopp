using CapaEntidad;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class MiContexto(DbContextOptions<MiContexto> options) : DbContext(options)
    {
    }
}
