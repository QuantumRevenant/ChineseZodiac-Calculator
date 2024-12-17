using QR.General;
using QR.QRMenu;
using tyme.festival;
using tyme.solar;

public class ChineseZodiacSign()
{
    public static readonly string[] animals = ["Mono", "Gallo", "Perro", "Cerdo/Jabalí", "Rata", "Buey/Búfalo", "Tigre", "Conejo", "Dragón", "Serpiente", "Caballo", "Cabra/Oveja"];
    public static readonly string[] element = ["Metal", "Agua", "Madera", "Fuego", "Tierra"];
    public static readonly string[] polarity = ["Yang", "Yin"];
    public static readonly string[] star = { "3", "4", "5", "6", "7", "8", "9", "1", "2" };
    public static readonly DateTime baseDate = new DateTime(2023, 8, 30); // Día Mono Metal en Mes Mono Metal
    public class ZodiacSign
    {
        public string ZodiacAnimal { get; private set; }
        public string ZodiacElement { get; private set; }
        public string ZodiacPolarity { get; private set; }

        public ZodiacSign(string animal, string element, string polarity)
        {
            ZodiacAnimal = animal;
            ZodiacElement = element;
            ZodiacPolarity = polarity;

        }
    }
    public static string GetZodiacAnimal(int iteration)
    { return QRGeneral.GetPatternValue(animals, iteration); }

    public static string GetElement(int iteration)
    { return QRGeneral.GetPatternValue(element, iteration, 2); }

    public static string GetPolarity(int iteration)
    { return QRGeneral.GetPatternValue(polarity, iteration); }
    public static ZodiacSign GetZodiacSign(int iteration)
    { return new ZodiacSign(GetZodiacAnimal(iteration), GetElement(iteration), GetPolarity(iteration)); }
    public static ZodiacSign GetZodiacDay(DateTime date)
    {
        int iteration = (date - baseDate).Days;
        return new ZodiacSign(GetZodiacAnimal(iteration), GetElement(iteration), GetPolarity(iteration));
    }
    public static ZodiacSign GetZodiacMonth(int month, int year)
    {
        int iteration = month + 12 * year-(baseDate.Month+12*baseDate.Year);
        int animIT = iteration % 12;
        int elemIT = iteration-2 % 10;
        return new ZodiacSign(GetZodiacAnimal(iteration), GetElement(iteration), GetPolarity(iteration));
    }
    public static ZodiacSign GetZodiacMonthDate(DateTime date)
    {
        int iteration = (date.Year - baseDate.Year) * 12 + date.Month - baseDate.Month;
        return new ZodiacSign(GetZodiacAnimal(iteration), GetElement(iteration), GetPolarity(iteration));
    }
    public static string GetStarDay(DateTime date)
    {
        string getComplement(string value)
        {
            if (int.TryParse(value, out int numericValue))
            {
                return (10 - numericValue).ToString();
            }
            throw new ArgumentException("El valor del patrón no es numérico.");
        }

        int iteration = ((date - baseDate).Days + 2) % 9;
        string pattern = QRGeneral.GetPatternValue(star, iteration);
        string complement = getComplement(pattern);


        DateTime SolarTermQi06 = QRTymeExtension.SolarDayToDateTime(SolarTerm.FromIndex(date.Year, 2 * 6).JulianDay.GetSolarDay());
        DateTime SolarTermQi12 = QRTymeExtension.SolarDayToDateTime(SolarTerm.FromIndex(date.Year, 2 * 12).JulianDay.GetSolarDay());



        if (date.Date == SolarTermQi06.Date || date.Date == SolarTermQi12.Date)
            return $"{pattern}/{complement}";
        else if (date.Date > SolarTermQi06.Date && date.Date < SolarTermQi12.Date)
            return complement;
        else
            return pattern;
    }
    public static bool CheckCollision(int index1, int index2)
    {
        return Math.Abs((index1 + 12) % 12 - (index2 + 12) % 12) == 6;
    }
}
public class KuaNumberClass()
{
    public static readonly (int male, int female) Century20 = (10, 5);
    public static readonly (int male, int female) Century21 = (9, 6);
    public enum Genre { male, female };

    public static ((int, int), (string, string)) GetKua(int year)
    {
        return (GetKuaNumber(year), GetKuaGroup(GetKuaNumber(year)));
    }

