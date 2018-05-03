using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SharedLibrary;

public class OtherPlayers : MonoBehaviour
{
    public GameObject avatarPrefab;
    public GameObject localPlayer;

	void Start ()
    {
		
	}
	void Update ()
    {
        ProcessServerOtherPlayerData();
    }

    void ProcessServerOtherPlayerData()
    {
        AvatarData[] players = PlayersData.GetAllPlayers();

        for (int x = 0; x < players.Length; x++)
        {
            if (!players[x].GetUsername().Contains(Commons.defaultName))
            {
                if (players[x].GetUsername().Equals((PlayersData.GetLocalPlayerData().GetUsername())))
                {
                    ProcessLocalPlayer(players[x]);
                }
                else
                {
                    if (players[x].GetAvatar() == null)
                    {
                        ProcessNullPlayer(players[x]);
                    }

                    ProcessOtherPlayers(players[x]);
                }
            }
        }
    }

    private void ProcessNullPlayer(AvatarData player)
    {
        Vector3 start = Vector3.zero;
        start.x += 10.0f;
        start.y += 10.0f;

        player.SetAvatar(Instantiate(avatarPrefab, start, Quaternion.identity));
    }

    private void ProcessLocalPlayer(AvatarData player)
    {
        Vec3 serverPos = player.GetPosition();
        Vec4 serverRot = player.GetRotation();

        Vector3 pos = new Vector3(serverPos.x, serverPos.y, serverPos.z);
        Vector3 rot = new Vector3(serverRot.x, serverRot.y, serverRot.z);

        float dist = Vector3.Distance(pos, localPlayer.transform.position);

        if (dist > 100.0)
        {
            localPlayer.transform.position = pos;
            localPlayer.transform.rotation = Quaternion.Euler(rot);
        }
    }

    private void ProcessOtherPlayers(AvatarData player)
    {
        Vec3 serverPos = player.GetPosition();

        Vector3 pos = new Vector3(serverPos.x, serverPos.y, serverPos.z);

        Vector3 newPos = Lerp(player.GetAvatar().transform.position, pos, 1.0f * Time.deltaTime);
        newPos.y = 0.0f;
        Quaternion newRot = Quaternion.LookRotation(newPos - player.GetAvatar().transform.position);

        player.GetAvatar().transform.position = newPos;
        player.GetAvatar().transform.rotation = newRot;
    }

    private Vector3 Lerp(Vector3 start, Vector3 end, float percent)
    {
        return (start + percent * (end - start));
    }
}
