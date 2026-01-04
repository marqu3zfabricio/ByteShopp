using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CapaNegocio
{
    public class CN_Distrito(CD_Distrito objCapaDatos)
    {
        private readonly CD_Distrito _objCapaDatos = objCapaDatos;

        // Listar distritos
        public async Task<List<Distrito>> ListarDistritos()
        {
            return await _objCapaDatos.ListarDistritos();
        }
    }
}
