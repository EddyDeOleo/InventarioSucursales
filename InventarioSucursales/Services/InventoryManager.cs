using System;
using InventarioSucursales.Models;

namespace InventarioSucursales.Services
{
    /// <summary>
    /// Administra el inventario en una matriz int[,] stock:
    ///   filas    (índice i) = libros
    ///   columnas (índice j) = sucursales
    ///   stock[i, j]         = cantidad disponible del libro i en la sucursal j
    ///
    /// Notación Big O usada en los comentarios:
    ///   n = número de filas (libros) | m = número de columnas (sucursales)
    ///
    /// Esta clase NO usa Console: recibe índices base 0 y devuelve datos.
    /// Los índices base 1 que ve el usuario se convierten en la capa de interfaz.
    /// </summary>
    public class InventoryManager
    {
        public const int LowStockThreshold = 3;

        private readonly string[] bookTitles;
        private readonly string[] branchNames;
        private readonly int[,] stock;

        public int Rows
        {
            get { return stock.GetLength(0); }
        }

        public int Columns
        {
            get { return stock.GetLength(1); }
        }

        // Temporal: O(n * m) al copiar los datos iniciales (tamaño constante 5 x 3 = O(1) en la práctica)
        // Espacial: O(n * m) por la matriz
        public InventoryManager()
        {
            bookTitles = new string[]
            {
                "Clean Code",
                "Design Patterns",
                "Introduction to Algorithms",
                "Cien años de soledad",
                "Don Quijote de la Mancha"
            };

            branchNames = new string[]
            {
                "Centro",
                "Norte",
                "Sur"
            };

            // 5 libros (filas) x 3 sucursales (columnas)
            stock = new int[,]
            {
                //  Centro Norte Sur
                {   5,     2,    8 },   // Clean Code
                {   3,     7,    1 },   // Design Patterns
                {   4,     6,    2 },   // Introduction to Algorithms
                {   9,     3,    5 },   // Cien años de soledad
                {   1,     4,    6 }    // Don Quijote de la Mancha
            };
        }

        // ------------------- Nombres y validación -------------------

        // Temporal: O(1) | Espacial: O(1)
        public string GetBookTitle(int bookIndex)
        {
            CheckBook(bookIndex);
            return bookTitles[bookIndex];
        }

        // Temporal: O(1) | Espacial: O(1)
        public string GetBranchName(int branchIndex)
        {
            CheckBranch(branchIndex);
            return branchNames[branchIndex];
        }

        // Temporal: O(1) | Espacial: O(1)
        private void CheckBook(int bookIndex)
        {
            if (bookIndex < 0 || bookIndex >= Rows)
                throw new ArgumentOutOfRangeException("bookIndex");
        }

        // Temporal: O(1) | Espacial: O(1)
        private void CheckBranch(int branchIndex)
        {
            if (branchIndex < 0 || branchIndex >= Columns)
                throw new ArgumentOutOfRangeException("branchIndex");
        }

        // ------------------- 2. CONSULTAR -------------------

        /// <summary>Acceso directo por índices a la posición stock[fila, columna].</summary>
        // Temporal: O(1) (acceso directo, sin recorrer) | Espacial: O(1)
        public int GetStock(int bookIndex, int branchIndex)
        {
            CheckBook(bookIndex);
            CheckBranch(branchIndex);
            return stock[bookIndex, branchIndex];
        }

        // ------------------- 3. ACTUALIZAR -------------------

        /// <summary>Modifica la cantidad de una posición. Rechaza cantidades negativas.</summary>
        // Temporal: O(1) (acceso directo) | Espacial: O(1)
        public bool UpdateStock(int bookIndex, int branchIndex, int newQuantity)
        {
            CheckBook(bookIndex);
            CheckBranch(branchIndex);
            if (newQuantity < 0) return false;

            stock[bookIndex, branchIndex] = newQuantity;
            return true;
        }

        // ------------------- 4. TOTAL DE UN LIBRO -------------------

        /// <summary>Suma una fila completa: recorre todas las sucursales del libro.</summary>
        // Temporal: O(m) (un solo ciclo sobre las columnas) | Espacial: O(1)
        public int GetBookTotal(int bookIndex)
        {
            CheckBook(bookIndex);

            int total = 0;
            for (int j = 0; j < Columns; j++)
            {
                total += stock[bookIndex, j];
            }
            return total;
        }

        // ------------------- 5. INVENTARIO DE UNA SUCURSAL -------------------

        /// <summary>Suma una columna completa: recorre todos los libros de la sucursal.</summary>
        // Temporal: O(n) (un solo ciclo sobre las filas) | Espacial: O(1)
        public int GetBranchTotal(int branchIndex)
        {
            CheckBranch(branchIndex);

            int total = 0;
            for (int i = 0; i < Rows; i++)
            {
                total += stock[i, branchIndex];
            }
            return total;
        }

        // ------------------- 6. BAJO INVENTARIO -------------------

        /// <summary>
        /// Recorre toda la matriz (dos ciclos anidados) y devuelve las posiciones con
        /// cantidad menor o igual al umbral. Primera pasada: contar; segunda: llenar
        /// un arreglo del tamaño exacto.
        /// </summary>
        // Temporal: O(n * m) (dos pasadas, cada una O(n * m)) | Espacial: O(k), k = posiciones encontradas (k <= n * m)
        public StockPosition[] FindLowStock(int threshold)
        {
            int count = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (stock[i, j] <= threshold) count++;
                }
            }

            StockPosition[] result = new StockPosition[count];
            int k = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (stock[i, j] <= threshold)
                    {
                        result[k] = new StockPosition(i, j, stock[i, j]);
                        k++;
                    }
                }
            }
            return result;
        }

        // ------------------- 7. SUCURSAL CON MAYOR DISPONIBILIDAD -------------------

        /// <summary>
        /// Compara las sucursales de un libro y devuelve los índices de la(s) sucursal(es)
        /// con la mayor cantidad (puede haber empate). maxQuantity devuelve ese máximo.
        /// </summary>
        // Temporal: O(m) (dos pasadas sobre una fila) | Espacial: O(t), t = sucursales empatadas (t <= m)
        public int[] GetBestBranches(int bookIndex, out int maxQuantity)
        {
            CheckBook(bookIndex);

            maxQuantity = stock[bookIndex, 0];
            for (int j = 1; j < Columns; j++)
            {
                if (stock[bookIndex, j] > maxQuantity) maxQuantity = stock[bookIndex, j];
            }

            int ties = 0;
            for (int j = 0; j < Columns; j++)
            {
                if (stock[bookIndex, j] == maxQuantity) ties++;
            }

            int[] result = new int[ties];
            int k = 0;
            for (int j = 0; j < Columns; j++)
            {
                if (stock[bookIndex, j] == maxQuantity)
                {
                    result[k] = j;
                    k++;
                }
            }
            return result;
        }
    }
}