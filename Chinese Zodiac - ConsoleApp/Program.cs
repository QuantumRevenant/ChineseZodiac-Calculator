using QR.QRMenu;
internal class Program
{
    private static void Main(string[] args)
    {
        do
        {
            int option;
            string[] options = ["Calcular una fecha única", "Mostrar un rango completo en consola", "Generar Excel con un rango completo (limitado del año 1900 a 2100)"];

            option = QRMenu.OptionMenu("Zodiaco de Año Chino", "¿Qué deseas hacer?", options);

            if (option == 0) Exit();

            int year1 = (int)QRMenu.AskNumberMenu($"Escribe el{((option != 1) ? " primer" : "")} año:", false);
            int year2 = year1;

            Console.WriteLine();

            if (option != 1) year2 = (int)QRMenu.AskNumberMenu($"Escribe el segundo año:", false);


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
                    QRMenu.ErrorMenu("Error desconocido", "No deberías de estar aquí. Contacta al administrador.\n\n Código de Error: #OPTMEN421", 421);
                    Exit();
                    break;
            }
            Console.WriteLine();
        } while (QRMenu.ConfirmationMenu("¿Deseas continuar?"));
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
        excel.AddRow("AÑO,INICIA,TERMINA,SIGNO,H,M,ELEMENTO,GRUPO KUA H,GRUPO KUA M,POLARIDAD");

        for (int year = year1; year <= year2; year++)
        {
            var output = new NewChineseYear(year);
            if (year > 2100 || year <= 1900)
                continue;

            // Agregar las dos fechas (inicial y final) y otros datos al archivo
            excel.AddRow(
            $"{output.StartDate.Year:D4}," +                                                          // Año (4 dígitos)
            $"{output.StartDate.Day:D2}/{output.StartDate.Month:D2}/{output.StartDate.Year:D4}," +    // Fecha inicial completa (DD/MM/AAAA)
            $"{output.EndDate.Day:D2}/{output.EndDate.Month:D2}/{output.EndDate.Year:D4}," +          // Fecha final completa (DD/MM/AAAA)
            $"{output.zodiacSign.ZodiacAnimal.PadRight(10)}," +                                       // Animal (ajustado a 10 caracteres)
            $"{output.Kua.number.male},{output.Kua.number.female}," +                                // Kua para hombre y mujer (2 dígitos)
            $"{output.zodiacSign.ZodiacElement.PadRight(8)}," +                                       // Elemento animal (ajustado a 8 caracteres)
            $"{output.Kua.group.male.PadRight(9)}," +                                                 // Coordenadas Kua hombre (ajustado a 9 caracteres)
            $"{output.Kua.group.female.PadRight(9)}," +                                               // Coordenadas Kua mujer (ajustado a 9 caracteres)
            $"{output.zodiacSign.ZodiacPolarity.PadRight(4)},"                                        // Polaridad animal (ajustado a 4 caracteres)
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
                        "ELEMENTO   " +         // 8 caracteres (para elementos como "Madera")
                        "POLARIDAD\t" +           // 4 caracteres (para polaridades como "Yang")
                        "H\tM  " +              // 4 caracteres (2 para H y 2 para M)
                        "GRUPO KUA H  " +       // 9 caracteres (Grupo Kua hombre)
                        "GRUPO KUA M"           // 9 caracteres (Grupo Kua mujer)
        );
        Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.

    }
    public static void PrintValue(NewChineseYear output)
    {
        Console.WriteLine(
            $"{output.StartDate.Year:D4}   " +                                                          // Año (4 dígitos)
            $"{output.StartDate.Day:D2}/{output.StartDate.Month:D2}/{output.StartDate.Year:D4}   " +    // Fecha inicial completa (DD/MM/AAAA)
            $"{output.EndDate.Day:D2}/{output.EndDate.Month:D2}/{output.EndDate.Year:D4}   " +          // Fecha final completa (DD/MM/AAAA)
            $"{output.zodiacSign.ZodiacAnimal.PadRight(10)}\t" +                                        // Animal (ajustado a 10 caracteres)
            $"{output.zodiacSign.ZodiacElement.PadRight(8)}   " +                                       // Elemento animal (ajustado a 8 caracteres)
            $"{output.zodiacSign.ZodiacPolarity.PadRight(4)}\t\t" +                                      // Polaridad animal (ajustado a 4 caracteres)
            $"{output.Kua.number.male}\t{output.Kua.number.female}   " +                                // Kua para hombre y mujer (2 dígitos)
            $"{output.Kua.group.male.PadRight(9)}   " +                                                 // Coordenadas Kua hombre (ajustado a 9 caracteres)
            $"{output.Kua.group.female.PadRight(9)}"                                                    // Coordenadas Kua mujer (ajustado a 9 caracteres)
        );
    }

    public static void Exit()
    {
        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey();
        Environment.Exit(0);
    }
}
