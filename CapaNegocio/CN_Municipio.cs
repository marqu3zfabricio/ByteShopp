using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Municipio(CD_Municipio objCapaDatos)
    {
        private readonly CD_Municipio _objCapaDatos = objCapaDatos;
        // Listar municipios
        public async Task<List<Municipio>> ListarMunicipios()
        {
            return await _objCapaDatos.ListarMunicipios();
        }
    }
}
