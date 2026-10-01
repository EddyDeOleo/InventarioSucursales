using System;

namespace InventarioSucursales.UI
{
    /// <summary>
    /// Utilidades de lectura y validación de datos desde la consola.
    /// </summary>
    public static class InputReader
    {
        // Temporal: O(k), k = intentos del usuario | Espacial: O(1)
        public static int ReadInt(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Valor numérico inválido. " + prompt);
            }
            return value;
        }

        // Temporal: O(k) | Espacial: O(1). Repite hasta que el valor esté en [min, max].
        public static int ReadIntInRange(string prompt, int min, int max)
        {
            int value = ReadInt(prompt);
            while (value < min || value > max)
            {
                Console.WriteLine("El valor debe estar entre " + min + " y " + max + ".");
                value = ReadInt(prompt);
            }
            return value;
        }

        // Temporal: O(1) | Espacial: O(1)
        public static void Pause()
        {
            Console.WriteLine("\nPresione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}