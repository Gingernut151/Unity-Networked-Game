using UnityEngine.UI;
using UnityEngine;

public class btnLobby : MonoBehaviour
{
    private LobbyConnection _connection;

    public void ConnectButtonPress()
    {
        _connection = GameObject.FindGameObjectWithTag("Connection").GetComponent<LobbyConnection>();
        string name = GameObject.FindGameObjectWithTag("inpBxName").GetComponent<InputField>().textComponent.text;
        Text btnConnect = GameObject.FindGameObjectWithTag("Lobby_ConnectBtn").GetComponentInChildren<Text>();

        _connection._username = name;
        _connection.ConnectToServer();

        if (_connection.isConnected() == true)
        {
            btnConnect.text = "Disconnect";        
        }
        else
        {
            btnConnect.text = "Connect";
        }
    }

    public void ReadyButtonPress()
    {
        _connection = GameObject.FindGameObjectWithTag("Connection").GetComponent<LobbyConnection>();
        _connection.ReadyToPlay();
    }

    public void ChangeToGameScene()
    {
        GetComponent<Transition>().ChangeScene();
    }
}
