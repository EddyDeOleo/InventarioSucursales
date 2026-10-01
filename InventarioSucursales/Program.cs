using InventarioSucursales.Services;
using InventarioSucursales.UI;

namespace InventarioSucursales
{
    /// <summary>
    /// Punto de entrada. Solo ensambla los objetos y arranca la aplicación.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            InventoryManager inventory = new InventoryManager();
            ConsoleMenu menu = new ConsoleMenu(inventory);
            menu.Run();
        }
    }
}
