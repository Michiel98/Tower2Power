using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class LumberRecognition : MonoBehaviour
{
    PlayerInventory inventory;
    public AudioSource wood;
    FileInfo f;
    StreamWriter w;

    int count;
    
    bool startStreak = true;

    // Start is called before the first frame update
    void Start()
    {
        inventory = PlayerInventory.instance;
        Input.gyro.enabled = true;
    }
    // Update is called once per frame
    void Update()// Fixedupdate skipt vaak ne count
    {
        if (Input.touchCount >= 1)
        {
            if (Input.gyro.rotationRate.z > 10 && Math.Abs(Input.gyro.attitude.x) + Math.Abs(Input.gyro.attitude.y) < 0.3 && Input.acceleration.x > 2)
            {

                if (startStreak)
                {
                    StartCoroutine(streak());

                }
            }


        }

    }
    public void PlayWoodSound()
    {
        wood.Play();
    }


    IEnumerator streak()
    {

        startStreak = false;
        inventory.AddResource(ResourceType.Lumber, 1);
        PlayWoodSound();
        //Wait for 2 seconds
        yield return new WaitForSeconds(0.5f);
        startStreak = true;

    }
}


