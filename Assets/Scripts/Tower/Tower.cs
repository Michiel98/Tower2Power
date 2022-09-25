using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

[System.Serializable]
public class Level
{
    [Header("Resources")] public int oreCost;
    public int lumbercost;
    public int ropeCost;

    [Header("Labour")] public int sawCost;
    public int hammerCost;
}

[RequireComponent(typeof(PhotonView))]
public class Tower : Photon.Pun.MonoBehaviourPun
{
    [Header("Tower Upgrading")] public Image[] images;
    public Level[] levels; // levels[0] is 'level 1' so the first level you can reach, after you start
    public Button upgradeButton;

    [Header("Tower Levels")] public TMP_Text enemyLevelText;
    public TMP_Text homeLevelText;

    [Header("Stored Resources Labels")] [SerializeField]
    TMP_Text storedOreLabel;

    [SerializeField] TMP_Text storedLumberLabel;
    [SerializeField] TMP_Text storedRopeLabel;
    [SerializeField] TMP_Text storedHammerLabel;
    [SerializeField] TMP_Text storedSawLabel;

    [Header("Stealable Resources Labels")] [SerializeField]
    TMP_Text stealableOreLabel;

    [SerializeField] TMP_Text stealableLumberLabel;
    [SerializeField] TMP_Text stealableRopeLabel;


    [Header("Audio")] public AudioSource upgradeSound;
    public AudioSource enemyUpgradeSound;

    // tower
    int currentTowerLevel = 0;
    int storedOreAmount;
    int storedLumberAmount;
    int storedRopeAmount;
    int receivedSawActions = 0;
    int receivedHammerActions = 0;

    public int StoredOreAmount => storedOreAmount;

    public int StoredLumberAmount => storedLumberAmount;

    public int StoredRopeAmount => storedRopeAmount;

    public int currentLevel => currentTowerLevel;

    public ResourceType GetResourceType() => towerType;

    // multiplayer
    PhotonView view;

    public int GetStoredResourceAmount(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Lumber: return storedLumberAmount;
            case ResourceType.Ore: return storedOreAmount;
            case ResourceType.Rope: return storedRopeAmount;
        }

