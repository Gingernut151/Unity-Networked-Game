using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionData : MonoBehaviour
{
    public int port = 4444;
    public string address = "216.58.207.67"; //Google ip address
    public string username = "Player1";

    private void Awake()
    {
            DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateConnectionData (int port, string address, string username)
    {
        this.port = port;
        this.address = address;
        this.username = username;
	}

    void Update ()
    {
		
	}
}
