using System;
using InventarioSucursales.Models;
using InventarioSucursales.Services;

namespace InventarioSucursales.UI
{
    /// <summary>
    /// Interfaz de consola: menú (while + switch) y presentación de resultados.
    /// El usuario trabaja con números base 1 (Libro 1..5, Sucursal 1..3);
    /// aquí se convierten a índices base 0 antes de llamar al InventoryManager.
    /// Notación: n = libros (filas), m = sucursales (columnas).
    /// </summary>
    public class ConsoleMenu
    {
        private readonly InventoryManager inventory;

        // Temporal: O(1) | Espacial: O(1)
        public ConsoleMenu(InventoryManager inventory)
        {
            this.inventory = inventory;
        }

        // Temporal: O(k) según las opciones que elija el usuario | Espacial: O(1)
        public void Run()
        {
            bool running = true;

            while (running)
            {
                ShowMenu();
                string option = Console.ReadLine();
                Console.WriteLine();

                switch (option)
                {
                    case "1": ShowInventoryScreen(); break;
                    case "2": ConsultScreen(); break;
                    case "3": UpdateScreen(); break;
                    case "4": BookTotalScreen(); break;
                    case "5": BranchTotalScreen(); break;
                    case "6": LowStockScreen(); break;
                    case "7": BestBranchScreen(); break;
                    case "0":
                        running = false;
                        Console.WriteLine("Saliendo del sistema. ¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente de nuevo.");
                        break;
                }

                if (running) InputReader.Pause();
            }
        }

        // Temporal: O(1) | Espacial: O(1)
        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("====== INVENTARIO POR SUCURSAL ======");
            Console.WriteLine();
            Console.WriteLine("1. Mostrar inventario completo");
            Console.WriteLine("2. Consultar disponibilidad");
            Console.WriteLine("3. Actualizar disponibilidad");
            Console.WriteLine("4. Total disponible de un libro");
            Console.WriteLine("5. Total de inventario por sucursal");
            Console.WriteLine("6. Mostrar libros con bajo inventario");
            Console.WriteLine("7. Sucursal con mayor disponibilidad");
            Console.WriteLine("0. Salir");
            Console.WriteLine();
            Console.Write("Seleccione una opción: ");
        }

        // ------------------------- Pantallas -------------------------

