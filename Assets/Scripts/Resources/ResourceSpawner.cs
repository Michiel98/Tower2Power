using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public static ResourceSpawner instance;

    [Header("Parent GameObject")]
    public GameObject spawnedResources;

    [Header("Spawned Prefabs")]
    [SerializeField] GameObject lumberPrefab;
    [SerializeField] GameObject orePrefab;
    [SerializeField] GameObject ropePrefab;
    [SerializeField] GameObject manaPrefab;

    [Header("Amount of Spawns")]
    [SerializeField] int numberOfLumberResources = 10;
    [SerializeField] int numberOfOreResources = 10;
    [SerializeField] int numberOfRopeResources = 10;
    [SerializeField] int numberOfManaResources = 1000;

    [Header("Resource Richness")]
    [SerializeField] int minLumberRichness = 1;
    [SerializeField] int maxLumberRichness = 10;
    [SerializeField] int minOreRichness = 1;
    [SerializeField] int maxOreRichness = 10;
    [SerializeField] int minRopeRichness = 1;
    [SerializeField] int maxRopeRichness = 10;
    [SerializeField] int minManaRichness = 1;
    [SerializeField] int maxManaRichness = 10;

    // the dimensions of the map
    private static readonly float[] mapStart = new float[] { 0f, 0f };
    private static readonly float[] mapEnd = new float[] { 40f, 150f };


    void Start() => InitializeResources();

    void Awake()
    {
        if (instance != null) GameObject.Destroy(instance);
        else instance = this;
        DontDestroyOnLoad(this);
    }

    // create all necessary resource objects
    void InitializeResources()
    {

        for (int i = 0; i <= numberOfLumberResources - 1; i++) CreateLumberObject();
        for (int i = 0; i <= numberOfOreResources - 1; i++) CreateOreObject();
        for (int i = 0; i <= numberOfRopeResources - 1; i++) CreateRopeObject();
        for (int i = 0; i <= numberOfManaResources - 1; i++) CreateManaObject();
    }

    void InstantiatePrefab(GameObject prefab, int amount, int max, ResourceType type, float height)
    {
        GameObject spawnedResource = Instantiate(prefab, new Vector3(Random.Range(mapStart[0], mapEnd[0]), height, Random.Range(mapStart[1], mapEnd[1])), Quaternion.identity);

        // initialize resource properties
        spawnedResource.GetComponent<ResourceScript>().SetRichness(amount);
        spawnedResource.GetComponent<ResourceScript>().SetType(type);
        spawnedResource.GetComponent<MeshRenderer>().enabled = type == ResourceType.Mana; // only mana resources are visible from the start

        // set resource scale and rotation
        if (type == ResourceType.Mana)
            spawnedResource.transform.localScale *= (0.4f); // Smaller scale for mana 
        else
            spawnedResource.transform.localScale *= (1 + ((float)amount / (float)max) * 2f); // scale with resource richness
        spawnedResource.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0f); // random rotation between 0 and 360 degrees

        spawnedResource.transform.parent = spawnedResources.transform; // link the resource to a parent object that will hold all resources
    }

    public void CreateLumberObject() => InstantiatePrefab(lumberPrefab, Random.Range(minLumberRichness, maxLumberRichness), maxLumberRichness, ResourceType.Lumber, 0f);
    public void CreateOreObject() => InstantiatePrefab(orePrefab, Random.Range(minOreRichness, maxOreRichness), maxOreRichness, ResourceType.Ore, 0f);
    public void CreateRopeObject() => InstantiatePrefab(ropePrefab, Random.Range(minRopeRichness, maxRopeRichness), maxRopeRichness, ResourceType.Rope, 0f);
    public void CreateManaObject() => InstantiatePrefab(manaPrefab, Random.Range(minManaRichness, maxManaRichness), maxManaRichness, ResourceType.Mana, 1f);
}
