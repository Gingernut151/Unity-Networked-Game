using System.Collections.Generic;
using System.Threading;

using SharedLibrary;
using ServerLibrary;

namespace Multiplayer_Server
{
    class Lobby
    {
        List<playerLobbyData> clientsConnected = new List<playerLobbyData>();
        Server _server;
        HiResTimer timer;

        float sendRate = 20.0f;
        float deltaTime;

        public void InitLobbyConnection()
        {
            TCP_Config tcp_Config;
            tcp_Config.address ="0.0.0.0"; // "127.0.0.1"; //
            tcp_Config.port = 4444;

            ServerListenerTCP lobbyListener = new ServerListenerTCP(tcp_Config);
            ProtoBufSerializer lobbySerialize = new ProtoBufSerializer();
            ServerConnectionTCP lobbyConnection = new ServerConnectionTCP("Lobby_Screen");
            lobbyConnection.AddListener(lobbyListener);
            lobbyConnection.AddSerializer(lobbySerialize);

            _server = new Server();
            _server.AddConnection(lobbyConnection);

            timer = new HiResTimer();
        }

        public void StartLobbyServer()
        {
            _server.Start();
            _server.AllowTcpConnection("Lobby_Screen");

            deltaTime = 0.0f;
            timer.Start();
        }
        public void CloseLobby()
        {
            //_server.Stop();
        }
        public void UpdateLobby()
        {
            while (true)
            {
                CalculateDeltaTime();
                HandleConnections();
                HandleLobbyMessagesIncoming();

                if (StartMatch() == true)
                {
                    LobbyReadyPacket packet = new LobbyReadyPacket("Server", true, 0);
                    _server.SendPacketToAll(packet, "Lobby_Screen");
                    Thread.Sleep(2000);
                    break;
                }
            }
        }

        private void HandleConnections()
        {
            List<ServerClient> clients = _server.GetConnectionClientList("Lobby_Screen");

            try
            {
                for (int x = 0; x < clientsConnected.Count; x++)
                {
                    bool isPresent = false;

                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (clientsConnected[x].username == clients[i]._name)
                        {
                            isPresent = true;
                            break;
                        }
                    }

                    if (isPresent == false)
                    {
                        clientsConnected.RemoveAt(x);
                        x--;
                    }
                }


                for (int x = 0; x < clients.Count; x++)
                {
                    bool isPresent = false;

                    for (int i = 0; i < clientsConnected.Count; i++)
                    {
                        if (clientsConnected[i].username == clients[x]._name)
                        {
                            isPresent = true;
                            break;
                        }
                    }

                    if (isPresent == false)
                    {
                        playerLobbyData player = new playerLobbyData();
                        player.address = clients[x]._address.ToString();
                        player.username = clients[x]._name.ToString();
                        player.ping = -1;
                        clientsConnected.Add(player);
                    }
                }
            }
            catch
            {

            }
        }
        private void HandleLobbyMessagesIncoming()
        {
            List<Packet> packetList;
            packetList = _server.RecieveMessages("Lobby_Screen");

            if (packetList.Count > 0)
            {
                try
                {
                    if (packetList[0].type == PacketType.PING)
                    {
                        int ping = (int)((PingNumPacket)packetList[0]).ping;
                        string sender = ((PingNumPacket)packetList[0]).sender;

                        for (int i = 0; i < clientsConnected.Count; i++)
                        {
                            if (packetList[0].sender.Equals(clientsConnected[i].username))
                            {
                                playerLobbyData data = clientsConnected[i];
                                data.ping = ping;
                                clientsConnected[i] = data;
                            }
                        }
                        _server.ClearMessage("Lobby_Screen", 0);
                    }

                    if (packetList[0].type == PacketType.LOBBYREADY)
                    {
                        bool isReady = ((LobbyReadyPacket)packetList[0]).isReady;
                        string sender = ((LobbyReadyPacket)packetList[0]).sender;

                        for (int i = 0; i < clientsConnected.Count; i++)
                        {
                            if (packetList[0].sender.Equals(clientsConnected[i].username))
                            {
                                playerLobbyData readyData = clientsConnected[i];
                                readyData.isReady = isReady;
                                clientsConnected[i] = readyData;
                            }
                        }

                        _server.ClearMessage("Lobby_Screen", 0);
                    }
                }
                catch
                {

                }
            }
        }
        private void HandleLobbyMessagesOutgoing()
        {
            LobbyData[] lobbyData = new LobbyData[10];

            if (clientsConnected.Count > 0)
            {
                for (int i = 0; i < clientsConnected.Count; i++)
                {
                    lobbyData[i].isPlayer = true;
                    lobbyData[i].isReady = clientsConnected[i].isReady;
                    lobbyData[i].ping = clientsConnected[i].ping;
                    lobbyData[i].playerAddress = clientsConnected[i].username;
                }

                for (int i = clientsConnected.Count; i < 10; i++)
                {
                    lobbyData[i].isPlayer = false;
                    lobbyData[i].isReady = false;
                    lobbyData[i].ping = -1;
                    lobbyData[i].playerAddress = "-----------";
                }

                LobbyPacket packet = new LobbyPacket("Server", lobbyData, 0);
                _server.SendPacketToAll(packet, "Lobby_Screen");
            }
        }
        private bool StartMatch()
        {
            for (int i = 0; i < clientsConnected.Count; i++)
            {
                if (clientsConnected[i].isReady == false)
                {
                    return false;
                }
            }

            if (clientsConnected.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<playerLobbyData> GetClients()
        {
            return clientsConnected;
        }

        private void CalculateDeltaTime()
        {
            timer.Stop();
            deltaTime += (timer.Duration() / 1000.0f) * 2.0f;
            timer.Reset();
            timer.Start();

            if (deltaTime > (1.0f / sendRate))
            {
                deltaTime = 0.0f;
                HandleLobbyMessagesOutgoing();
            }
        }
    }
}
