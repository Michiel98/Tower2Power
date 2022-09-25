using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }

    private float m_longitude, m_latitude; // longitude and latitude in degrees

    [Header("GPS Settings")]
    public int maximumWaitTime = 5;
    [Tooltip("Desired service accuracy in meters")]
    public float accuracy = 2f;
    [Tooltip("Minimum distance in meters a device must move laterally before Input.location property is updated")]
    public float updateDistance = 2f;

    private void Start()
    {
      if (Instance != null) GameObject.Destroy(Instance);
      else Instance = this;
      DontDestroyOnLoad(this);

      StartCoroutine(LocationService());
    }

    IEnumerator LocationService()
    {
        LocationService service = Input.location;

        // Check if app has location permission
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }

        // Check if user has enabled location
        if (!service.isEnabledByUser)
        {
            ShowLocationDisabled();
            yield break;
        }

        // Start location service
        service.Start(accuracy, updateDistance);

        // Wait for initialization for a limited amount of time
        while (service.status == LocationServiceStatus.Initializing && maximumWaitTime > 0)
        {
            yield return new WaitForSeconds(1);
            maximumWaitTime--;
        }

        // Initialization timed out
        if (maximumWaitTime < 1)
        {
            ShowLocationTimedOut();
            yield break;
        }

        // Location service failed
        if (service.status == LocationServiceStatus.Failed)
        {
            ShowLocationFailed();
            yield break;
        }
    }

    private void Update()
    {
        m_longitude = Input.location.lastData.longitude;
        m_latitude = Input.location.lastData.latitude;
    }

    /* public getters */
    public float f_longitude { get { return m_longitude; } private set { m_longitude = value; } }
    public float f_latitude { get { return m_latitude; } private set { m_latitude = value; } }

    // Show the user a warning that their location is disabled
    private void ShowLocationDisabled()
    {

    }

    // Show the user a warning that the location serivce has failed
    private void ShowLocationTimedOut()
    {

    }

    // Show the user a warning that the location serivce has failed
    private void ShowLocationFailed()
    {

    }
}
