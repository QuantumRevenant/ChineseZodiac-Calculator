internal class Program
{
    private static void Main(string[] args)
    {
        string str_option;
        int option;
        string continuar;

        do
        {
            str_option = string.Empty;
            option = 0;
            continuar = string.Empty;

            do
            {
                Console.WriteLine("¿Qué deseas hacer?");
                Console.WriteLine("1. Calcular una fecha única");
                Console.WriteLine("2. Mostrar un rango completo en consola");
                Console.WriteLine("3. Generar Excel con un rango completo (limitado del año 1900 a 2100)");
                Console.WriteLine("4. Salir");
                Console.WriteLine();
                str_option = Console.ReadLine() ?? string.Empty;
                Console.WriteLine();
            } while (int.TryParse(str_option, out int selection) && selection < 0 && selection > 5);

            option = int.Parse(str_option);

            if (option == 4) Exit();

            Console.WriteLine($"Escribe el{((option != 1) ? " primer" : "")} año:");

            string? str_Year1 = Console.ReadLine();
            string? str_Year2 = str_Year1;

            Console.WriteLine();

            if (option != 1)
            {
                Console.WriteLine($"Escribe el segundo año:");
                str_Year2 = Console.ReadLine();
                Console.WriteLine();
            }

            if (int.TryParse(str_Year1, out int year1) && int.TryParse(str_Year2, out int year2))
            {
                switch (option)
                {
                    case 1:
                        DoSingleConsoleQuery(year1);
                        break;
                    case 2:
                        DoRangeConsoleQuery(year1, year2);
                        break;
                    case 3:
                        DoRangeExcelQuery(year1, year2);
                        break;
                    default:
                        Console.WriteLine("No deberías de estar aquí. Este es un mensaje de error.\nContacta al administrador.\n\n Código de Error: #OPTMEN421");
                        Exit();
                        break;
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("El valor introducido debe ser numérico");

            // Preguntar si desea continuar
            Console.WriteLine("¿Deseas continuar? (Y/S para sí, cualquier otra tecla para salir)");
            continuar = (Console.ReadLine() ?? string.Empty).ToLower();
            Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
        } while (continuar is "y" or "s");

        Exit();
    }

    private static void DoSingleConsoleQuery(int year)
    {
        DoRangeConsoleQuery(year - 1, year + 1);
    }
    private static void DoRangeConsoleQuery(int year1, int year2)
    {
        PrintTitle();

        for (int year = year1; year <= year2; year++)
        {
            var output = new NewChineseYear(year);
            if (year > 2100 || year <= 1900)
                continue;

            PrintValue(output);
        }
    }
    private static void DoRangeExcelQuery(int year1, int year2)
    {
        Console.WriteLine("Introduce el nombre del archivo Excel (sin extensión):");
        string? fileName = Console.ReadLine();
        string filePath = $"./{fileName}.xlsx";

        // Advertir sobre la sobreescritura si el archivo ya existe
        if (File.Exists(filePath))
        {
            Console.WriteLine($"Advertencia: El archivo '{fileName}.xlsx' ya existe. ¿Deseas sobreescribirlo? (Y/S para sí, cualquier otra tecla para cancelar)");
            string sobreescribir = (Console.ReadLine() ?? string.Empty).ToLower();
            Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
            if (!(sobreescribir is "y" or "s"))
            {
                Console.WriteLine("Operación cancelada. No se generó el archivo Excel.");
                return;
            }
        }

        var excel = new xlsxCreator();

        excel.Create("Hoja1");
        excel.AddRow("AÑO,INICIA,TERMINA,SIGNO,H,M,ELEMENTO,GRUPO KUA H,GRUPO KUA M");

        for (int year = year1; year <= year2; year++)
        {
            var output = new NewChineseYear(year);
            if (year > 2100 || year <= 1900)
                continue;

            // Agregar las dos fechas (inicial y final) y otros datos al archivo
            excel.AddRow(
                $"{output.DateOfStart.year:D4}," +                                                              // Año
                $"{output.DateOfStart.day:D2}/{output.DateOfStart.month:D2}/{output.DateOfStart.year:D4}," +    // Fecha inicial completa
                $"{output.DateOfStart.day:D2}/{output.DateOfStart.month:D2}/{output.DateOfStart.year:D4}," +    // Fecha final completa
                $"{output.Sign.animal}," +                                                                      // Animal
                $"{output.Kua.number.male},{output.Kua.number.female}," +                                       // Kua para hombre y mujer
                $"{output.Sign.element}," +                                                                     // Elemento animal
                $"{output.Kua.group.male},{output.Kua.group.female}"                                            // Coordenadas del Kua
            );
        }

        excel.Close(filePath);

        Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
        Console.WriteLine($"¡Excel generado exitosamente como '{fileName}.xlsx'!");
    }

    public static void PrintTitle()
    {
        Console.WriteLine(
                        "AÑO      " +           // 4 caracteres
                        "INICIA      " +        // 16 caracteres (DD/MM/AAAA formato)
                        "TERMINA     " +        // 16 caracteres (DD/MM/AAAA formato)
                        "SIGNO      \t" +       // 10 caracteres (para signos largos como "Cabra/Oveja")
                        "H\tM  " +              // 4 caracteres (2 para H y 2 para M)
                        "ELEMENTO   " +         // 8 caracteres (para elementos como "Madera")
                        "GRUPO KUA H  " +       // 9 caracteres (Grupo Kua hombre)
                        "GRUPO KUA M"           // 9 caracteres (Grupo Kua mujer)
        );
        Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.

    }
    public static void PrintValue(NewChineseYear output)
    {
        Console.WriteLine(
            $"{output.DateOfStart.year:D4}   " +                                                            // Año (4 dígitos)
            $"{output.DateOfStart.day:D2}/{output.DateOfStart.month:D2}/{output.DateOfStart.year:D4}   " +  // Fecha inicial completa (DD/MM/AAAA)
            $"{output.DateOfEnd.day:D2}/{output.DateOfEnd.month:D2}/{output.DateOfEnd.year:D4}   " +        // Fecha final completa (DD/MM/AAAA)
            $"{output.Sign.animal.PadRight(10)}\t" +                                                        // Animal (ajustado a 10 caracteres)
            $"{output.Kua.number.male}\t{output.Kua.number.female}   " +                                    // Kua para hombre y mujer (2 dígitos)
            $"{output.Sign.element.PadRight(8)}   " +                                                       // Elemento animal (ajustado a 8 caracteres)
            $"{output.Kua.group.male.PadRight(9)}   " +                                                     // Coordenadas Kua hombre (ajustado a 9 caracteres)
            $"{output.Kua.group.female.PadRight(9)}"                                                        // Coordenadas Kua mujer (ajustado a 9 caracteres)
        );
    }

    public static void Exit()
    {
        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey();
        Environment.Exit(0);
    }
}
