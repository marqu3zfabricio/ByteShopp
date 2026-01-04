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
    public class CD_Municipio(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");
        //listar municipios
        public async Task<List<Municipio>> ListarMunicipios()
        {
            var lista = new List<Municipio>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    @"SELECT d.IdMunicipio, d.Nombre, d.IdDepartamento, 
                             m.IdDepartamento AS DepartamentoId, m.Nombre AS DepartamentoNombre
                      FROM Municipio d
                      LEFT JOIN Departamento m ON d.IdDepartamento = m.IdDepartamento",
                    oconexion
                );
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Municipio
                    {
                        IdMunicipio = Convert.ToInt32(dr["IdMunicipio"]),
                        Nombre = dr["Nombre"].ToString()!,
                        oDepartamento = dr["IdDepartamento"] != DBNull.Value
                            ? new Departamento
                            {
                                IdDepartamento = Convert.ToInt32(dr["IdDepartamento"]),
                                Nombre = dr["DepartamentoNombre"].ToString()!
                            }
                            : null
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar municipios: " + ex.Message);
            }

            return lista;
        }
    }
}