        /// <summary>1. Recorre todas las filas y columnas (dos ciclos anidados).</summary>
        // Temporal: O(n * m) | Espacial: O(1)
        private void ShowInventoryScreen()
        {
            Console.Write(string.Format("{0,-4}{1,-30}", "#", "Libro"));
            for (int j = 0; j < inventory.Columns; j++)
            {
                Console.Write(string.Format("{0,14}", inventory.GetBranchName(j)));
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', 34 + 14 * inventory.Columns));

            for (int i = 0; i < inventory.Rows; i++)                 // ciclo externo: filas (libros)
            {
                Console.Write(string.Format("{0,-4}{1,-30}", i + 1, Truncate(inventory.GetBookTitle(i), 29)));

                for (int j = 0; j < inventory.Columns; j++)          // ciclo interno: columnas (sucursales)
                {
                    Console.Write(string.Format("{0,14}", inventory.GetStock(i, j)));
                }
                Console.WriteLine();
            }
        }

        /// <summary>2. Consulta directa de stock[fila, columna].</summary>
        // Temporal: O(1) para la consulta (mostrar las listas de referencia es O(n + m)) | Espacial: O(1)
        private void ConsultScreen()
        {
            int book = ReadBook();
            int branch = ReadBranch();

            int quantity = inventory.GetStock(book, branch);
            Console.WriteLine();
            Console.WriteLine("'" + inventory.GetBookTitle(book) + "' en la sucursal "
                              + inventory.GetBranchName(branch) + ": " + quantity + " unidades.");
            Console.WriteLine("(Acceso directo a stock[" + book + "," + branch + "] -> O(1))");
        }

        /// <summary>3. Modifica la cantidad de una posición.</summary>
        // Temporal: O(1) | Espacial: O(1)
        private void UpdateScreen()
        {
            int book = ReadBook();
            int branch = ReadBranch();

            Console.WriteLine();
            Console.WriteLine("Cantidad actual: " + inventory.GetStock(book, branch));
            int newQuantity = InputReader.ReadInt("Nueva cantidad: ");

            bool ok = inventory.UpdateStock(book, branch, newQuantity);
            Console.WriteLine(ok
                ? "Disponibilidad actualizada correctamente."
                : "La cantidad no puede ser negativa. No se realizaron cambios.");
        }

        /// <summary>4. Suma una fila completa.</summary>
        // Temporal: O(m) | Espacial: O(1)
        private void BookTotalScreen()
        {
            int book = ReadBook();
            int total = inventory.GetBookTotal(book);

            Console.WriteLine();
            Console.WriteLine("Total disponible de '" + inventory.GetBookTitle(book)
                              + "' en todas las sucursales: " + total + " unidades.");
        }

        /// <summary>5. Suma una columna completa.</summary>
        // Temporal: O(n) | Espacial: O(1)
        private void BranchTotalScreen()
        {
            int branch = ReadBranch();
            int total = inventory.GetBranchTotal(branch);

            Console.WriteLine();
            Console.WriteLine("Inventario total de la sucursal " + inventory.GetBranchName(branch)
                              + ": " + total + " unidades.");
        }

        /// <summary>6. Lista las posiciones con cantidad menor o igual al umbral.</summary>
        // Temporal: O(n * m) | Espacial: O(k), k = posiciones encontradas
        private void LowStockScreen()
        {
            StockPosition[] low = inventory.FindLowStock(InventoryManager.LowStockThreshold);

            Console.WriteLine("Posiciones con cantidad <= " + InventoryManager.LowStockThreshold + ":");
            Console.WriteLine();

            if (low.Length == 0)
            {
                Console.WriteLine("No hay libros con bajo inventario.");
                return;
            }

            for (int k = 0; k < low.Length; k++)
            {
                StockPosition p = low[k];
                Console.WriteLine(string.Format("- {0} | Sucursal {1} | {2} unidades  (stock[{3},{4}])",
                    inventory.GetBookTitle(p.BookIndex),
                    inventory.GetBranchName(p.BranchIndex),
                    p.Quantity, p.BookIndex, p.BranchIndex));
            }
            Console.WriteLine();
            Console.WriteLine("Total de posiciones con bajo inventario: " + low.Length);
        }

        /// <summary>7. Compara las sucursales de un libro y muestra la de mayor cantidad.</summary>
        // Temporal: O(m) | Espacial: O(t), t = sucursales empatadas
        private void BestBranchScreen()
        {
            int book = ReadBook();
            int maxQuantity;
            int[] best = inventory.GetBestBranches(book, out maxQuantity);

            Console.WriteLine();
            if (best.Length == 1)
            {
                Console.WriteLine("La sucursal con mayor disponibilidad de '" + inventory.GetBookTitle(book)
                                  + "' es " + inventory.GetBranchName(best[0]) + " con " + maxQuantity + " unidades.");
            }
            else
            {
                Console.WriteLine("Hay empate para '" + inventory.GetBookTitle(book)
                                  + "' con " + maxQuantity + " unidades en:");
                for (int k = 0; k < best.Length; k++)
                {
                    Console.WriteLine("- " + inventory.GetBranchName(best[k]));
                }
            }
        }

        // ------------------- Lectura de libro y sucursal -------------------

        /// <summary>Muestra los libros disponibles, pide el número (1..n) y devuelve el índice base 0.</summary>
        // Temporal: O(n) | Espacial: O(1)
        private int ReadBook()
        {
            Console.WriteLine("Libros:");
            for (int i = 0; i < inventory.Rows; i++)
            {
                Console.WriteLine("  " + (i + 1) + ". " + inventory.GetBookTitle(i));
            }
            int number = InputReader.ReadIntInRange("Número de libro (1-" + inventory.Rows + "): ", 1, inventory.Rows);
            return number - 1;
        }

        /// <summary>Muestra las sucursales, pide el número (1..m) y devuelve el índice base 0.</summary>
        // Temporal: O(m) | Espacial: O(1)
        private int ReadBranch()
        {
            Console.WriteLine("Sucursales:");
            for (int j = 0; j < inventory.Columns; j++)
            {
                Console.WriteLine("  " + (j + 1) + ". " + inventory.GetBranchName(j));
            }
            int number = InputReader.ReadIntInRange("Número de sucursal (1-" + inventory.Columns + "): ", 1, inventory.Columns);
            return number - 1;
        }

        // Temporal: O(1) (longitud acotada) | Espacial: O(1)
        private static string Truncate(string text, int max)
        {
            if (text == null) return string.Empty;
            return text.Length <= max ? text : text.Substring(0, max);
        }
    }
}