using CapaDatos;
using System;
using CapaEntidad;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Departamento(CD_Departamento objCapaDatos)
    {
        private readonly CD_Departamento _objCapaDatos = objCapaDatos;
        // Listar departamentos
        public async Task<List<Departamento>> ListarDepartamentos()
        {
            return await _objCapaDatos.ListarDepartamentos();
        }
    }
}