    public static (int, int) GetKuaNumber(int year)
    {
        return (GetKuaNumber(year, Genre.male), GetKuaNumber(year, Genre.female));
    }

    public static int GetKuaNumber(int year, Genre genre)
    {
        int kuaNumber;
        (int male, int female) modifier;

        int factor = QRGeneral.SelectLastNDigits(year, 2);

        while (factor > 9) factor = QRGeneral.SumOfDigits(factor);

        if (year < 2000)
            modifier = Century20;
        else
            modifier = Century21;


        if (genre == Genre.male)
        {
            kuaNumber = modifier.male - factor;

            if (kuaNumber == 0) return 9;
            else if (kuaNumber == 5) return 2;

            return kuaNumber;
        }
        else
        {
            kuaNumber = QRGeneral.SumOfDigits(modifier.female + factor);

            if (kuaNumber == 5) return 8;

            return kuaNumber;
        }
    }
    public static (string, string) GetKuaGroup((int male, int female) number)
    {
        return (GetKuaGroup(number.male), GetKuaGroup(number.female));
    }

    public static string GetKuaGroup(int number)
    {
        int[] Este = [1, 3, 4, 9];
        int[] Oeste = [2, 6, 7, 8];

        if (Este.Contains(number))
            return "Este";
        else if (Oeste.Contains(number))
            return "Oeste";
        else
            return "Invalido";
    }
}
public class NewChineseYear
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public ChineseZodiacSign.ZodiacSign zodiacSign { get; private set; }
    public ((int male, int female) number, (string male, string female) group) Kua { get; private set; }

    public NewChineseYear(int year)
    {
        if (year > 2100 || year <= 1900)
        {
            Console.WriteLine($"No podemos calcular el rango {year} :(");
            year = 1901;
        }

        SolarDay startNewYearDate = LunarFestival.FromIndex(year, 0).Day.GetSolarDay();
        SolarDay nextNewYearDate = LunarFestival.FromIndex(year + 1, 0).Day.GetSolarDay();

        StartDate = new DateTime(startNewYearDate.Year, startNewYearDate.Month, startNewYearDate.Day);
        EndDate = new DateTime(nextNewYearDate.Year, nextNewYearDate.Month, nextNewYearDate.Day).AddDays(-1);
        zodiacSign = ChineseZodiacSign.GetZodiacSign(year);
        Kua = KuaNumberClass.GetKua(year);
    }
}

public class QRTymeExtension
{
    public static DateTime SolarDayToDateTime(SolarDay solarDay)
    { return new DateTime(solarDay.Year, solarDay.Month, solarDay.Day); }

    public static SolarDay DateTimeToSolarDay(DateTime dateTime)
    { return SolarDay.FromYmd(dateTime.Year, dateTime.Month, dateTime.Day); }

    public static bool IsSolarTerm(DateTime dateTime)
    {
        int year = dateTime.Year;

        return GetYearSolarTerms(year).Any(d => d.Date == dateTime.Date);
    }

    public static bool isJie(DateTime dateTime)
    {
        if (!IsSolarTerm(dateTime)) return false;

        return DateTimeToSolarDay(dateTime).Term.IsJie;
    }

    public static bool isQi(DateTime dateTime)
    {
        if (!IsSolarTerm(dateTime)) return false;

        return DateTimeToSolarDay(dateTime).Term.IsQi;
    }
    public static bool isNewYear(DateTime dateTime)
    {
        return dateTime == getNewYear(dateTime.Year);
    }
    public static DateTime getSolarTerm(int year, int index)
    {
        return SolarDayToDateTime(SolarTerm.FromIndex(year, index).JulianDay.GetSolarDay());
    }
    public static DateTime getSolarTermJie(int year, int index) { return getSolarTerm(year, index * 2 - 1); }
    public static DateTime getSolarTermQi(int year, int index) { return getSolarTerm(year, index * 2); }

    public static DateTime getNewYear(int year)
    {
        return SolarDayToDateTime(LunarFestival.FromIndex(year, 0).Day.GetSolarDay());
    }

    private static List<DateTime> GetYearSolarTerms(int year)
    {
        var solarDates = new List<DateTime>();

        for (int i = 1; i <= 24; i++)
        {
            SolarDay solarDay = SolarTerm.FromIndex(year, i).JulianDay.GetSolarDay();
            solarDates.Add(SolarDayToDateTime(solarDay));
        }

        return solarDates;
    }
}