using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Categoria(CD_Categoria objCapaDatos)
    {
        private readonly CD_Categoria _objCapaDatos = objCapaDatos;

        // Listar categorías
        public async Task<List<Categoria>> ListarCategorias()
        {
            return await _objCapaDatos.ListarCategorias();
        }

        // Registrar categoría
        public async Task<(int id, string mensaje)> RegistrarCategoria(Categoria obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Descripcion))
                return (0, "La descripción no puede estar vacía");

            return await _objCapaDatos.RegistrarCategoria(obj);
        }


        // Editar categoría
        public async Task<(bool resultado, string mensaje)> EditarCategoria(Categoria obj)
        {
            if (obj.IdCategoria <= 0)
                throw new ArgumentException("El ID de la categoría no es válido");

            if (string.IsNullOrWhiteSpace(obj.Descripcion))
                return (false, "La descripción no puede ser vacía");

            return await _objCapaDatos.EditarCategoria(obj);
        }

        // Eliminar categoría
        public async Task<(bool exito, string mensaje)> EliminarCategoria(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la categoría no es válido");

            return await _objCapaDatos.EliminarCategoria(id);
        }
    }
}
