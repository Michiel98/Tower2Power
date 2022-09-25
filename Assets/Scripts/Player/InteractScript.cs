using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class InteractScript : MonoBehaviour
{
    [SerializeField] HandleTouch touchScript;
    
    [Header("Gather Mode UI")]
    [SerializeField] GameObject oreResource;
    [SerializeField] GameObject ropeResource;
    [SerializeField] GameObject lumberResource;
    [SerializeField] TMP_Text remainingOreLabel;
    [SerializeField] TMP_Text remainingRopeLabel;
    [SerializeField] TMP_Text remainingLumberLabel;

    [Header("Gather Mode Instructions")]
    [TextArea] [SerializeField] string oreGestureInstruction;
    [TextArea] [SerializeField] string ropeGestureInstruction;
    [TextArea] [SerializeField] string lumberGestureInstruction;
    [SerializeField] TMP_Text gatherInstructionText;

    [Header("Gather Mode Audio Source")]
    [SerializeField] AudioSource lumberAudio;
    [SerializeField] AudioSource oreAudio;
    [SerializeField] AudioSource ropeAudio;
    [SerializeField] AudioSource resourceDepletedAudio;

    [Header("Towers")]
    [SerializeField] Tower redTower;
    [SerializeField] Tower blueTower;
    
    // interacted object
    GameObject interactedObject; // the gameobject we tapped on
    ResourceScript interactedResource; // the resource script of the gameobject we tapped on

    // gesture detection
    float waitTime;
    IEnumerator gestureDetection;
    bool wait = false;
    bool step0 = true;
    bool step1 = false;
    bool step2 = false;
    bool step3 = false;


    // called when tapping on an object
    public void Interact(GameObject other)
    {
        interactedObject = other;

        // interacted with resource object
        if (!other.TryGetComponent<ResourceScript>(out interactedResource)) {}//Debug.Log("Tapped object did not have the <ResourceScript> component attached.");
        else if (interactedResource.IsInteractable)
        {
            ResourceType type = interactedResource.Type;
            if (type == ResourceType.RedTower || type == ResourceType.BlueTower) InteractWithTower(type); // we clicked on a tower
            else
            {
                InteractWithResource(type); // we clicked on a resource
            }
            touchScript.enabled = false; // disable touch input
        }
    }

    public void InteractWithResource(ResourceType type)
    {
        UISwitcher.instance.EnterGatherUI();

        switch (type)
        {
            case ResourceType.Lumber:
                Input.gyro.enabled = true;
                lumberResource.SetActive(true); oreResource.SetActive(false); ropeResource.SetActive(false); // enable resource UI
                remainingLumberLabel.text = interactedResource.Richness.ToString(); // show resource richness
                gatherInstructionText.text = lumberGestureInstruction; // set instruction text
                break;

            case ResourceType.Ore:
                lumberResource.SetActive(false); oreResource.SetActive(true); ropeResource.SetActive(false); // enable resource UI
                remainingOreLabel.text = interactedResource.Richness.ToString(); // show resource richness
                gatherInstructionText.text = oreGestureInstruction; // set instruction text
                break;

            case ResourceType.Rope:
                lumberResource.SetActive(false); oreResource.SetActive(false); ropeResource.SetActive(true); // enable resource UI
                remainingRopeLabel.text = interactedResource.Richness.ToString(); // show resource richness
                gatherInstructionText.text = ropeGestureInstruction; // set instruction text
                break;
        }

        StartGestureDetection(type); // start gesture detection
    }

    public void AddResourceToPlayer(ResourceType type)
    {
        interactedResource.DecrementRichness(); // remove unit from resource

        PlayerInventory.instance.AddResource(type, 1); // add resource to player inventory

        // update label to reflect remaining resource amount
        switch (type)
        {
            case ResourceType.Lumber: remainingLumberLabel.text = interactedResource.Richness.ToString(); break;
            case ResourceType.Ore: remainingOreLabel.text = interactedResource.Richness.ToString(); break;
            case ResourceType.Rope: remainingRopeLabel.text = interactedResource.Richness.ToString(); break;
        }

        if (interactedResource.Richness <= 0) // if we depleted the resource
        {
            // spawn new resource to compensate for resource we just depleted
            switch (type)
            {
                case ResourceType.Lumber: ResourceSpawner.instance.CreateLumberObject(); break;
                case ResourceType.Ore: ResourceSpawner.instance.CreateOreObject(); break;
                case ResourceType.Rope: ResourceSpawner.instance.CreateRopeObject(); break;
            }

            resourceDepletedAudio.Play(); // play resource depleted audio
            LeaveGatherMode(); // stop gesture detection and switch UI, also re-enable touch input
            Destroy(interactedObject.gameObject); // despawn the depleted resource
        }
    }

    //TODO: make the tower initialize themselves through resource script! so they need to set their type themselves to red/blue tower
    public void InteractWithTower(ResourceType type)
    {
        UISwitcher.instance.HideUpgradeInstructionUI();

        PhotonView view = transform.parent.GetComponent<PhotonView>();
        string playerName = transform.parent.gameObject.name;

        Debug.Log("Player with name [" + playerName + "] clicked on tower");

        if (type == ResourceType.RedTower) // red tower
        {
            if (view.IsMine && playerName == "Red Team") // visiting home tower
            {
                PlayerInventory.instance.SetTargetTower(redTower); // enable resource transfer to red tower
                UISwitcher.instance.EnterHomeTowerUI(Color.red);
                redTower.SetLabels(false);
            }

            else if (view.IsMine && playerName == "Blue Team") // visiting enemy tower
            {
                PlayerInventory.instance.SetTargetTower(redTower); // enable resource transfer to red tower
                UISwitcher.instance.EnterEnemyTowerUI(Color.red);
                redTower.SetLabels(false);
            }
        }

        else if (type == ResourceType.BlueTower) // blue tower
        {
            if (view.IsMine && playerName == "Blue Team") // visiting home tower
            {
                PlayerInventory.instance.SetTargetTower(blueTower); // enable resource transfer to blue tower
                UISwitcher.instance.EnterHomeTowerUI(Color.blue);
                blueTower.SetLabels(false);
            }

            else if (view.IsMine && playerName == "Red Team") // visiting enemy tower
            {
                PlayerInventory.instance.SetTargetTower(blueTower); // enable resource transfer to blue tower
                UISwitcher.instance.EnterEnemyTowerUI(Color.blue);
                blueTower.SetLabels(false);
            }
        }
    }

    public void StartGestureDetection(ResourceType type)
    {
        gestureDetection = Recognise(type);
        StartCoroutine(gestureDetection);
    }

    public void LeaveGatherMode()
    {
        StopCoroutine(gestureDetection);
        UISwitcher.instance.LeaveGatherUI();
        touchScript.enabled = true;
    }

    public IEnumerator Recognise(ResourceType type)
    {
        while (true)
        {
            if (Input.touchCount >= 1)
            {
                switch (type)
                {
                    case ResourceType.Lumber: if (RecogniseLumberGesture()) AddResourceToPlayer(ResourceType.Lumber); break;
                    case ResourceType.Ore: if (RecogniseOreGesture()) AddResourceToPlayer(ResourceType.Ore); break;
                    case ResourceType.Rope: if (RecogniseRopeGesture()) AddResourceToPlayer(ResourceType.Rope); break;
                }
            }
            yield return null;
        }
    }

    bool RecogniseLumberGesture()
    {
        waitTime = 0.5f;

        if (Input.gyro.rotationRate.z > 3 && Mathf.Abs(Input.gyro.attitude.x) + Mathf.Abs(Input.gyro.attitude.y) < 0.3 && Input.acceleration.x > 2)
        {
            if (!wait)
            {
                Debug.Log("Recognised gesture [Lumber].");
                StartCoroutine(Wait1(waitTime));
                lumberAudio.Play();
                //Handheld.Vibrate();
                return true;
            }
        }
        return false;
    }

    bool RecogniseOreGesture()
    {
        waitTime = 0.5f;
        if (Input.touchCount >= 1)
        {
            if ((Input.acceleration.y < -0.8 || Input.compass.rawVector.z < 0) && !step1/*((Mathf.Abs(Input.gyro.attitude.z) + Mathf.Abs(Input.gyro.attitude.w) < 0.7 && Mathf.Abs(Input.gyro.attitude.z) + Mathf.Abs(Input.gyro.attitude.w) > 0.3))*/)
            {
                step2 = false;
                if (!wait)
                {
                    StartCoroutine(Wait1(waitTime));
                    step1 = true;
                }
            }
            else if ((/*Input.acceleration.z < -2 ||*/ Input.acceleration.y > 2 || Input.gyro.rotationRate.x < -10) && step0 == true && step1 == true) { 
                step2 = true;
                step3 = true;
                if (step1 && step2 && step0)
                {
                    step1 = false;
                    step2 = false;
                    step0 = false;
                    Debug.Log("Recognised gesture [Ore].");
                    StartCoroutine(wait2(0.5f));
                    oreAudio.Play();
                    //Handheld.Vibrate();
                    return true;
                }
            }
        }
        return false;
    }

    bool RecogniseRopeGesture()
    {
        waitTime = 1f;
        if (Input.acceleration.z < -4)
        {
            if (!wait)
            {
                Debug.Log("Recognised gesture [Rope].");
                StartCoroutine(Wait1(waitTime));
                step1 = true;
            }
        }
        else if (Input.acceleration.x < -2)
        {
            step2 = true;
            if (step1 && step2 && step0)
            {
                step1 = false;
                step2 = false;
                step0 = false;
                StartCoroutine(wait2(1));
                ropeAudio.Play();
                //Handheld.Vibrate();
                return true;
            }
        }
        return false;
    }

    //Set the frequency at which the gestures are recognised
    IEnumerator Wait1(float waitTime)
    {
        wait = true;
        yield return new WaitForSeconds(waitTime);
        wait = false;
    }

    IEnumerator wait2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        step1 = false;
        step2 = false;
        step3 = false;
        step0 = true;
    }
}