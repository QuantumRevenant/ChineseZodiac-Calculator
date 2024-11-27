using QR.General;
using tyme.festival;
using tyme.solar;

public class ChineseZodiacSing()
{
    public static readonly string[] animals = ["Mono", "Gallo", "Perro", "Cerdo/Jabalí", "Rata", "Buey/Búfalo", "Tigre", "Conejo", "Dragón", "Serpiente", "Caballo", "Cabra/Oveja"];
    public static readonly string[] element = ["Metal", "Agua", "Madera", "Fuego", "Tierra"];
    public static readonly string[] polarity = ["Yang", "Yin"];
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
    {
        return QRGeneral.GetPatternValue(animals, iteration);
    }

    public static string GetElement(int iteration)
    {
        return QRGeneral.GetPatternValue(element, iteration, 2);
    }

    public static string GetPolarity(int iteration)
    {
        return QRGeneral.GetPatternValue(polarity, iteration);
    }
    public static ZodiacSign GetZodiacSign(int iteration)
    {
        return new ZodiacSign(GetZodiacAnimal(iteration), GetElement(iteration), GetPolarity(iteration));
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
    public ChineseZodiacSing.ZodiacSign zodiacSign { get; private set; }
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
        zodiacSign = ChineseZodiacSing.GetZodiacSign(year);
        Kua = KuaNumberClass.GetKua(year);
    }
}