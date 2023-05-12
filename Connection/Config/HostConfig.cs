using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rock_Paper_Scissors_Multiplayer.Connection.Config
{
    internal class HostConfig
    {
        internal IPEndPoint ServerEndpoint;

        internal enum GameMode
        {
            Duel =1,
            Tournament =2

        }

        internal GameMode gamemode;

        internal int Expected_Players_For_Tournament;

    }
}
