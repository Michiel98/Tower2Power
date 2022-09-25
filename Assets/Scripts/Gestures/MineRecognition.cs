using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class MineRecognition : MonoBehaviour
{
    PlayerInventory inventory;
    public AudioSource ore;
    FileInfo f;
    StreamWriter w;
    string s1;
    string s2;
    string s3;
    string s4;
    int count;
    bool free = true;
    bool startStreak = true;
    bool f1 = false;
    bool f2 = false;
    bool f3 = false;
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
            //if (Math.Abs(Input.gyro.attitude.z) + Math.Abs(Input.gyro.attitude.w) < 0.5 && Math.Abs(Input.gyro.attitude.z) + Math.Abs(Input.gyro.attitude.w) > 0.3)
            if(Input.acceleration.y < -0.8)
            {
                if (startStreak)
                {
                    //StartCoroutine(streak());
                    f1 = true;
                }
            }
            else if (Input.acceleration.z < -3 && free == true)
                f2 = true;
            else if (Input.acceleration.y > 0)
            {
                f3 = true;
                if (f1 && f2 && f3 && free)
                {
                    f1 = false;
                    f2 = false;
                    f3 = false;
                    PlayOre();

                }
            }

        }
    }


    public void PlayOre()
    {
        free = false;
        inventory.AddResource(ResourceType.Ore, 1);
        ore.Play();
        StartCoroutine(waiter());

    }
    IEnumerator waiter()
    {


        //Wait for 2 seconds
        yield return new WaitForSeconds(0.5f);
        f1 = false;
        f2 = false;
        f3 = false;
        free = true;

    }
    IEnumerator streak()
    {

        startStreak = false;
        //Wait for 2 seconds
        yield return new WaitForSeconds(1);
        f1 = false;
        f2 = false;
        f3 = false;
        startStreak = true;

    }
}


