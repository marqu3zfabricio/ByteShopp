using CapaEntidad;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;


namespace CapaDatos
{
    public class CD_Reporte(MiContexto context)
    {
        private readonly string _connectionString = context.Database.GetConnectionString()
                ?? throw new Exception("Cadena de conexión no encontrada.");

        // ============================
        // OBTENER DASHBOARD
        // ============================
        public async Task<DashBoard> VerDashBoard()
        {
            var objeto = new DashBoard();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_ReportesDashboard", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                await oconexion.OpenAsync();
                using var dr = await cmd.ExecuteReaderAsync();

                /* ========== 1. KPIs ========== */
                if (await dr.ReadAsync())
                {
                    objeto.TotalCliente = Convert.ToInt32(dr["TotalCliente"]);
                    objeto.TotalVenta = Convert.ToInt32(dr["TotalVenta"]);
                    objeto.TotalProducto = Convert.ToInt32(dr["TotalProducto"]);
                    objeto.TotalIngresosHoy = Convert.ToDecimal(dr["TotalIngresosHoy"]);
                    objeto.PedidosPendientes = Convert.ToInt32(dr["PedidosPendientes"]);
                    objeto.PedidosHoy = Convert.ToInt32(dr["PedidosHoy"]);
                    objeto.TotalIngresos = Convert.ToDecimal(dr["TotalIngresos"]);
                    objeto.TotalIngresosMesActual = Convert.ToDecimal(dr["TotalIngresosMesActual"]);
                }

                /* ========== 2. HISTÓRICO DIARIO ========== */
                if (await dr.NextResultAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        objeto.IngresosDiarios.Add(new IngresoDiario
                        {
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }

                /* ========== 3. HISTÓRICO MENSUAL ========== */
                if (await dr.NextResultAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        objeto.IngresosMensuales.Add(new IngresoMensual
                        {
                            Mes = dr["Mes"].ToString(),
                            MesKey = dr["MesKey"].ToString(),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }
                
            }
            catch
            {
                objeto = new DashBoard();
            }

            return objeto;
        }



    }
}
