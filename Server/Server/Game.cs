using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Server
{
    class Game
    {
        List<playerLobbyData> clients;

        public void Run()
        {
            while (true)
            {
                RunLobby();
                RunMatch();
            }
        }

        private void RunLobby()
        {
            Console.WriteLine("Entering Lobby");
            Lobby lobby = new Lobby();
            lobby.InitLobbyConnection();
            lobby.StartLobbyServer();
            lobby.UpdateLobby();
            clients = lobby.GetClients();
            lobby.CloseLobby();
        }

        private void RunMatch()
        {
            Console.WriteLine("Entering Game");
            Match match = new Match();
            match.InitMatch(clients);
            match.StartMatch();
            match.UpdateMatch();
        }
    }
}
