using System;
using System.Collections.Generic;
using SharedLibrary;
using ServerLibrary;

namespace Multiplayer_Server
{
    class Match
    {
        private float sendRate = 20.0f;

        float deltaTime = 0.0f;
        HiResTimer timer;

        List<playerGameData> clientsConnected = new List<playerGameData>();
        Server _server;

        Vec3 defaultVec3 = new Vec3();
        Vec4 defaultVec4 = new Vec4();

        int currentPacketAmount = 0;
        int currentPacketsReceived = 0;
        int currentPacketIndexIncoming = 0;
        int currentPacketIndexOutgoing = 0;

        public void InitMatch(List<playerLobbyData>  clients)
        {
            defaultVec3.x = 0.0f;
            defaultVec3.y = 0.0f;
            defaultVec3.z = 0.0f;

            defaultVec4.x = 0.0f;
            defaultVec4.y = 0.0f;
            defaultVec4.z = 0.0f;

            for (int i = 0; i < clients.Count; i++)
            {
                playerGameData client;
                client.address = clients[i].address;
                client.username = clients[i].username;
                client.position = defaultVec3;
                client.rotation = defaultVec4;

                clientsConnected.Add(client);
            }

            InitConnection();
            timer = new HiResTimer();
        }

        public void InitConnection()
        {
            UDP_Config udp_Config;
            udp_Config.address =  "0.0.0.0"; //"127.0.0.1"; //
            udp_Config.port = 5555;

            ServerListenerUDP udpListener = new ServerListenerUDP(udp_Config);
            ProtoBufSerializer udpSerialize = new ProtoBufSerializer();
            ServerConnectionUDP udpConnection = new ServerConnectionUDP("Udp_Game");
            udpConnection.AddListener(udpListener);
            udpConnection.AddSerializer(udpSerialize);

            _server = new Server();
            _server.AddConnection(udpConnection);
        }

        public void StartMatch()
        {
            _server.Start();
        }
        public void EndMatch()
        {
            _server.Stop();
        }

        public void UpdateMatch()
        {
            while (true)
            {
                CalculateDeltaTime();
                HandleGameMessagesIncoming();

                if (clientsConnected.Count == 0)
                {
                    EndMatch();
                    break;
                }
            }
        }

        private void HandleGameMessagesIncoming()
        {
            try
            {
                List<Packet> packetList = _server.RecieveMessages("Udp_Game");
                currentPacketAmount = packetList.Count;

                if (currentPacketAmount > 0)
                {
                    currentPacketsReceived++;
                    Packet packet = packetList[0];

                    HandlePacketMisses(packet.index, packet.type);

                    if (packet.type == PacketType.PING)
                    {
                        _server.ClearMessage("Udp_Game", 0);
                    }
                    else if (packetList[0].type == PacketType.PLAYERPOS)
                    {
                        HandleIncomingPlayerData((PlayerPosPacket)packet);
                        _server.ClearMessage("Udp_Game", 0);
                    }
                }
            }
            catch
            { }
        }

        private void HandleIncomingPlayerData(PlayerPosPacket packet)
        {
            Vec3 pos = packet.position;
            Vec4 rot = packet.rotation;
            string sender = packet.sender;

            for (int i = 0; i < clientsConnected.Count; i++)
            {
                if (sender.Equals(clientsConnected[i].username))
                {
                    playerGameData data = clientsConnected[i];

                    float dist = CalcDistance(pos, data.position);

                    if (dist < 50.0f)
                    {
                        data.username = sender;
                        data.position = pos;
                        data.rotation = rot;
                        clientsConnected[i] = data;
                    }
                }
            }
        }

        private void HandleMessagesOutgoing()
        {
            PlayerData[] lobbyData = new PlayerData[100];

            if (clientsConnected.Count > 0)
            {
                for (int i = 0; i < clientsConnected.Count; i++)
                {
                    lobbyData[i].playerAddress = clientsConnected[i].username;
                    lobbyData[i].position = clientsConnected[i].position;
                    lobbyData[i].rotation = clientsConnected[i].rotation;
                }

                for (int i = clientsConnected.Count; i < lobbyData.Length; i++)
                {
                    lobbyData[i].playerAddress = "-------------";
                    lobbyData[i].position = defaultVec3;
                    lobbyData[i].rotation = defaultVec4;
                }

                AllPlayerPosPacket packet = new AllPlayerPosPacket("Server", lobbyData, currentPacketIndexOutgoing++);
                _server.SendPacketToAll(packet, "Udp_Game");
            }
        }

        private void HandlePacketMisses(int index, PacketType type)
        {
            if (index != currentPacketIndexIncoming + 1)
            {
                if (index < currentPacketIndexIncoming)
                {
                    // TODO
                }
                else
                {
                    // TODO
                    currentPacketIndexIncoming = index;
                }
            }
            else
            {
                currentPacketIndexIncoming = index;
            }
        }

        private void CalculateDeltaTime()
        {
            timer.Stop();
            deltaTime += ((timer.Duration() * 5.0f) / 1000.0f);
            timer.Reset();
            timer.Start();

            if (deltaTime > (1.0f / sendRate))
            {
                deltaTime = 0.0f;
                HandleMessagesOutgoing();
            }

        }

        private float CalcDistance(Vec3 from, Vec3 To)
        {
            Vec3 difference;
            difference.x = from.x - To.x;
            difference.y = from.y - To.y;
            difference.z = from.z - To.z;

            double distance = Math.Sqrt(
                Math.Pow(difference.x, 2.0f) +
                Math.Pow(difference.y, 2.0f) +
                Math.Pow(difference.z, 2.0f));

            return (float)distance;
        }
    }
}