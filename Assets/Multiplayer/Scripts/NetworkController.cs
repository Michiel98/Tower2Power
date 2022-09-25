using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* script used to connect to photon server */
public class NetworkController : MonoBehaviourPunCallbacks
{
    [Header("Network Settings")]
    [SerializeField] int sendRate = 20;
    [SerializeField] int serializationRate = 5;
    [SerializeField] bool autoSyncScene = true;

    private void Start()
    {
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializationRate;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = autoSyncScene;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to " + PhotonNetwork.CloudRegion + " server.");
    }

    public override void OnDisconnected (DisconnectCause cause)
    {
        Debug.Log("Failed to connect. [" + cause.ToString() + "]");
    }
}