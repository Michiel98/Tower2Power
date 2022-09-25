using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RopeRecognition : MonoBehaviour
{
    PlayerInventory inventory;
    public AudioSource rope;

    bool free = true;
    bool startStreak = true;
    bool f1 = false;
    bool f2 = false;
    // Start is called before the first frame update
    void Start()
    {
        inventory = PlayerInventory.instance;
    }
    // Update is called once per frame
    void Update()// Fixedupdate skipt vaak ne count
    {
        if (Input.touchCount >= 1)
        {
            if (Input.acceleration.z < -3)
            {
                if (startStreak)
                {
                    StartCoroutine(streak());
                    f1 = true;
                }
            }
            else if (Input.acceleration.x < -2)
            {
                f2 = true;
                if (f1 && f2 && free)
                {
                    f1 = false;
                    f2 = false;
                    PlayRope();

                }
            }

        }
    }


    public void PlayRope()
    {
        free = false;
        inventory.AddResource(ResourceType.Rope, 1);
        rope.Play();
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        //Wait for 2 seconds
        yield return new WaitForSeconds(1);
        f1 = false;
        f2 = false;
        free = true;

    }
    IEnumerator streak()
    {
        startStreak = false;
        //Wait for 2 seconds
        yield return new WaitForSeconds(1);
        f1 = false;
        f2 = false;
        startStreak = true;

    }
}
