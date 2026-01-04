using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio 
{
    
    public class CN_Carrito(CD_Carrito objCapaDatos)
    {
        private readonly CD_Carrito _objCapaDatos = objCapaDatos;
         
        //Listar Productos del carrito 
        public async Task<List<Carrito>> ListarCarrito()
        {
            return await _objCapaDatos.ListarCarrito();
        }
        //operacion de carrito
        public async Task<(bool Resultado, string Mensaje)> OperacionCarrito(int idProducto, bool sumar)
        {
            return await _objCapaDatos.OperacionCarrito(idProducto, sumar);
        }
        // Verificar si un producto ya existe en el carrito
        public async Task<bool> ExisteCarrito(int idProducto)
        {
            return await _objCapaDatos.ExisteCarrito(idProducto);
        }
        // Eliminar producto del carrito
        public async Task<bool> EliminarCarrito(int idProducto)
        {
            return await _objCapaDatos.EliminarCarrito(idProducto);
        }


    }
}
