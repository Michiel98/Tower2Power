using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SawRecognition : MonoBehaviour
{
    public AudioSource sawForth;
    public AudioSource sawBack;

    int count;

    bool startStreak = true;
    bool forward = false;

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
            if (Math.Abs(Input.gyro.attitude.x) + Math.Abs(Input.gyro.attitude.y) < 0.3 && Input.acceleration.y < - 1 && Input.gyro.rotationRate.x > -2)
            {

                if (startStreak)
                {
                    StartCoroutine(streakforth());

                }
            }
            if (Math.Abs(Input.gyro.attitude.x) + Math.Abs(Input.gyro.attitude.y) < 0.3 && Input.acceleration.y > 1)
            {

                if (startStreak)
                {
                    StartCoroutine(streakback());

                }
            }


        }

    }
    public void PlaySawSoundForth()
    {
        sawForth.Play();
    }
    public void PlaySawSoundBack()
    {
        sawBack.Play();
    }


    IEnumerator streakforth()
    {

        startStreak = false;
        PlaySawSoundForth();
        forward = true;
        //Wait for 2 seconds
        yield return new WaitForSeconds(0.3f);
        startStreak = true;

    }
    IEnumerator streakback()
    {

        startStreak = false;
        if (forward)
        {
            //Handheld.Vibrate();
            PlayerInventory.instance.IncreaseSawActions(1);
        }

        PlaySawSoundBack();
        //Wait for 2 seconds
        forward = false;
        yield return new WaitForSeconds(0.3f);
        startStreak = true;

    }
}
