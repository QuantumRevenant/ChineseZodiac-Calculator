namespace QR.General
{
    class QRGeneral
    {
        public static int SelectLastNDigits(int number, int n = 1)
        {
            var chars = number.ToString();

            if (n <= 0) return 0;

            var lastDigits = chars.Length >= n
                ? chars.Substring(chars.Length - n)
                : chars;

            return int.Parse(lastDigits);
        }

        public static int SumOfDigits(int number)
        {
            number = Math.Abs(number);
            int sum = 0;

            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }

            return sum;
        }

        public static string GetPatternValue(string[] pattern, int iterationElapsed, int groupSize = 1)
        {
            if (pattern.Length == 0)
                return string.Empty;

            int iterationGroup = iterationElapsed / groupSize;
            int index = (iterationGroup % pattern.Length + pattern.Length) % pattern.Length;
            return pattern[index];
        }
    }

}

namespace System
{
    public static class StringExtensions
    {
        public static string PadCenter(this string str, int totalWidth, char paddingChar = ' ')
        {
            int padding = totalWidth - str.Length;
            int padLeft = padding / 2 + str.Length;
            return str.PadLeft(padLeft, paddingChar).PadRight(totalWidth, paddingChar);
        }
    }
}