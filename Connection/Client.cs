using Rock_Paper_Scissors_Multiplayer.Connection.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Rock_Paper_Scissors_Multiplayer.Connection
{
    internal class Client
    {

        enum SteerCodes
        {
            Choose =1,
            CallResult =2
        }

        enum RockPaperScissors
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }


        TcpClient Connection;

        NetworkStream ClientStream;

        byte[] buffer =new byte[256];

        HostConfig.GameMode GameMode;

        public Client(IPEndPoint ConnectTo,string nickname)
        {
            Thread connectionThread = new Thread(() =>
            {
                Connection = new TcpClient();

                Connection.Connect(ConnectTo);

                ClientStream = Connection.GetStream();

                ClientStream.ReadTimeout = Timeout.Infinite;
                ClientStream.WriteTimeout = Timeout.Infinite;


                ClientStream.Write(Encoding.UTF8.GetBytes(nickname));

                //
                byte[] ModeBuffer = new byte[32];
                ClientStream.Read(ModeBuffer);
                int GameModeint = BitConverter.ToInt32(ModeBuffer);
                

                GameMode = (HostConfig.GameMode)GameModeint;


                switch (GameMode)
                {

                    case HostConfig.GameMode.Duel:
                        HandleDuelMode().Wait();
                        break;

                    case HostConfig.GameMode.Tournament:

                        break;


                }




            });
            connectionThread.Start();


        }



        private async Task HandleDuelMode()
        {
            
            while (true)
            {
               
                byte[] Buffer = new byte[256];



            int RoundNumber = 0;
            
             //   Console.WriteLine("Round " + RoundNumber);

                int Result;
                await ClientStream.ReadAsync(Buffer);
                Result = BitConverter.ToInt32(Buffer);

                SteerCodes code =(SteerCodes)Result;

                switch (code)
                {

                    case SteerCodes.Choose:
                        int choosed;

                        Console.WriteLine("Choose:\n1.Rock \n2. Paper\n3.Scissors");

                        while (!int.TryParse(Program.GetUserKey(), out choosed))
                        {
                            Console.WriteLine("Its not valid input");
                        }

                        await ClientStream.WriteAsync(BitConverter.GetBytes(choosed));

                       

                        break;


                    case SteerCodes.CallResult:

                        Console.WriteLine("RESULTS:");
                        byte[] OpponnentCh = new byte[2048];
                        await ClientStream.ReadAsync(OpponnentCh,0,OpponnentCh.Length);

                        Console.WriteLine("Your opponnent has chosen " + Encoding.UTF8.GetString(OpponnentCh).Trim(Convert.ToChar(0x00)).Trim().Replace("\0",""));

                        byte[] RoundResult = new byte[2048];
                        await ClientStream.ReadAsync(RoundResult);

                        Console.WriteLine(Encoding.UTF8.GetString(RoundResult).Trim(Convert.ToChar(0x00)).Trim().Replace("\0", ""));

                       

                        break;


                        
                }


                RoundNumber++;
                
            }










        }



    }
}
