using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Marca(CD_Marca objCapaDatos)
    {
        private readonly CD_Marca _objCapaDatos = objCapaDatos;

        public async Task<List<Marca>> ListarMarcas()
        {
            return await _objCapaDatos.ListarMarcas();
        }

        // Registrar marca
        public async Task<(int id, string mensaje)> RegistrarMarca(Marca obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Descripcion))
                return (0, "La descripción no puede estar vacía");

            return await _objCapaDatos.RegistrarMarca(obj);
        }


        public async Task<(bool resultado, string mensaje)> EditarMarca(Marca obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Descripcion))
                return (false, "La descripción de la marca no puede estar vacía");

            return await _objCapaDatos.EditarMarca(obj);
        }


        public async Task<(bool exito, string mensaje)> EliminarMarca(int id)
        {
            return await _objCapaDatos.EliminarMarca(id);
        }
        //filtro marcas por categoria 
        public async Task<List<Marca>> ListarMarcasPorCategoria(int idCategoria)
        {
            if (idCategoria <= 0)
                throw new ArgumentException("El ID de la categoría no es válido");

            return await _objCapaDatos.ListarMarcasPorCategoria(idCategoria);
        }

    }
}
