using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;
using TMPro;

public class TowerAR : MonoBehaviour
{
    // Gyro
    
    private Gyroscope gyro;
    private GameObject cameraContainer;
    private Quaternion rotation;

    // Cam
    private WebCamTexture cam;
    public RawImage background;
    public AspectRatioFitter fit;

    [Header("Resources Labels")]
    [SerializeField] private TMP_Text AROreLabel;
    [SerializeField] private TMP_Text ARLumberLabel;
    [SerializeField] private TMP_Text ARRopeLabel;

    [Header("Towers")]
    public Tower RedTower;
    public Tower BlueTower;

    public GameObject playerChild;

    private bool arReady = false;

    private void Start()
    {
        // Check if we support both services
        // Gyroscope
        if (!SystemInfo.supportsGyroscope)
        {
            Debug.Log("This device does not have a Gyroscope");
            return;
        }

        // Back Camera
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            if (!WebCamTexture.devices[i].isFrontFacing)
            {
                cam = new WebCamTexture(WebCamTexture.devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        // If we did not find a back camera, exit
        if (cam == null)
        {
            Debug.Log("This device does not have a back Camera");
            return;
        }

        // Both services are supported, let's enalbe them
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        //gyro = Input.gyro;
        Input.gyro.enabled = true;
        cameraContainer.transform.rotation = Quaternion.Euler(90f, 0, 0);
        rotation = new Quaternion(0, 0, 1, 0);

        cam.Play();
        background.texture = cam;

        arReady = true;

        labels();
    }


    private void Update()
    {
        if (arReady)
        {
            // Update camera

            float ratio = (float)cam.width / (float)cam.height;
            fit.aspectRatio = ratio;

            float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            // Update gyro
            transform.localRotation = Input.gyro.attitude * rotation;
        }
    }

    private void labels() // set resource labels of enemy tower
    {
        string playerName = playerChild.transform.parent.gameObject.name;

        if (playerName == "Red Team")
        {
            AROreLabel.text = BlueTower.GetStoredResourceAmount(ResourceType.Ore).ToString();
            ARRopeLabel.text = BlueTower.GetStoredResourceAmount(ResourceType.Rope).ToString();
            ARLumberLabel.text = BlueTower.GetStoredResourceAmount(ResourceType.Lumber).ToString();
        }
        else if (playerName == "Blue Team")
        {
            AROreLabel.text = RedTower.GetStoredResourceAmount(ResourceType.Ore).ToString();
            ARRopeLabel.text = RedTower.GetStoredResourceAmount(ResourceType.Rope).ToString();
            ARLumberLabel.text = RedTower.GetStoredResourceAmount(ResourceType.Lumber).ToString();
        }

    }
}