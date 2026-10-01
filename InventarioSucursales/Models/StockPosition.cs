namespace InventarioSucursales.Models
{
    /// <summary>
    /// Representa una posición (fila, columna) de la matriz junto con su cantidad.
    /// Se usa para devolver resultados como "libros con bajo inventario".
    /// Solo contiene datos.
    /// </summary>
    public class StockPosition
    {
        public int BookIndex { get; private set; }     // fila
        public int BranchIndex { get; private set; }   // columna
        public int Quantity { get; private set; }

        // Temporal: O(1) | Espacial: O(1)
        public StockPosition(int bookIndex, int branchIndex, int quantity)
        {
            BookIndex = bookIndex;
            BranchIndex = branchIndex;
            Quantity = quantity;
        }
    }
}
