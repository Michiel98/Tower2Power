using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomJoiner : MonoBehaviourPunCallbacks
{
    [Header("Inputs")]
    [SerializeField] private InputField m_roomName;

    [Header("Buttons")]
    [SerializeField] private Button m_joinButton;
    [SerializeField] private Button m_backButton;

    [Header("Feedback")]
    [SerializeField] private Text feedback;

    [Header("UI")]
    [SerializeField] private GameObject joinUI;
    [SerializeField] private GameObject roomUI;
    [SerializeField] private GameObject loading;

    private void Start()
    {
        m_joinButton.onClick.AddListener(delegate { JoinRoom(); });
        loading.SetActive(false);
    }

    public void JoinRoom()
    {
        /* disable inputs */
        EnableButtons(false);
        loading.SetActive(true);

        /* join room */
        PhotonNetwork.JoinRoom(m_roomName.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room. [" + message + "]");
        if (message == "Game does not exist") feedback.text = "Room does not exist.";
        else if (message == "Game full") feedback.text = "Room was full.";
        EnableButtons(true);
        loading.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room. [" + PhotonNetwork.CurrentRoom.Name + "]");
        EnableButtons(true);

        /* switch UI */
        joinUI.SetActive(false);
        roomUI.SetActive(true);
        loading.SetActive(false);
    }

    public void EnableButtons(bool enable)
    {
        m_joinButton.interactable = enable;
        m_backButton.interactable = enable;
    }
}
