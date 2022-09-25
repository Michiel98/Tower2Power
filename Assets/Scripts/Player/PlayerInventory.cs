using UnityEngine;
using TMPro;
using System.Collections;

public enum ResourceType
{
    Ore,
    Lumber,
    Mana,
    Rope,
    RedTower,
    BlueTower
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public GameObject message;

    [Header("Resource Caps")]
    [Range(0, 100)] [SerializeField] int oreCap = 50;
    [Range(0, 100)] [SerializeField] int lumberCap = 50;
    [Range(0, 100)] [SerializeField] int manaCap = 50;
    [Range(0, 100)] [SerializeField] int ropeCap = 50;

    [Header("Map View Inventory Labels")]
    [SerializeField] TMP_Text mapViewOre;
    [SerializeField] TMP_Text mapViewLumber;
    [SerializeField] TMP_Text mapViewMana;
    [SerializeField] TMP_Text mapViewrope;

    [Header("Tower Transfer View Labels")]
    [SerializeField] TMP_Text towerViewOre;
    [SerializeField] TMP_Text towerViewLumber;
    [SerializeField] TMP_Text towerViewRope;

    // inventory resource amounts
    int oreAmount;
    int lumberAmount;
    int manaAmount;
    int ropeAmount;

    // public getters for inventory resource amounts
    public int OreAmount { get { return oreAmount; } }
    public int LumberAmount { get { return lumberAmount; } }
    public int ManaAmount { get { return manaAmount; } }
    public int RopeAmount { get { return ropeAmount; } }

    Tower targetTower; // the current target tower that the player inventory will interact with
    public void SetTargetTower(Tower tower) => this.targetTower = tower;
    public Tower GetTargetTower() => this.targetTower;

    void Awake()
    {
        if (instance != null) GameObject.Destroy(instance);
        else instance = this;
        DontDestroyOnLoad(this);
    }

    void Start() => UpdateLabels();

    // update the labels in the scene to reflect the current inventory status
    void UpdateLabels()
    {
        mapViewOre.text = oreAmount.ToString();
        mapViewMana.text = manaAmount.ToString();
        mapViewrope.text = ropeAmount.ToString();
        mapViewLumber.text = lumberAmount.ToString();

        towerViewOre.text = oreAmount.ToString();
        towerViewRope.text = ropeAmount.ToString();
        towerViewLumber.text = lumberAmount.ToString();
    }

    public void IncreaseSawActions(int amount) => targetTower.AddSawActions(amount);

    public void IncreaseHammerActions(int amount) => targetTower.AddHammerActions(amount);

    // transfer all resources from player inventory to tower
    public void TransferAllToTower()
    {
        // add resources to tower
        targetTower.AddResource(ResourceType.Ore, oreAmount);
        targetTower.AddResource(ResourceType.Rope, ropeAmount);
        targetTower.AddResource(ResourceType.Lumber, lumberAmount);

        // remove resources from inventory
        this.RemoveResource(ResourceType.Ore, oreAmount);
        this.RemoveResource(ResourceType.Rope, ropeAmount);
        this.RemoveResource(ResourceType.Lumber, lumberAmount);
    }

    // transfer a single resource from player inventory to tower
    public void TransferSingleToTower(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Lumber: if (lumberAmount > 0) targetTower.AddResource(type, 1); break;
            case ResourceType.Ore: if (oreAmount > 0) targetTower.AddResource(type, 1); break;
            case ResourceType.Rope: if (ropeAmount > 0) targetTower.AddResource(type, 1); break;
        }
        RemoveResource(type, 1);
    }

    public void TransferLumberToTower() => TransferSingleToTower(ResourceType.Lumber);
    public void TransferOreToTower() => TransferSingleToTower(ResourceType.Ore);
    public void TransferRopeToTower() => TransferSingleToTower(ResourceType.Rope);

    public void stealOre() => stealSingleFromTower(ResourceType.Ore);
    public void StealLumber() => stealSingleFromTower(ResourceType.Lumber);
    public void StealRope() => stealSingleFromTower(ResourceType.Rope);

    public void stealSingleFromTower(ResourceType type)
    {
        if (manaAmount >= 10 && targetTower.GetStoredResourceAmount(type) > 0)
        {
            switch (type)
            {
                case ResourceType.Lumber: targetTower.StealResource(type, 1); break;
                case ResourceType.Ore: targetTower.StealResource(type, 1); break;
                case ResourceType.Rope: targetTower.StealResource(type, 1); break;
            }
            RemoveResource(ResourceType.Mana, 10);
            AddResource(type, 1);
        }
    }

    // add resources to player inventory
    // the resources are clamped between 0 and cap
    public void AddResource(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Ore: oreAmount = Mathf.Clamp(oreAmount + amount, 0, oreCap); break;
            case ResourceType.Lumber: lumberAmount = Mathf.Clamp(lumberAmount + amount, 0, lumberCap); break;
            case ResourceType.Mana: manaAmount = Mathf.Clamp(manaAmount + amount, 0, manaCap); break;
            case ResourceType.Rope: ropeAmount = Mathf.Clamp(ropeAmount + amount, 0, ropeCap); break;
        }
        UpdateLabels();

        //TODO: check CapReached() here and give player visual feedback 
        if (CapReached(resource)) {
            message.SetActive(true);
            StartCoroutine(Wait(5));
        }
    }

    // remove resources from player inventory
    // the resources are clamped between 0 and cap
    public void RemoveResource(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Ore: oreAmount = Mathf.Clamp(oreAmount - amount, 0, oreCap); break;
            case ResourceType.Lumber: lumberAmount = Mathf.Clamp(lumberAmount - amount, 0, lumberCap); break;
            case ResourceType.Mana: manaAmount = Mathf.Clamp(manaAmount - amount, 0, manaCap); break;
            case ResourceType.Rope: ropeAmount = Mathf.Clamp(ropeAmount - amount, 0, ropeCap); break;
        }
        UpdateLabels();
    }

    // returns whether the resource cap was reached
    public bool CapReached(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Ore: return oreAmount >= oreCap;
            case ResourceType.Lumber: return lumberAmount >= lumberCap;
            case ResourceType.Mana: return false;/* return manaAmount >= manaCap;*/
            case ResourceType.Rope: return ropeAmount >= ropeCap;
            default: return true;
        }
    }

    // set certain resource to zero
    public void ResetResource(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Lumber: lumberAmount = 0; break;
            case ResourceType.Mana: manaAmount = 0; break;
            case ResourceType.Rope: ropeAmount = 0; break;
            case ResourceType.Ore: oreAmount = 0; break;
        }
        UpdateLabels();
    }

    // set all resources to zero
    public void ResetAllResources()
    {
        lumberAmount = manaAmount = ropeAmount = oreAmount = 0;
        UpdateLabels();
    }

    public void EnterUpgradeMode() => targetTower.EnterUpgradeMode();

    // debug method for adding resources
    public void DebugAddResources()
    {
        AddResource(ResourceType.Lumber, 50);
        AddResource(ResourceType.Ore, 50);
        AddResource(ResourceType.Mana, 50);
        AddResource(ResourceType.Rope, 50);
    }

    // debug method for adding ugprade actions
    public void DebugAddUpgradeActions()
    {
        targetTower.AddSawActions(50);
        targetTower.AddHammerActions(50);
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        message.SetActive(false);
    }
}