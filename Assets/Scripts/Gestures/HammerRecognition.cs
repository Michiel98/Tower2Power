using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class HammerRecognition : MonoBehaviour
{
    public AudioSource hammer;
    bool free = true;
    bool startStreak = true;
    bool f1 = false;
    bool f2 = false;
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;

    }
    // Update is called once per frame
    void Update()// Fixedupdate skipt vaak ne count
    {
        if (Input.touchCount >= 1)
        {
            if (Math.Abs(Input.gyro.attitude.z) + Math.Abs(Input.gyro.attitude.w) < 0.5)
            {
                if (startStreak)
                {
                    StartCoroutine(streak());
                    f1 = true;
                }
            }
            //if (Input.acceleration.z < -5)
              //  f1 = false;
            else if (Input.gyro.rotationRate.x < -5 || (Math.Abs(Input.gyro.attitude.z) + Math.Abs(Input.gyro.attitude.w) > 0.9 && Math.Abs(Input.gyro.attitude.z) + Math.Abs(Input.gyro.attitude.w) < 1.1))
            {
                f2 = true;
                if (f1 && f2 && free)
                {
                    f1 = false;
                    f2 = false;
                    PlayHammer();

                }
            }

        }
    }


    public void PlayHammer()
    {
        free = false;
        PlayerInventory.instance.IncreaseHammerActions(1);
        //Handheld.Vibrate();
        hammer.Play();
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        //Wait for 2 seconds
        yield return new WaitForSeconds(0.2f);
        f1 = false;
        f2 = false;
        free = true;

    }
    IEnumerator streak()
    {
        startStreak = false;
        //Wait for 2 seconds
        yield return new WaitForSeconds(0.2f);
        f1 = false;
        f2 = false;
        startStreak = true;

    }
}
