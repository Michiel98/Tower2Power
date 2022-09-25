using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    public Image soundOn;
    public Image soundOff;


    // Start is called before the first frame update
    void ToggleSound()
    {
        soundOn.gameObject.SetActive(!soundOn.gameObject.activeInHierarchy);
        soundOff.gameObject.SetActive(!soundOff.gameObject.activeInHierarchy);

    }


}
