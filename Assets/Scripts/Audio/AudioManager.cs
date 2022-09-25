using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public AudioSource guideMusic;

    public Image soundOn;
    public Image soundOff;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (soundOff.gameObject.activeInHierarchy)
        {
            audioSource1.volume = 0;
            audioSource2.volume = 0;
            audioSource3.volume = 0;
            guideMusic.volume = 0;
        }
        else
        {
            audioSource1.volume = .5f;
            audioSource2.volume = 1;
            audioSource3.volume = .8f;
            guideMusic.volume = 1;
        }

        
    }
}
