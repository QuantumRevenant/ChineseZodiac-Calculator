using System.Globalization;
using QR.QRMenu;
class TSMenu
{
    public static void TongShuMenu()
    {
        do
        {
            int option;
            string[] options = ["Mostrar un mes único", "Seleccionar que meses mostrar", "Mostrar un año entero"];

            option = QRMenu.OptionMenu("Tong Shu", "Selecciona tu opción para el calendario", options, true, true);

            if (option == 0) return;

            int year = (int)QRMenu.AskNumberMenu($"Escribe el año:", false);

            string strMonths = (option != 2) ? "" : QRMenu.AskMenu("Escriba números de página e rangos separados por comas contando desde el inicio del documento o de la sección. Por ejemplo, escriba 1, -3, 5-12, -8--5\n" +
                                                            "Escribe los meses:");

            int month = (option != 1) ? 1 : (int)QRMenu.AskNumberMenu($"Escribe el mes:", false);

            switch (option)
            {
                case 1:
                    PrintCalendar(year, month);
                    break;
                case 2:
                    PrintCalendar(ParseSelection(strMonths, year));
                    break;
                case 3:
                    PrintCalendar(ParseSelection("1-12", year));
                    break;
                default:
                    QRMenu.ErrorMenu("Error desconocido", "No deberías de estar aquí. Contacta al administrador.\n\n Código de Error: #OPTMEN421", 421);
                    return;
            }
            Console.WriteLine();
        } while (QRMenu.ConfirmationMenu("¿Deseas volver al menú anterior?"));
        QRMenu.Exit();
    }

    static void PrintCalendar(List<(int month, int year)> months, int firstDayofWeek = -1)
    {
        foreach (var (month, year) in months)
            PrintCalendar(year, month, firstDayofWeek);
    }

    static void PrintCalendar(int year, int month, int firstDayofWeek = -1)
    {
        int dayBlockSize = 12;
        int leftPadding = 5;
        int d;
        int w;


        // Configurar la cultura para obtener los nombres de los días correctamente
        CultureInfo culture = CultureInfo.CurrentCulture;
        if (firstDayofWeek == -1) firstDayofWeek = (int)culture.DateTimeFormat.FirstDayOfWeek;


        QRCalendar calendar = new QRCalendar(culture, dayBlockSize, leftPadding);

        ChineseZodiacSign.ZodiacSign YearSign = ChineseZodiacSign.GetZodiacSign(year);
        ChineseZodiacSign.ZodiacSign MonthSign = ChineseZodiacSign.GetZodiacMonth(month, year);
        calendar.PrintTitle($"{culture.DateTimeFormat.GetMonthName(month)} - {MonthSign.ZodiacAnimal} {MonthSign.ZodiacElement} {MonthSign.ZodiacPolarity}",
                            $"{YearSign.ZodiacAnimal} {YearSign.ZodiacElement} {year}");
        QRMenu.NewLine();
        calendar.PrintWeekNames(firstDayofWeek);
        calendar.PrintLine();

        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        List<string>[,] monthArr = new List<string>[calendar.maxWeeksInMonth, calendar.daysInWeek];

        int startDay = ((int)firstDayOfMonth.DayOfWeek - firstDayofWeek + calendar.daysInWeek) % calendar.daysInWeek;

        d = startDay;
        w = 0;

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new DateTime(year, month, day);
            ChineseZodiacSign.ZodiacSign zodiacSignDay = ChineseZodiacSign.GetZodiacDay(date);


            List<string> dayLines =
            [
                $"{day}{(QRTymeExtension.isJie(date) ? "*" : "")}{(QRTymeExtension.isNewYear(date)?"#":"")}",
                zodiacSignDay.ZodiacAnimal,
                zodiacSignDay.ZodiacElement,
                $"{GetStarDay(date)}   {GetCollisionDay(date)}",
            ];
            monthArr[w, d] = dayLines;
            d++;
            if ((day + startDay) % 7 == 0)
            {
                d = 0;
                w++;
            }
        }

        calendar.PrintMonth(monthArr);
    }

    // Lógica personalizada (placeholders por ahora)
    public static string GetStarDay(DateTime date)
    {
        return $"¤{ChineseZodiacSign.GetStarDay(date)}";
    }

    public static string GetCollisionDay(DateTime date)
    {
        // Calcular las iteraciones
        int dayIteration = (date - ChineseZodiacSign.baseDate).Days % 12; // Iteración de días
        int monthIteration = (date.Month + 4) % 12; // Iteración de meses
        int yearIteration = date.Year % 12; // Iteración de años

        bool inMonth = date.Date >= QRTymeExtension.getSolarTermJie(date.Year, date.Month);
        bool inYear = date.Date >= QRTymeExtension.getNewSolarYear(date.Year);
        
        bool monthCollision = ChineseZodiacSign.CheckCollision(dayIteration, monthIteration);
        bool yearCollision = ChineseZodiacSign.CheckCollision(dayIteration, yearIteration);

        if (!inYear)
            yearCollision = ChineseZodiacSign.CheckCollision(dayIteration, (date.Year - 1) % 12);

        if (!inMonth)
            monthCollision = ChineseZodiacSign.CheckCollision(dayIteration, (date.Month - 1 + 4) % 12);


        if (monthCollision && yearCollision) return "AM";
        else if (monthCollision) return "M";
        else if (yearCollision) return "A";
        else return "     ";
    }

    static List<(int month, int year)> ParseSelection(string input, int baseYear)
    {
        var result = new List<(int month, int year)>();
        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            // Detectamos si es un rango con un guión válido
            int rangeSeparatorIndex = part.IndexOf('-', 1); // Busca un '-' que no sea al principio
            if (rangeSeparatorIndex > 0)
            {
                string startPart = part.Substring(0, rangeSeparatorIndex); // Antes del '-'
                string endPart = part.Substring(rangeSeparatorIndex + 1);  // Después del '-'

                if (int.TryParse(startPart, out int start) &&
                    int.TryParse(endPart, out int end))
                {
                    if (start <= end) // Rango creciente
                        AddRange(result, start, end, baseYear);
                    else // Rango decreciente
                        AddRange(result, end, start, baseYear);
                }
            }
            else if (int.TryParse(part, out int singleValue)) // Valor individual
            {
                AddMonth(result, singleValue, baseYear);
            }
        }
        return result;
    }

    static void AddRange(List<(int month, int year)> result, int start, int end, int baseYear)
    {
        if (start <= end) // Rango en orden
        {
            for (int i = start; i <= end; i++)
            {
                AddMonth(result, i, baseYear);
            }
        }
        else // Rango en orden inverso
        {
            for (int i = start; i >= end; i--)
            {
                AddMonth(result, i, baseYear);
            }
        }
    }

    static void AddMonth(List<(int month, int year)> result, int monthOffset, int baseYear)
    {
        int totalMonths = (baseYear * 12) + monthOffset - 1; // Convertimos año y mes a un índice absoluto
        int year = totalMonths / 12;
        int month = (totalMonths % 12) + 1;

        if (month <= 0) // Ajustamos cuando el mes es negativo
        {
            month += 12;
            year -= 1;
        }

        result.Add((month, year));
    }

}
