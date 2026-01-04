using CapaEntidad;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Distrito(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");
        public async Task<List<Distrito>> ListarDistritos()
        {
            var lista = new List<Distrito>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    @"SELECT d.IdDistrito, d.Nombre, d.IdMunicipio, 
                             m.IdMunicipio AS MunicipioId, m.Nombre AS MunicipioNombre
                      FROM DISTRITO d
                      LEFT JOIN MUNICIPIO m ON d.IdMunicipio = m.IdMunicipio",
                    oconexion
                );
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Distrito
                    {
                        IdDistrito = Convert.ToInt32(dr["IdDistrito"]),
                        Nombre = dr["Nombre"].ToString()!,
                        oMunicipio = dr["IdMunicipio"] != DBNull.Value
                            ? new Municipio
                            {
                                IdMunicipio = Convert.ToInt32(dr["IdMunicipio"]),
                                Nombre = dr["MunicipioNombre"].ToString()!
                            }
                            : null
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar distritos: " + ex.Message);
            }

            return lista;
        }
    }
}
