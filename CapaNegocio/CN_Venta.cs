using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Venta(CD_Venta objCapaDatos)
    {
        private readonly CD_Venta _objCapaDatos = objCapaDatos;

        // Listar pedidos para módulo de administración
        public async Task<List<Venta>> ListarPedidos()
        {
            return await _objCapaDatos.ListarPedidos();
        }
        //registrar venta
        public async Task<(bool Resultado, string Mensaje)> RegistrarVenta(
        int totalProducto,
        decimal montoTotal,
        int idDistrito,
        string telefono,
        string direccion,
        string idTransaccion)
        {
            return await _objCapaDatos.RegistrarVenta(
                totalProducto,
                montoTotal,
                idDistrito,
                telefono,
                direccion,
                idTransaccion
            );
        }
        public async Task<List<CarritoFactura>> ListarFacturaPorTransaccion(string idTransaccion)
        {
            return await _objCapaDatos.ListarFacturaPorTransaccion(idTransaccion);
        }
        // Marcar pedido como entregado
        public async Task<(bool Resultado, string Mensaje)> MarcarPedidoEntregado(int idVenta)
        {
            return await _objCapaDatos.MarcarPedidoEntregado(idVenta);
        }

    }

}
