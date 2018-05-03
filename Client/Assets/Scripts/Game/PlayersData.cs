using UnityEngine;

static public class PlayersData
{
    static private int numOfPlayers = 5;
    static private AvatarData[] _players = new AvatarData[numOfPlayers];
    static private AvatarData _localPlayer = new AvatarData();

    static public void SetAllPlayers(AvatarData[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            _players[i] = players[i];
        }
    }

    static public AvatarData[] GetAllPlayers()
    {
        return _players;
    }

    static public void SetLocalPlayerData(AvatarData player)
    {
        _localPlayer = player;
    }

    static public AvatarData GetLocalPlayerData()
    {
        return _localPlayer;
    }

    static public int GetNumOfPlayers()
    {
        return numOfPlayers;
    }
}
