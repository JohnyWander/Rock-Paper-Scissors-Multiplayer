using System.Net;
using System.Text;

namespace Rock_Paper_Scissors_Multiplayer
{
    internal class Program
    {
        static void Main(string[] args)
        {

            menu();





        }


        static void menu()
        {

            Console.WriteLine("1. Connect to server");
            Console.WriteLine("2. Host new game");
            Console.WriteLine("3. Options");
            Console.WriteLine("3. Quit");

            int sw = -1;

            while (sw == -1)
            {
                try
                {//
                    ConsoleKeyInfo k = Console.ReadKey(true);
                    sw = int.Parse(k.KeyChar.ToString());
                }
                catch (System.FormatException)
                {
                    continue;
                }

            }


            switch (sw)
            {

                case 1:
                    IPEndPoint endp = GetAdressForConnection();

                    break;

                case 2:

                    break;





            }

        }

        static IPEndPoint GetAdressForConnection()
        {
            Console.WriteLine("Please enter server ip and port like that - \"123.123.123.235:8000\"\n");

            (int x, int y) = Console.GetCursorPosition();
            IPEndPoint? add = null;

            while (!IPEndPoint.TryParse(GetUserInput(), out add))
            {
                Console.WriteLine("That's not valid address");
            }



            return add;
        }









        static string GetUserInput()
        {
            string input = "";

            input = Console.ReadLine();


            return input;
        }























    }
}