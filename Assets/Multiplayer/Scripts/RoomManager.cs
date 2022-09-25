using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance { set; get; }

    [SerializeField] private int gameScene;

    private bool m_randomTeams;
    private string m_team;
    List<string> m_teams = new List<string> { "Red Team", "Blue Team" };

    [Header("Buttons")]
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_leaveButton;

    [Header("Room Info")]
    [SerializeField] private Text m_roomName;
    [SerializeField] private Text m_maxPlayers;
    [SerializeField] private Text m_serverAddress;

    [Header("Player List")]
    [SerializeField] private GameObject m_playerList;
    [SerializeField] private GameObject m_playerListingPrefab;

    [Header("Teams")]
    [SerializeField] private Toggle m_randomTeamsToggle;
    [SerializeField] private Dropdown m_teamsDropdown;
    [SerializeField] private Image m_background;

    [Header("Scene Transition")]
     [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup canvasGroup;

    private List<PlayerListing> m_playerListings = new List<PlayerListing>();

    private int currentScene;

    void Awake()
    {
        if (instance != null) GameObject.Destroy(instance);
        else instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
       // view = GetComponent<PhotonView>();

        m_startButton.gameObject.SetActive(false);
        m_teamsDropdown.gameObject.SetActive(false);

        m_startButton.onClick.AddListener(delegate { StartGame(); });
        m_leaveButton.onClick.AddListener(delegate { LeaveRoom(); });

        // TEAMS
        m_randomTeamsToggle.onValueChanged.AddListener(delegate { SetTeamMethod(m_randomTeamsToggle); });

        m_teamsDropdown.onValueChanged.AddListener(delegate { SetTeam(m_teamsDropdown); });
        m_teamsDropdown.ClearOptions();
        m_teamsDropdown.AddOptions(m_teams);
        m_background.color = new Color(1f, 0.61f, 0.61f); // red team by default

    }

    void SetTeam(Dropdown dropdown)
    {
//TODO: don't allow everybody having the same
        SetTeam(dropdown.options[dropdown.value].text);
    }

    void SetTeam(string team)
    {
        m_team = team;
        Debug.Log("Set Team [" + m_team + "]");
        if (m_team == "Red Team") m_background.color = new Color(1f, 0.61f, 0.61f);
        else if (m_team == "Blue Team") m_background.color = new Color(0.57f, 0.85f, 1f);
    }


    void SetTeamMethod(Toggle toggle) => m_randomTeams = toggle.isOn;

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public override void OnPlayerEnteredRoom(Player player) => PlayerJoinedRoom(player);
    public override void OnPlayerLeftRoom(Player player) => PlayerLeftRoom(player);

    public override void OnJoinedRoom()
    {
        /* room information */
        m_roomName.text = "<color=red>[RoomName]</color> " + PhotonNetwork.CurrentRoom.Name;
        m_maxPlayers.text = "<color=red>[MaxPlayers]</color> " + PhotonNetwork.CurrentRoom.MaxPlayers;
        m_serverAddress.text = "<color=red>[ServerAddress]</color> " + PhotonNetwork.ServerAddress;

        // set random team from start
        SetTeam(PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0 ? "Red Team" : "Blue Team");
        m_teamsDropdown.value = m_team == "Red Team" ? 0 : 1;

        if (m_randomTeams) m_teamsDropdown.gameObject.SetActive(false); // players can't change
        else // players can change
        {
            m_teamsDropdown.gameObject.SetActive(true);
        }

        /* player list */
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) PlayerJoinedRoom(players[i]);

        /* show start button to master client only */
        m_startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }




    private void StartGame() => StartCoroutine(TransitionToMapView());

 IEnumerator TransitionToMapView()
    {
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(1, 1));

        PhotonNetwork.LoadLevel(gameScene);
        float progress = 0;

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress / 0.9f);
            if (progressBar) progressBar.value = progress;
            if (progressText) progressText.text = Mathf.Round(progress * 100) + "%";
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(0, 1));
        if(loadingScreen != null) loadingScreen.SetActive(false);
    }

    IEnumerator FadeLoadingScreen(float target, float duration)
    {
        float startValue = canvasGroup == null ? 0 : canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            if(canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(startValue, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if(canvasGroup != null) canvasGroup.alpha = target;
    }



    private void PlayerJoinedRoom(Player player)
    {
        if (player == null) return;
        PlayerLeftRoom(player);

        GameObject go = Instantiate(m_playerListingPrefab);
        go.transform.SetParent(m_playerList.transform, false);

        PlayerListing listing = go.GetComponent<PlayerListing>();
        listing.SetPlayerName(player);

        m_playerListings.Add(listing);
    }

    private void PlayerLeftRoom(Player player)
    {
        int index = m_playerListings.FindIndex(x => x.GetName() == player.NickName);

        if (index != -1 && SceneManager.GetActiveScene().buildIndex != gameScene)
        {
            Destroy(m_playerListings[index].gameObject);
            m_playerListings.RemoveAt(index);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == gameScene) SpawnPlayer();
    }



    private void SpawnPlayer()
    {
        Debug.Log("Created Player");
      //  GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)), Quaternion.identity, 0);
       GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        player.name = m_team;
    }
}
