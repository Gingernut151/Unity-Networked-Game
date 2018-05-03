using System.Net.NetworkInformation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SharedLibrary;
using ClientLibrary;
using System.Collections;

public class LobbyConnection : MonoBehaviour
{
    public bool _isConnected { get; private set; }
    private bool _isReady = false;

    TCP_Config _config;
    Client _client;
    ConnectionData _data;

    private List<Packet> _lobbyPacketList = new List<Packet>();

    PopulateTable _lobbyTable;
    int _currentPacketIndex = 0;

    UnityEngine.Ping _ping;
    private long _pingTime;

    public string _username = "Player1";

    void OnApplicationQuit()
    {
        DisconnectFromServer();
    }

    void Start ()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        InitConnection();

        _data = GameObject.FindGameObjectWithTag("Connection_Data").GetComponentInChildren<ConnectionData>();
        _lobbyTable = GameObject.FindGameObjectWithTag("Lobby_Table").GetComponentInChildren<PopulateTable>();

        _ping = new UnityEngine.Ping("52.56.41.229");
    }
	void Update ()
    {
        if (_isConnected == true)
        {
            _lobbyPacketList = GetMessages();
            UpdateData();        
        }

    }

    private void InitConnection()
    {
        _config.address =  "52.56.41.229"; //"127.0.0.1"; //
        _config.port = 4444;

        ProtoBufSerializer tcpSerializer = new ProtoBufSerializer();
        ClientListenerTCP chatListener = new ClientListenerTCP(_config);
        ClientConnectionTCP tcpConnection = new ClientConnectionTCP("Lobby_Connection", _username);
        tcpConnection.AddListener(chatListener);
        tcpConnection.AddSerializer(tcpSerializer);

        _client = new Client(_username);
        _client.AddConnection(tcpConnection);
    }
    private void UpdateData()
    {
        if (_lobbyPacketList.Count > 0)
        {
            if (_lobbyPacketList[0].type == PacketType.LOBBYREADY)
            {
                _client.Stop();
                GetComponent<Transition>().ChangeScene();
            }
            if (_lobbyPacketList[0].type == PacketType.LOBBY)
            {
                LobbyPacket packet = (LobbyPacket)_lobbyPacketList[0];
                _lobbyTable.PopulateData(packet);
            }

            _lobbyPacketList.RemoveAt(0);
        }
    }

    public List<Packet> GetMessages()
    {
        return _lobbyPacketList = _client.RecieveMessages("Lobby_Connection");
    }
    public void DeleteMessages()
    {
        _client.ClearMessages("Lobby_Connection");
    }

    public void ConnectToServer()
    {
        if (_isConnected == true)
        {

            DisconnectFromServer();
            _isConnected = false;
        }
        else
        {
            _client.Start();

            UsernamePacket packet = new UsernamePacket(_username, _currentPacketIndex++);
            _client.SendMessage(packet, "Lobby_Connection");
            _client.ChangeUsername("Lobby_Connection", _username);

            _isConnected = true;

            _data.UpdateConnectionData(_config.port, _config.address, _username);

            StartCoroutine(PingUpdate());
        }
    }
    public void DisconnectFromServer()
    {
        _client.Stop();
        _isConnected = false;
        StopCoroutine(PingUpdate());
    }
    public void ReadyToPlay()
    {
        _isReady = !_isReady;

        LobbyReadyPacket packet = new LobbyReadyPacket(_username, _isReady, _currentPacketIndex++);
        _client.SendMessage(packet, "Lobby_Connection");
    }
    public bool isConnected()
    {
        return _isConnected;
    }

    IEnumerator PingUpdate()
    {
        yield return new WaitForSeconds(1.0f);

        if (_ping.isDone)
        {
            _pingTime = _ping.time;

            PingNumPacket packet = new PingNumPacket(_username, _pingTime, _currentPacketIndex++);
            _client.SendMessage(packet, "Lobby_Connection");

            _ping = new UnityEngine.Ping("52.56.41.229");
        }
        StartCoroutine(PingUpdate());
    }
}
