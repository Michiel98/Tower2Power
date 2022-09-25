using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreator : MonoBehaviourPunCallbacks
{
    [Header("Inputs")]
    [SerializeField] private InputField m_roomName;
    [SerializeField] private Slider m_maxPlayers;

    [Header("Buttons")]
    [SerializeField] private Button m_createButton;
    [SerializeField] private Button m_backButton;

    [Header("Feedback")]
    [SerializeField] private Text feedback;

    [Header("UI")]
    [SerializeField] private GameObject createUI;
    [SerializeField] private GameObject roomUI;
    [SerializeField] private GameObject loading;

    private void Start()
    {
        m_createButton.onClick.AddListener(delegate { CreateRoom(); });
        loading.SetActive(false);
    }

    public void CreateRoom()
    {
        /* disable inputs */
        EnableButtons(false);
        loading.SetActive(true);

        /* room options */
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = (byte)(int)m_maxPlayers.value;

        /* create room */
        PhotonNetwork.CreateRoom(m_roomName.text, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room. [" + message + "]");
        if (message == "A game with the specified id already exist.") feedback.text = "A room with that name already exists.";
        EnableButtons(true);
        loading.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Successfully created room. [" + PhotonNetwork.CurrentRoom.Name + "]");
        EnableButtons(true);

        /* switch UI */
        createUI.SetActive(false);
        roomUI.SetActive(true);
        loading.SetActive(false);
    }

    public void EnableButtons(bool enable)
    {
        m_createButton.interactable = enable;
        m_backButton.interactable = enable;
    }
}
