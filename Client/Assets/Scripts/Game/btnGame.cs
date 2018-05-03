using System.Threading;
using UnityEngine;

public class btnGame : MonoBehaviour
{
    private GameConnection _connection;

    public void DisconnectButtonPress()
    {
        _connection = GameObject.FindGameObjectWithTag("Connection").GetComponent<GameConnection>();
        _connection.Disconnect();
        Thread.Sleep(1000);
        GetComponent<Transition>().ChangeScene();
    }

    public void ChangeToGameScene()
    {
        GetComponent<Transition>().ChangeScene();
    }
}
