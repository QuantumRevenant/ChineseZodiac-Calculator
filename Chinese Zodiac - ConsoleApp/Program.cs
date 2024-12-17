using QR.QRMenu;
internal class Program
{
    private static void Main(string[] args)
    {
        do
        {
            int option;
            string[] options = ["Consultar Zodiaco Chino", "Consultar Tong Shu"];

            option = QRMenu.OptionMenu("Zodiaco Chino y Tong Shu", "Selecciona tu opción", options);

            if (option == 0) QRMenu.Exit();

            switch (option)
            {
                case 1:
                    CZMenu.ChineseZodiacMenu();
                    break;
                case 2:
                    TSMenu.TongShuMenu();
                    break;
                default:
                    QRMenu.ErrorMenu("Error desconocido", "No deberías de estar aquí. Contacta al administrador.\n\n Código de Error: #OPTMEN421", 421);
                    return;
            }
            Console.WriteLine();
        } while (true);
    }

}
