namespace Proyecto_Taqueria.Data.Modelos
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; } // Ej: "Alimentos", "Limpieza"
        public int CantidadStock { get; set; }
        public int MinimoStock { get; set; } // Alerta cuando el stock sea menor a este número
        public int MaximoStock { get; set; }
        public DateTime FechaCaducidad { get; set; } // Para alertas
    }
}
