using System;
using System.Globalization;
using QR.General;
public class ChineseAnimals()
{
    public static readonly string[] animals = ["Mono", "Gallo", "Perro", "Cerdo/Jabalí", "Rata", "Buey/Búfalo", "Tigre", "Conejo", "Dragón", "Serpiente", "Caballo", "Cabra/Oveja"];
    public static readonly string[] elementPerYear = ["Metal", "Metal", "Agua", "Agua", "Madera", "Madera", "Fuego", "Fuego", "Tierra", "Tierra"];

    public static string GetFromValue(int animalID)
    {
        return animals[animalID];
    }
    public static string GetElement(int year)
    {
        return elementPerYear[QRGeneral.SelectLastNDigits(year, 1)];
    }
    public static (string, string) GetSign(int year)
    {
        return (GetFromValue(year % 12), GetElement(year));
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
    public (int year, int month, int day) DateOfStart { get; private set; }
    public (int year, int month, int day) DateOfEnd { get; private set; }
    public (string animal, string element) Sign { get; private set; }
    public ((int male, int female) number, (string male, string female) group) Kua { get; private set; }
    public NewChineseYear(int year)
    {
        if (year > 2100 || year <= 1900)
        {
            Console.WriteLine($"No podemos calcular el rango {year} :(");
            year = 1901;
        }

        DateOfStart = GetStartOfChineseNewYear(year);
        DateOfEnd = GetEndOfChineseYear(year);

        Sign = ChineseAnimals.GetSign(year);
        Kua = KuaNumberClass.GetKua(year);
    }

    public static (int year, int month, int day) GetStartOfChineseNewYear(int yearAsked)
    {
        ChineseLunisolarCalendar chinese = new ChineseLunisolarCalendar();
        GregorianCalendar gregorian = new GregorianCalendar();

        // Obtener el Año Nuevo Chino para el año solicitado
        DateTime chineseNewYear = chinese.ToDateTime(yearAsked, 1, 1, 0, 0, 0, 0);

        // Convertir la fecha al calendario gregoriano
        int year = gregorian.GetYear(chineseNewYear);
        int month = gregorian.GetMonth(chineseNewYear);
        int day = gregorian.GetDayOfMonth(chineseNewYear);

        return (year, month, day);
    }

    public static (int year, int month, int day) GetEndOfChineseYear(int yearAsked)
    {
        ChineseLunisolarCalendar chinese = new ChineseLunisolarCalendar();
        GregorianCalendar gregorian = new GregorianCalendar();

        // Obtener el último mes y último día del año chino
        int lastMonth = chinese.GetMonthsInYear(yearAsked);
        int lastDay = chinese.GetDaysInMonth(yearAsked, lastMonth);

        // Convertir la fecha del último día al calendario gregoriano
        DateTime endOfYear = chinese.ToDateTime(yearAsked, lastMonth, lastDay, 0, 0, 0, 0);
        int year = gregorian.GetYear(endOfYear);
        int month = gregorian.GetMonth(endOfYear);
        int day = gregorian.GetDayOfMonth(endOfYear);

        return (year, month, day);
    }
}