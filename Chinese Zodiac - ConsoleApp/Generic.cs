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
    }

}