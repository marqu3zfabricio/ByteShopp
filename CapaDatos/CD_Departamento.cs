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
    public class CD_Departamento(MiContexto context)
    {
        private readonly MiContexto _context = context;
        private readonly string _connectionString = context.Database.GetConnectionString() ?? throw new Exception("Cadena de conexión no encontrada.");
        public async Task<List<Departamento>> ListarDepartamentos()
        {
            var lista = new List<Departamento>();

            try
            {
                using var oconexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    @"SELECT * FROM DEPARTAMENTO",
                    oconexion
                );
                cmd.CommandType = CommandType.Text;
                await oconexion.OpenAsync();

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Departamento
                    {
                        IdDepartamento = Convert.ToInt32(dr["IdDepartamento"]),
                        Nombre = dr["Nombre"].ToString()!,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar Departamentos: " + ex.Message);
            }

            return lista;
        }
    }
}
