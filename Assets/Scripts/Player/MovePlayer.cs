
using System.Net;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class MovePlayer : MonoBehaviour
{
    /*Park belle vue
    50.877941, 4.719764
    50.873926, 4.719099
    delta LAT = 0.004015 => lenght map = 400
    delta LONG = 0.000665 => width map = 70
     */

    // Coordinates corners on real world, notation {long,lat}
    private static readonly float[] parkStart = new float[] { 4.719399f, 50.873926f };
    private static readonly float[] parkEnd = new float[] { 4.719752f, 50.87652f };

    // Coordinates corners on map, notation {long,lat}
    private static readonly float[] mapStart = new float[] { 0f, 0f };
    private static readonly float[] mapEnd = new float[] { 40f, 150f };

    private float GPSX;
    private float GPSY;
    private float GPSX_old;
    private float GPSY_old;
    private float angle;
    private bool wait;

    private int windowSize = 50;
    private float[] sampleWindowSin;
    private float[] sampleWindowCos;
    private float samplesTotalSin = 0;
    private float samplesTotalCos = 0;

    public Animator animator;

    void Start()
    {
        sampleWindowSin = new float[windowSize];
        sampleWindowCos = new float[windowSize];
        wait = false;
    }

    void FixedUpdate()
    {
            GPSX = GPS.Instance.f_longitude;
            GPSY = GPS.Instance.f_latitude;
            //GPSX = 4.7195f;
            //GPSY = 50.876f;

        if ((float)System.Math.Round(GPSX, 5) != GPSX_old || (float)System.Math.Round(GPSY, 5) != GPSY_old)
        {
            //Set animation to isWalking
            animator.SetBool("isWalking", true);
            StopCoroutine(Wait(1));

            //update old GPS
            GPSX_old = (float)System.Math.Round(GPSX, 5);
            GPSY_old = (float)System.Math.Round(GPSY, 5);
        }
        else
        {
            if (!wait)
            {
                StartCoroutine(Wait(1));
            }
        }

        angle = MovingAverage(Compass.Instance.f_trueHeading * Mathf.Deg2Rad);
    }

    void Update()
    {
        transform.position = new Vector3((GPSX - parkStart[0]) / (parkEnd[0] - parkStart[0]) * (mapEnd[0] - mapStart[0]), 0.5f, (GPSY - parkStart[1]) / (parkEnd[1] - parkStart[1]) * (mapEnd[1] - mapStart[1]));
        transform.eulerAngles = new Vector3(0, angle, 0);
    }

    float MovingAverage(float rawAngle)
    {
        samplesTotalSin = samplesTotalSin - sampleWindowSin[0] + Mathf.Sin(rawAngle);
        samplesTotalCos = samplesTotalCos - sampleWindowCos[0] + Mathf.Cos(rawAngle);

        for (int i = 0; i <= windowSize - 2; i++)
        {
            sampleWindowSin[i] = sampleWindowSin[i + 1];
            sampleWindowCos[i] = sampleWindowCos[i + 1];
        }
        sampleWindowSin[windowSize - 1] = Mathf.Sin(rawAngle);
        sampleWindowCos[windowSize - 1] = Mathf.Cos(rawAngle);

        float averageSin = samplesTotalSin / windowSize;
        float averageCos = samplesTotalCos / windowSize;

        float averageAngle = Mathf.Atan2(averageSin, averageCos) * Mathf.Rad2Deg;
        return averageAngle;
    }

    IEnumerator Wait(float waitTime)
    {
        wait = true;
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("isWalking", false);
        wait = false;
    }
}
