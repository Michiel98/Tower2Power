using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroductionManager : MonoBehaviour
{
    [Header("Background Images")]
    public Image image1;
    public Image image2;
    public Image image3;
    public Image guide1;

    int currentImage = 1;

    public void Start()
    {
        image1.gameObject.SetActive(true);
        image2.gameObject.SetActive(false);
        image3.gameObject.SetActive(false);
    }

    public void NextImage()
    {
        currentImage++;

        switch (currentImage)
        {
            case 2:
                image1.gameObject.SetActive(false);
                image2.gameObject.SetActive(true);
                image3.gameObject.SetActive(false);
                break;
            case 3:
                image1.gameObject.SetActive(false);
                image2.gameObject.SetActive(false);
                image3.gameObject.SetActive(true);
                break;
            case 4:
                image3.gameObject.SetActive(false);
                guide1.gameObject.SetActive(true);
                break;
            case 5:
                SceneManager.LoadScene("Main Menu");
                break;
        }
    }
}
