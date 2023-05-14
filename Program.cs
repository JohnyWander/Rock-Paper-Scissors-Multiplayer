using Rock_Paper_Scissors_Multiplayer.Connection;
using Rock_Paper_Scissors_Multiplayer.Connection.Config;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rock_Paper_Scissors_Multiplayer
{
    internal class Program
    {
        static Thread ServerTread = null;
        private static Host Host;

        private static Client client;
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
                    Console.Clear();
                    IPEndPoint endp = GetAdressForConnection();
                    Console.WriteLine("Choose nickname");
                    string nickname = GetUserInput();
                    
                    Program.client = new Client(endp, nickname);

                    break;

                case 2:
                    
                    if (Program.Host  == null)
                    {
                        HostConfig hostconfig = CreateHostConfig();
                        Program.Host = new Host(hostconfig);
                        
                        Console.Clear();
                        menu();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("You are already hosting, do you want to stop server?(Y/N)");
                        ConsoleKeyInfo pressed = Console.ReadKey(true);
                        while (pressed.Key != ConsoleKey.Y || pressed.Key !=ConsoleKey.N)
                        {
                            if (pressed.Key == ConsoleKey.Y)
                            {
                                Program.Host.StopServer();
                                Program.Host = null;
                                Console.Clear();
                                menu();
                                return;
                            }
                            else
                            {
                                Console.Clear();
                                menu();
                                return;

                            }
                        }

                    }


                    break;





            }

        }

        static IPEndPoint GetAdressForConnection()
        {
            Console.WriteLine("Please enter server ip and port like that - \"123.123.123.235:8000\"\n");
            
            
            IPEndPoint? add = null;

            while (!IPEndPoint.TryParse(GetUserInput(), out add))
            {
                Console.WriteLine("That's not valid address");
            }

            

            return add;
        }

        internal static string GetUserInput(string placeholder ="")
        {
            string input = "";

            input = Console.ReadLine();

            if (input != "")
            {
                return input;
            }
            else
            {
                return placeholder;
            }
        }

        internal static ReadOnlySpan<char> GetUserKey()
        {
            char key = Console.ReadKey(true).KeyChar;

            return new ReadOnlySpan<char>(new char[] { key });
        }

        static HostConfig CreateHostConfig()
        {
            Console.Clear();
            HostConfig conf= new HostConfig();

            Console.WriteLine("Please provide IP address to host on(or leave blank and press enter to host on all interfaces)");
            IPAddress ProvidedIp;
            
            while (!IPAddress.TryParse(GetUserInput("0.0.0.0"),out ProvidedIp))
            {
                Console.WriteLine("It's not valid IP adress, enter it again ");
                
            }
            // Console.WriteLine(ProvidedIp.ToString());
            //Console.WriteLine(IPAddress.Any.ToString());


            Console.WriteLine("Please provide port to host on");
            IPEndPoint HostEnpoint;
            while (!IPEndPoint.TryParse(ProvidedIp+":"+GetUserInput("5000"), out HostEnpoint))
            {
                Console.WriteLine("It's not correct port, enter it again(or press enter to use default - 5000)");
            }

            conf.ServerEndpoint = HostEnpoint;


            


            Console.WriteLine("Select game mode\n ");
            Console.WriteLine("1. Duel");
            Console.WriteLine("2. Tournament (min 4 players)");

            conf.gamemode = (HostConfig.GameMode)int.Parse(GetUserInput());

            if (conf.gamemode == HostConfig.GameMode.Duel) { conf.Expected_Players = 2; }
            else
            {
                Console.WriteLine("Please enter how much players will join tournament (Mode is not yet avaiable!!!)");

                while (!int.TryParse(GetUserInput(), out conf.Expected_Players))
                {
                    Console.WriteLine("it's not valid number");
                };

            }

            
            return conf;
        }





















    }
}