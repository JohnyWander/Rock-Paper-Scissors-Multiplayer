using Rock_Paper_Scissors_Multiplayer.Connection.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Rock_Paper_Scissors_Multiplayer.Connection
{
    internal class Host
    {
        internal bool Running;
        TcpListener listener;

        internal HostConfig.GameMode GameMode;

        HandleGamemode HandleGame;


        internal List<ConnectedClient> clients = new List<ConnectedClient>();

        internal void StopServer()
        {

            listener.Stop();


        }

        internal Host(HostConfig config)
        {
            listener = new TcpListener(config.ServerEndpoint);
            GameMode = config.gamemode;
            HandleGame = new HandleGamemode(this.GameMode,this);




            Thread ServerLister = new Thread(() =>
            {
                listener.Start();
                Running = true;

                while (listener.Server.IsBound)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    clients.Add(new ConnectedClient(client));
                    if (clients.Count() == config.Expected_Players)
                    {
                        HandleGamemode.EnoughPlayersConnected.SetResult();
                    }
                }


            });
            ServerLister.Start();

        }




    }

    internal class ConnectedClient
    {
        internal TcpClient client;
        internal Task ClientTask;
        internal NetworkStream client_stream;

        internal string Nickname;

        byte[] buffer = new byte[256];
        public ConnectedClient(TcpClient client)
        {
            Thread ClientThread = new Thread(() =>
            {
                this.client = client;
                this.client_stream = this.client.GetStream();



                client_stream.Read(buffer, 0, buffer.Length);
                

                Nickname = Encoding.UTF8.GetString(buffer);

            });
            ClientThread.Start();
        }
    }

    internal class HandleGamemode
    {

        enum SteerCodes
        {
            Choose = 1,
            CallResult = 2



        }


        enum RockPaperScissors
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }
        private Host serverInstance;
        



        internal static TaskCompletionSource EnoughPlayersConnected = new TaskCompletionSource();
        
        public HandleGamemode(HostConfig.GameMode mode, Host serverInstance)
        {
            this.serverInstance = serverInstance;
            
            Thread GameThread = new Thread(() =>
            {

                switch (mode)
                {
                    case HostConfig.GameMode.Duel:

                        EnoughPlayersConnected.Task.Wait();
                       // Console.WriteLine("Enough clients starting...");
                        DuelGamemode();
                        break;


                    case HostConfig.GameMode.Tournament:

                        Console.WriteLine("Tournament Gamemode is coming soon! launching duel mode..");
                        break;





                }
            });
            GameThread.Start();
            
        }

        public async Task DuelGamemode()
        {
            byte[] player1Buffer = new byte[255];
            byte[] player2Buffer = new byte[255];

            int Player1Points = 0;
            int Player2Points = 0;



            foreach(ConnectedClient cl in serverInstance.clients)
            {
                cl.client_stream.Write(BitConverter.GetBytes((int)this.serverInstance.GameMode));               
            }

            // inform each other about nick names

            /// player 2 name to player 1
           //await  serverInstance.clients[0].client_stream.WriteAsync(Encoding.UTF8.GetBytes(serverInstance.clients[1].Nickname));
          // await serverInstance.clients[1].client_stream.WriteAsync(Encoding.UTF8.GetBytes(serverInstance.clients[0].Nickname));

            while (true)
            {
                player1Buffer = new byte[255];
                player2Buffer = new byte[255];
                // inform clients that they should choose Rock,Paper or Scissors
                foreach (ConnectedClient cl in serverInstance.clients)
                {
                    await cl.client_stream.WriteAsync(BitConverter.GetBytes((int)SteerCodes.Choose));
                }


                await serverInstance.clients[0].client_stream.ReadAsync(player1Buffer);
                await serverInstance.clients[1].client_stream.ReadAsync(player2Buffer);

                RockPaperScissors player1Choosed = (RockPaperScissors)BitConverter.ToInt32(player1Buffer);
                RockPaperScissors player2Choosed = (RockPaperScissors)BitConverter.ToInt32(player2Buffer);


                bool player1Lost= false; 
                bool player2Lost= false ;
                bool draw =false;

                if (player1Choosed == RockPaperScissors.Rock && player2Choosed == RockPaperScissors.Rock) { draw = true; }
                if (player1Choosed == RockPaperScissors.Paper && player2Choosed == RockPaperScissors.Paper) { draw = true; }
                if (player1Choosed == RockPaperScissors.Scissors && player2Choosed == RockPaperScissors.Scissors) { draw = true; }

                if (player1Choosed == RockPaperScissors.Rock && player2Choosed == RockPaperScissors.Paper) { player1Lost = true; player2Lost = false; }
                if (player1Choosed == RockPaperScissors.Rock && player2Choosed == RockPaperScissors.Scissors) { player1Lost = false; player2Lost = true; }

                if (player1Choosed == RockPaperScissors.Paper && player2Choosed == RockPaperScissors.Rock) { player1Lost = false; player2Lost = true; }
                if (player1Choosed == RockPaperScissors.Paper && player2Choosed == RockPaperScissors.Scissors) { player1Lost = true; player2Lost = false; }

                if (player1Choosed == RockPaperScissors.Scissors && player2Choosed == RockPaperScissors.Paper) { player1Lost = false; player2Lost = true; }
                if (player1Choosed == RockPaperScissors.Scissors && player2Choosed == RockPaperScissors.Rock) { player1Lost = true; player2Lost = false; }


                // Informing clients that result will be sent soon

                foreach(ConnectedClient cl in serverInstance.clients)
                {
                    await cl.client_stream.WriteAsync(BitConverter.GetBytes((int)SteerCodes.CallResult));
                }

                // informing players what opponnent chose 
                await serverInstance.clients[0].client_stream.WriteAsync(Encoding.UTF8.GetBytes(player2Choosed.ToString()));
                await serverInstance.clients[1].client_stream.WriteAsync(Encoding.UTF8.GetBytes(player1Choosed.ToString()));

                await Task.Delay(1000);
                // Sending Result to clients

                string Message="";
                if (draw == true)
                {
                    Message = $"Round draw - {serverInstance.clients[0].Nickname} {Player1Points.ToString()}:{Player2Points.ToString()} {serverInstance.clients[1].Nickname}";
                }

                if(player1Lost == true && player2Lost == false)
                {
                    Player2Points++;
                    Message = $"{serverInstance.clients[1].Nickname} Won! - {serverInstance.clients[0].Nickname} {Player1Points}:{Player2Points} {serverInstance.clients[1].Nickname}";
                }

                if (player1Lost == false && player2Lost == true)
                {
                    Player1Points++;
                    Message = $"{serverInstance.clients[1].Nickname} Won! - {serverInstance.clients[0].Nickname} {Player1Points}:{Player2Points} {serverInstance.clients[1].Nickname}";
                }

                foreach(ConnectedClient cl in serverInstance.clients)
                {

                    await cl.client_stream.WriteAsync(Encoding.UTF8.GetBytes(Message));

                }





                
            }


        }



    }

}
