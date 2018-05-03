using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using SharedLibrary;
using ClientLibrary;

public class GameConnection : MonoBehaviour
{
    private float _sendRate = 20.0f;
    float _deltaTime = 0.0f;

    //Stopwatch deltaStopwatch;
    Thread _deltaMessage;
    Client _client;

    GameObject _localPlayer;

    private string username = "Player1";
    private List<Packet> gamePacketList = new List<Packet>();

    Vec3 localPosition = new Vec3();
    Vec4 localRotation = new Vec4();

    int _currentPacketsReceived = 0;
    int _currentPacketIndexIncoming = 0;
    int _currentPacketIndexOutgoing = 0;

    void Start()
    {
        ConnectionData data = GameObject.FindGameObjectWithTag("Connection_Data").GetComponent<ConnectionData>();
        _localPlayer = GameObject.FindGameObjectWithTag("Player");

        username = data.username;

        AvatarData[] players = new AvatarData[PlayersData.GetNumOfPlayers()];

        for (int i = 0; i < players.Length; i++)
        {
            AvatarData player = new AvatarData();
            players[i] = player;
        }

        PlayersData.SetAllPlayers(players);

        GameObject bike = GameObject.FindGameObjectWithTag("Player");
        AvatarData local = new AvatarData();
        local.SetAvatar(bike);
        local.ChangePosition(bike.transform.position.x, bike.transform.position.y, bike.transform.position.z);
        local.ChangeRotation(bike.transform.rotation.x, bike.transform.rotation.y, bike.transform.rotation.z);
        local.SetUsername(username);
        PlayersData.SetLocalPlayerData(local);

        _deltaMessage = new Thread(SendPacketsViaDeltaTime);

        Thread.Sleep(2000);

        InitConnection();

    }
    void Update()
    {
        CalculateDeltaTime();
        UpdateLocalPosition();
        RecieveData();
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    private void InitConnection()
    {
        UDP_Config config;
        config.address =  "52.56.41.229"; //"127.0.0.1"; //
        config.port = 5555;

        ProtoBufSerializer serializer = new ProtoBufSerializer();
        ClientListenerUDP udpListener = new ClientListenerUDP(config);
        ClientConnectionUDP udpConnection = new ClientConnectionUDP("Game_Connection", username);
        udpConnection.AddListener(udpListener);
        udpConnection.AddSerializer(serializer);

        _client = new Client(username);
        _client.AddConnection(udpConnection);
        _client.Start();
        Debug.Log("Player connected");

        UdpConnectPacket packet = new UdpConnectPacket(username, config.address, _currentPacketIndexOutgoing++);
        _client.SendMessage(packet, "Game_Connection");
        Debug.Log("Player changed username");

        _deltaMessage.Start();
    }
    public void Disconnect()
    {
        _deltaMessage.Abort();
        _client.Stop();
        Debug.Log("Player has disconnected");
    }

    private void SendData()
    {
        PlayerPosPacket packet = new PlayerPosPacket(username, localPosition, localRotation, _currentPacketIndexOutgoing++);
        _client.SendMessage(packet, "Game_Connection");
    }
    private void RecieveData()
    {
        gamePacketList = _client.RecieveMessages("Game_Connection");

        if (gamePacketList.Count > 0)
        {
            _currentPacketsReceived++;

            if (gamePacketList[0].type == PacketType.PLAYERPOS)
            {
                AllPlayerPosPacket packet = (AllPlayerPosPacket)gamePacketList[0];
                RecieveAllPlayerPosMessage(packet);
            }

            gamePacketList.RemoveAt(0);
        }
    }
    private void RecieveAllPlayerPosMessage(AllPlayerPosPacket packet)
    {
        PlayerData[] data = packet.players;
        AvatarData[] players = PlayersData.GetAllPlayers();
        bool isPresent = false;

        CheckMissingPacket(packet.index, packet.type);

        for (int i = 0; i < data.Length; i++)
        {
            for (int k = 0; k < players.Length; k++)
            {
                if (players[k].GetUsername().Equals(data[i].playerAddress))
                {
                    Vec3 postion = data[i].position;
                    Vec4 rotation = data[i].rotation;

                    players[k].ChangePosition(postion.x, postion.y, postion.z);
                    players[k].ChangeRotation(rotation.x, rotation.y, rotation.z);
                    players[k].SetUsername(data[i].playerAddress);
                    isPresent = true;
                }
            }

            if (isPresent == false)
            {
                for (int x = 0; x < players.Length; x++)
                {
                    if (players[x].GetUsername().Equals(Commons.defaultName))
                    {
                        Vec3 postion = packet.players[x].position;
                        Vec4 rotation = packet.players[x].rotation;

                        players[x].ChangePosition(postion.x, postion.y, postion.z);
                        players[x].ChangeRotation(rotation.x, rotation.y, rotation.z);
                        players[x].SetUsername(data[i].playerAddress);
                        isPresent = false;
                        break;
                    }
                }
            }
        }
    }    
    private void CheckMissingPacket(int index, PacketType type)
    {
        if (index != _currentPacketIndexIncoming + 1)
        {
            if (index < _currentPacketIndexIncoming)
            {
                // To Do
            }
            else
            {
                _currentPacketIndexIncoming = index;
            }
        }
        else
        {
            _currentPacketIndexIncoming = index;
        }
    }

    private void CalculateDeltaTime()
    {
        _deltaTime += Time.deltaTime;
    }
    private void SendPacketsViaDeltaTime()
    {
        while (true)
        {
            if (_deltaTime > (1.0f / _sendRate))
            {
                _deltaTime = 0.0f;
                SendData();
            }
        }
    }
    private void UpdateLocalPosition()
    {
        localPosition.x = _localPlayer.transform.position.x;
        localPosition.y = _localPlayer.transform.position.y;
        localPosition.z = _localPlayer.transform.position.z;

        localRotation.x = _localPlayer.transform.rotation.x;
        localRotation.y = _localPlayer.transform.rotation.y;
        localRotation.z = _localPlayer.transform.rotation.z;
    }
}
