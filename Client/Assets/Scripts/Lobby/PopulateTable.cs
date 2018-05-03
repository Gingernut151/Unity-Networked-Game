using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SharedLibrary;

public class PopulateTable : MonoBehaviour
{
    private List<GameObject> _playerRows;

    public int _numOfPlayers = 5;
    public Transform _rowParent;
    public GameObject _playerRow;

    void Start ()
    {
        _playerRows = new List<GameObject>();
        InitPlayerRows();
    }

    void Update()
    {

    }

    private void InitPlayerRows()
    {
        for (int x = 0; x < _numOfPlayers; x++)
        {
            CreateRow();
        }
    }
    private void CreateRow()
    {
        if (_playerRows.Count == 0)
        {
            GameObject row = Instantiate(_playerRow) as GameObject;
            row.transform.SetParent(_rowParent, false);
            _playerRows.Add(row);
        }
        else
        {
            int numOfRows = _playerRows.Count - 1;
            GameObject row = Instantiate(_playerRows[numOfRows]) as GameObject;
            row.transform.SetParent(_rowParent, false);
            _playerRows.Add(row);
        }
    }
    public void PopulateData(LobbyPacket packet)
    {
        for (int x = 0; x < _numOfPlayers; x++)
        {
            _playerRows[x].FindComponentInChildWithTag<Text>("Lobby_Name").text = packet.data[x].playerAddress;
            _playerRows[x].FindComponentInChildWithTag<Text>("Lobby_Ping").text = packet.data[x].ping.ToString();
            _playerRows[x].FindComponentInChildWithTag<Text>("Lobby_Ready").text = packet.data[x].isReady.ToString();
        }
    }
    public void updatePing(long ping)
    {
        GameObject.FindGameObjectWithTag("Finish").GetComponent<Text>().text = ping.ToString();
    }
}

public static class Finder
{
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
}
