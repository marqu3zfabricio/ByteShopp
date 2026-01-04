
namespace CapaEntidad
{
    public class Producto
    {
        
        public int IdProducto { get; set; }

        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }

       

       

        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }

        
        public required Marca oMarca { get; set; }

        
        public required Categoria oCategoria { get; set; }

        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public required string RutaImagen { get; set; }
        
        public required bool Activo { get; set; }
    }
}
