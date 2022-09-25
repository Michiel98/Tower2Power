using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviour
{
    public PhotonView view;
    private int team = 0;

    private GameObject spawnedPlayer;

    private bool teamSet = false;

    private void Start()
    {
        if (view.IsMine)
        {Debug.Log("getteam");
            view.RPC("GetTeam", RpcTarget.MasterClient);
        }
    }

    private void Update()
    {
        if (teamSet == false && team != 0)
        {
            if (team == 1 && view.IsMine)
            {
                //spawnedPlayer = (GameObject)PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
                this.gameObject.name = "Player [Team Red]";
                teamSet = true;
            }

            else if (team == 2 && view.IsMine)
            {
                //spawnedPlayer = (GameObject)PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
                this.gameObject.name = "Player [Team Blue]";
                teamSet = true;
            }
        }
    }

    [PunRPC]
    void GetTeam()
    {
        team = PhotonPlayerManager.instance.GetId();
        view.RPC("SetTeam", RpcTarget.OthersBuffered, team);
    }

    [PunRPC]
    void SetTeam(int team)
    {
        this.team = team;
    }
}
