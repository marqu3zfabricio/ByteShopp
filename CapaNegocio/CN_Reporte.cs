using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte(CD_Reporte objCapaDatos)
    {
        private readonly CD_Reporte _objCapaDatos = objCapaDatos ?? throw new ArgumentNullException(nameof(objCapaDatos));

        // ============================
        // OBTENER DASHBOARD
        // ============================
        public async Task<DashBoard> VerDashBoard()
        {
            return await _objCapaDatos.VerDashBoard();
        }

        
    }
}
