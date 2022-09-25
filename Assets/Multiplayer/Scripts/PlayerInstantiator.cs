using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.IO;
using UnityEngine;

public class PlayerInstantiator : MonoBehaviour
{
    void Start()
    {
     /*   var id = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        Debug.Log(id);
        GameObject player = (GameObject)PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        player.GetComponent<TeamMember>().SetTeamID(id % 2);
        if (id % 2 == 0) player.name += " [Team Red]";
        else if (id % 2 == 1) player.name += " [Team Blue]";*/
    }
}