        return 0;
    }

    // tower upgrade costs
    public int GetNextLevelOreCost() => levels[currentTowerLevel].oreCost;
    public int GetNextLevelLumberCost() => levels[currentTowerLevel].lumbercost;
    public int GetNextLevelRopeCost() => levels[currentTowerLevel].ropeCost;

    bool towerUpgradeMode = false;

    ResourceType towerType;
    GameObject[] players;
    GameObject player;
    PhotonView playerView;

    void Update()
    {
        if (towerUpgradeMode && receivedHammerActions >= levels[currentTowerLevel].hammerCost &&
            receivedSawActions >= levels[currentTowerLevel].sawCost) UpgradeTower();
    }

    void Start()
    {
        foreach (Image i in images) i.enabled = false; // disable tower images
        images[0].enabled = true; // enable base level image
        upgradeButton.interactable = false; // disabling upgrading at start
        SetLabels(false); // initialize all labels

        // set type of tower
        if (gameObject.name == "Red Tower") towerType = ResourceType.RedTower;
        else if (gameObject.name == "Blue Tower") towerType = ResourceType.BlueTower;

        GetComponent<ResourceScript>().SetType(towerType);

        view = GetComponent<PhotonView>();
        
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (!p.name.EndsWith("Team")) continue;
            playerView = p.GetComponent<PhotonView>();
            player = p;
        }
    }

    public void EnterUpgradeMode() => towerUpgradeMode = true;

    void UpgradeTower()
    {
        view.RPC("UpgradeTowerRPC", RpcTarget.AllBuffered, this.towerType);

        towerUpgradeMode = false; // leave upgrade mode since we just upgraded the tower
        
        view.RPC("ResetActionsRPC", RpcTarget.AllBuffered, this.towerType);
        
        if (currentTowerLevel == levels.Length - 1)
        {
            view.RPC("WonGameRPC", RpcTarget.AllBuffered, this.towerType);
            return;
        }

        upgradeButton.interactable = false; // don't allow upgrading twice

        Level nextLevel = levels[currentTowerLevel];
        
        // remove resources that were spent on upgrade, do this for all clients
        view.RPC("RemoveResourceRPC", RpcTarget.AllBuffered, ResourceType.Ore, nextLevel.oreCost, this.towerType);
        view.RPC("RemoveResourceRPC", RpcTarget.AllBuffered, ResourceType.Rope, nextLevel.ropeCost, this.towerType);
        view.RPC("RemoveResourceRPC", RpcTarget.AllBuffered, ResourceType.Lumber, nextLevel.lumbercost, this.towerType);

        // increase tower level, do this for all clients
        view.RPC("IncreaseLevelRPC", RpcTarget.AllBuffered, this.towerType);
    }
    
    public void AddSawActions(int amount)
    {
        view.RPC("AddSawActionRPC", RpcTarget.AllBuffered, amount, this.towerType);
    }

    public void AddHammerActions(int amount)
    {
        view.RPC("AddHammerActionRPC", RpcTarget.AllBuffered, amount, this.towerType);
    }

    public void AddResource(ResourceType type, int amount)
    {
        view.RPC("AddResourceRPC", RpcTarget.AllBuffered, type, amount, this.towerType);
    }

    public void StealResource(ResourceType type, int amount)
    {
        view.RPC("RemoveResourceRPC", RpcTarget.AllBuffered, type, amount, this.towerType);
    }

    void CheckUpgradeCost()
    {
        Level nextLevel = levels[currentTowerLevel];
        if (storedOreAmount < nextLevel.oreCost || storedLumberAmount < nextLevel.lumbercost ||
            storedRopeAmount < nextLevel.ropeCost) upgradeButton.interactable = false;
        else if (storedOreAmount >= nextLevel.oreCost && storedLumberAmount >= nextLevel.lumbercost &&
                 storedRopeAmount >= nextLevel.ropeCost) upgradeButton.interactable = true;
    }

    public void SetLabels(bool playerspecific) // only for home
    {
        if (playerspecific)
        {
            switch (towerType)
            {
                case ResourceType.BlueTower:
                    if (player.name != "Blue Team" &&
                        PlayerInventory.instance.GetTargetTower().GetResourceType() != ResourceType.BlueTower) return;
                    else if (player.name == "Blue Team" &&
                             PlayerInventory.instance.GetTargetTower().GetResourceType() == ResourceType.RedTower) return;
                    break;

                case ResourceType.RedTower:
                    if (player.name != "Red Team" &&
                        PlayerInventory.instance.GetTargetTower().GetResourceType() != ResourceType.RedTower) return;
                    else if (player.name == "Red Team" &&
                             PlayerInventory.instance.GetTargetTower().GetResourceType() == ResourceType.BlueTower) return;
                    break;
            }
        }

        CheckUpgradeCost();

        // tower resources
        storedOreLabel.text = storedOreAmount + " / " + levels[currentTowerLevel].oreCost;
        storedRopeLabel.text = storedRopeAmount + " / " + levels[currentTowerLevel].ropeCost;
        storedLumberLabel.text = storedLumberAmount + " / " + levels[currentTowerLevel].lumbercost;

        // steal resources
        stealableOreLabel.text = storedOreAmount.ToString();
        stealableRopeLabel.text = storedRopeAmount.ToString();
        stealableLumberLabel.text = storedLumberAmount.ToString();

        // upgrade actions
        storedHammerLabel.text = $"{receivedHammerActions} / {levels[currentTowerLevel].hammerCost}";
        storedSawLabel.text = receivedSawActions + " / " + levels[currentTowerLevel].sawCost.ToString();

        // level
        homeLevelText.text = currentTowerLevel.ToString();
        enemyLevelText.text = currentTowerLevel.ToString();
    }

    [PunRPC] // increase level of tower
    void IncreaseLevelRPC(ResourceType tower)
    {
        Debug.Log("IncreaseLevelRPC");
        Debug.Log("Instructed Tower: " + towerType + "   Received Tower: " + tower);
        if (towerType != tower) return;
        currentTowerLevel += 1;
        SetLabels(true);
    }

    [PunRPC] // add stored resources
    void AddResourceRPC(ResourceType type, int amount, ResourceType tower)
    {
        Debug.Log("AddResourceRPC");
        Debug.Log("Instructed Tower: " + towerType + "   Received Tower: " + tower);
        if (towerType != tower) return;
        switch (type)
        {
            case ResourceType.Lumber:
                storedLumberAmount += amount;
                break;
            case ResourceType.Ore:
                storedOreAmount += amount;
                break;
            case ResourceType.Rope:
                storedRopeAmount += amount;
                break;
        }

        SetLabels(true);
    }

    [PunRPC] // remove stored resources
    void RemoveResourceRPC(ResourceType type, int amount, ResourceType tower)
    {
        Debug.Log("RemoveResourceRPC");
        Debug.Log("Instructed Tower: " + towerType + "   Received Tower: " + tower);
        if (towerType != tower) return;
        switch (type)
        {
            case ResourceType.Lumber:
                storedLumberAmount -= amount;
                break;
            case ResourceType.Ore:
                storedOreAmount -= amount;
                break;
            case ResourceType.Rope:
                storedRopeAmount -= amount;
                break;
        }

        SetLabels(true);
    }

    [PunRPC] // reset actions
    void ResetActionsRPC(ResourceType tower)
    {
        Debug.Log("ResetActionsRPC");
        Debug.Log("Instructed Tower: " + towerType + "   Received Tower: " + tower);
        if (towerType != tower) return;
        receivedSawActions = 0;
        receivedHammerActions = 0;
        SetLabels(true);
    }

    [PunRPC] // add saw action
    void AddSawActionRPC(int amount, ResourceType tower)
    {
        Debug.Log("AddSawActionRPC");
        Debug.Log("Instructed Tower: " + towerType + "   Received Tower: " + tower);
        if (towerType != tower) return;
        receivedSawActions += amount;
        SetLabels(true);
    }

    [PunRPC] // add hammer action
    void AddHammerActionRPC(int amount, ResourceType tower)
    {
        Debug.Log("AddHammerActionRPC");
        Debug.Log("Instructed Tower: " + towerType + "   Received Tower: " + tower);
        if (towerType != tower) return;
        receivedHammerActions += amount;
        SetLabels(true);
    }

    [PunRPC]
    void UpgradeTowerRPC(ResourceType tower)
    {
        switch (tower)
        {
            case ResourceType.BlueTower:
                if (towerType == ResourceType.BlueTower)
                {
                    switch (player.name)
                    {
                        case "Blue Team":
                            UISwitcher.instance.LeaveTowerUpgradeUI();
                            upgradeSound.Play();
                            images[currentTowerLevel + 1].enabled = true; // add new image to tower
                            break;
                        case "Red Team":
                            enemyUpgradeSound.Play();
                            break;
                    }
                }
                break;

            case ResourceType.RedTower:
                if (this.towerType == ResourceType.RedTower)
                {
                    if (player.name == "Red Team")
                    {
                        UISwitcher.instance.LeaveTowerUpgradeUI();
                        upgradeSound.Play();
                        images[currentTowerLevel + 1].enabled = true; // add new image to tower
                    }
                    else if (player.name == "Blue Team")
                    {
                        enemyUpgradeSound.Play();
                    }
                }

                break;
        }
    }

    [PunRPC] // some tower won the game
    void WonGameRPC(ResourceType tower)
    {
        switch (tower)
        {
            case ResourceType.BlueTower: // a blue tower has won
                if (towerType == ResourceType.BlueTower) // let blue tower set the win/lose screens
                {
                    if (player.name == "Blue Team") WonGame(); // blue team wins
                    else LostGame(); // red team loses
                }
                break;
            
            case ResourceType.RedTower: // a red tower has won
                if (towerType == ResourceType.RedTower) // let red tower set the win/lose screens
                {
                    if (player.name == "Red Team") WonGame(); // red team wins
                    else LostGame(); // red team loses
                }
                break;
        }
    }

    void WonGame() => UISwitcher.instance.EnterWinUI();
    void LostGame() => UISwitcher.instance.EnterLoseUI();

    public void playerInactive()
    {
        player.SetActive(false);
    }

    public void playerActive()
    {
        player.SetActive(true);
    }
}