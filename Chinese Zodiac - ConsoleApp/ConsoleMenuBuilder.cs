using System.Globalization;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace QR.QRMenu
{
    class QRMenu
    {
        private static readonly double ErrorValue = -1;
        public static int OptionMenu(string Title, string Subtitle, string[] Options, bool errorOutput = true, bool dontExitReturn = false)
        {
            int number;

            do
            {
                Clear();

                if (!string.IsNullOrEmpty(Title))
                {
                    WriteLine(Title);
                    NewLine();
                }

                if (!string.IsNullOrEmpty(Subtitle))
                {
                    WriteLine(Subtitle);
                    NewLine();
                }

                for (int i = 1; i <= Options.Length; i++)
                {
                    WriteLine($"{i}) {Options[i - 1]}");
                }
                WriteLine($"{0}) {(dontExitReturn ? "Volver" : "Salir")}");

                number = AskIntegerMenu("", (0, Options.Length + 1));

            } while (number == ErrorValue || !errorOutput);

            Clear();

            return number;
        }

        public static bool ConfirmationMenu(string question)
        {
            WriteLine(question + " (Y/S para confirmar, cualquier otra tecla para cancelar)");
            NewLine();

            string input = ReadLine().ToLower();

            return input is "y" or "s";
        }

        public static void Exit()
        {
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static string AskMenu(string question)
        {
            if (!string.IsNullOrEmpty(question))
                WriteLine(question);

            return ReadLine();
        }
        public static void ErrorMenu(string Title = "", string Subtitle = "", int errorCode = 0)
        {
            Clear();
            if (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Subtitle) && errorCode == 0)
            {
                Title = "ERROR NO DEFINIDO";
                Subtitle = "#ERRNDF418";
                errorCode = 418;
            }

            if (!string.IsNullOrEmpty(Title))
            {
                WriteLine(Title);
                NewLine();
            }

            if (!string.IsNullOrEmpty(Subtitle))
            {
                WriteLine(Subtitle);
                NewLine();
            }

            if (errorCode != 0)
            {
                WriteLine($"https://http.cat/{errorCode}");
                NewLine();
            }

            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
            Clear();
        }
        public static bool AskDualOption(string question, string trueValue, string falseValue, bool errorOutput = true, bool defaultOutput = false)
        {
            bool failed;
            bool output;
            do
            {
                string answer = AskMenu($"{question} ({trueValue}/{falseValue})").ToLower();

                failed = answer.ToLower() != trueValue.ToLower() && answer.ToLower() != falseValue.ToLower();

                output = answer.ToLower() == trueValue.ToLower();

                if (failed && !errorOutput)
                    ErrorMenu("Error", $"Recuerda que debe ser {trueValue} o {falseValue}");

            } while (failed && !errorOutput);

            if (failed)
                output = defaultOutput;

            return output;
        }

        public static int AskIntegerMenu(string question, bool errorOutput = true)
        {
            return AskIntegerMenu(question, (double.NegativeInfinity, double.PositiveInfinity), errorOutput);
        }

        public static int AskIntegerMenu(string question, (double minor, double major) boundaries, bool errorOutput = true)
        {
            return (int)AskNumberMenu(question, (boundaries.minor, boundaries.major), errorOutput);
        }

        public static double AskNumberMenu(string question, bool errorOutput = true)
        {
            return AskNumberMenu(question, (double.NegativeInfinity, double.PositiveInfinity), errorOutput);
        }
        public static double AskNumberMenu(string question, (double minor, double major) boundaries, bool errorOutput = true)
        {
            string input;
            bool failed;
            double output;

            if (boundaries.major < boundaries.minor)
                (boundaries.major, boundaries.minor) = (boundaries.minor, boundaries.major);

            do
            {
                input = AskMenu(question);
                failed = !double.TryParse(input, out output) || output < boundaries.minor || output > boundaries.major;

                bool haveBoundaries = !double.IsInfinity(boundaries.minor) || !double.IsInfinity(boundaries.major);

                (string minor, string major) strBoundaries = (double.IsNegativeInfinity(boundaries.minor) ? "Not Def." : boundaries.minor.ToString(), double.IsPositiveInfinity(boundaries.major) ? "Not Def." : boundaries.major.ToString());

                if (failed)
                    ErrorMenu("Error", $"Recuerda que debe ser un número{(haveBoundaries ? $" y debe estar dentro del rango válido [mínimo: {strBoundaries.minor}, máximo:{strBoundaries.major}]" : "")}");

            } while (failed && !errorOutput);

            if (failed)
                return ErrorValue;

            return output;
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void Write(object obj)
        {
            Console.Write(obj);
        }
        public static void WriteLine(object obj)
        {
            Console.WriteLine(obj);
        }
        public static string ReadLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }
        public static void NewLine(int numberOfLines = 1)
        {
            for (int i = 0; i < numberOfLines; i++)
                Console.WriteLine();
        }
    }

    class QRCalendar
    {
        private CultureInfo culture;
        private int DayBlockSize = 1;
        private int leftPadding = 5;
        private readonly string dayBlockSeparator = " | ";
        private bool checkedBlockSize = false;
        public readonly int daysInWeek = 7; // Set to avoid magic numbers, obviously don't edit it (unexpected behaviors)
        public readonly int maxWeeksInMonth = 6;
        public QRCalendar(CultureInfo culture, int DayBlockSize = 1, int leftPadding = 5, string dayBlockSeparator = " | ")
        {
            this.culture = culture;
            this.DayBlockSize = DayBlockSize;
            this.leftPadding = leftPadding;
            this.dayBlockSeparator = dayBlockSeparator;
        }
        private void CheckMinimumDayBlockSize() { CheckMinimumDayBlockSize(out int triggerSize, out int abbreviatedSize, out int shortestSize); }
        private void CheckMinimumDayBlockSize(out int triggerSize, out int abbreviatedSize, out int shortestSize)
        {
            triggerSize = culture.DateTimeFormat.DayNames.Max(s => s.Length);                   // get the longest week name size in culture
            abbreviatedSize = culture.DateTimeFormat.AbbreviatedDayNames.Max(s => s.Length);    // get the abbreviated week name in culture
            shortestSize = culture.DateTimeFormat.ShortestDayNames.Max(s => s.Length);          // get the shortest week name in culture

            checkedBlockSize = true;
            if (DayBlockSize < int.Max(shortestSize, 2)) DayBlockSize = int.Max(shortestSize, 2);
        }
        public void PrintTitle(string leftComponent, string rightComponent = "")
        {
            QRMenu.Write(string.Empty.PadCenter(leftPadding));
            int count = (DayBlockSize + dayBlockSeparator.Length) * daysInWeek - dayBlockSeparator.Length;

            QRMenu.WriteLine($"{leftComponent.PadRight(count - rightComponent.Length)}{rightComponent}");

        }

        public void PrintLine()
        {
            if (!checkedBlockSize)
                CheckMinimumDayBlockSize();

            QRMenu.Write(string.Empty.PadCenter(leftPadding));

            int count = (DayBlockSize + dayBlockSeparator.Length) * daysInWeek - dayBlockSeparator.Length;
            QRMenu.WriteLine(string.Empty.PadCenter(count, '-'));
        }

        public void PrintWeekNames(int firstDayofWeek = 0)
        {
            QRMenu.Write(string.Empty.PadCenter(leftPadding));
            CheckMinimumDayBlockSize(out int triggerSize, out int abbreviatedSize, out int shortestSize);

            string[] dayNames = culture.DateTimeFormat.ShortestDayNames;

            if (DayBlockSize >= triggerSize) dayNames = culture.DateTimeFormat.DayNames;
            else if (DayBlockSize >= abbreviatedSize) dayNames = culture.DateTimeFormat.AbbreviatedDayNames;
            else dayNames = culture.DateTimeFormat.ShortestDayNames;

            if (DayBlockSize < int.Max(shortestSize, 2)) DayBlockSize = int.Max(shortestSize, 2);

            dayNames = dayNames.Select(s => s.ToUpper()).ToArray();

            for (int i = 0; i < daysInWeek; i++)
            {
                int index = (i + firstDayofWeek) % daysInWeek;
                QRMenu.Write(dayNames[index].PadCenter(DayBlockSize));

                if (i != daysInWeek - 1) QRMenu.Write(dayBlockSeparator);
                else QRMenu.NewLine();
            }
        }

        public void PrintWeek(List<string>[] week)
        {
            int lineCount = week.Max(ls => ls?.Count ?? 0);

            for (int l = 0; l < lineCount; l++)
            {
                QRMenu.Write(string.Empty.PadCenter(leftPadding));
                for (int d = 0; d < daysInWeek; d++) // Recorre los días de la semana
                {
                    Console.Write((l < (week[d]?.Count ?? 0) ? week[d][l] : string.Empty).PadRight(DayBlockSize));
                    if (d < daysInWeek - 1) Console.Write(dayBlockSeparator);
                }
                QRMenu.NewLine();
            }
        }

        public void PrintMonth(List<string>[,] month)
        {
            List<string>[] week = new List<string>[daysInWeek];

            for (int w = 0; w < maxWeeksInMonth; w++)
            {
                for (int d = 0; d < daysInWeek; d++) week[d] = month[w, d];

                if (w >= 4 && week[0] == null) return;

                PrintWeek(week);
                PrintLine();
            }
        }
    }
}