using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UISwitcher : MonoBehaviour
{
    public static UISwitcher instance;

    [Header("UI")]
    public GameObject PlayerInventoryUI;
    public GameObject MapViewUI;
    
    
    [Header("Towers")]
    public GameObject HomeTowerUI;
    public Image HomeTowerBackground;
    public GameObject EnemyTowerUI;
    public Image EnemyTowerBackground;
    public GameObject TowerUpgradeUI;
    public GameObject UpgradeInstructionUI;

    [Header("Win/Lose")]
    public GameObject GameWonUI;
    public GameObject GameLostUI;
    
    [Header("Gather UI")]
    public GameObject GatherResourceUI;
    public GameObject GatherInstructionUI;
  
    [Header("Other")]
    public GameObject AnimationCamera;
    public GameObject GestureAnimation;
    public GameObject terrain;
    public Image FadeUI;
    public float fadeDuration = 3f;


    Color bgcolor;
    
    IEnumerator FadeIn(Action UIAction)
    {
        float startValue = FadeUI.color.a;
        float time = 0;
        while (time < fadeDuration)
        {
            FadeUI.color = new Color(0,0,0,Mathf.Lerp(startValue, 1f, time / fadeDuration));
            time += Time.deltaTime;
            yield return null;
        }
        FadeUI.color = new Color(0, 0, 0, 1f);
        UIAction();
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        float startValue = FadeUI.color.a;
        float time = 0;
        while (time < fadeDuration)
        {
            FadeUI.color = new Color(0,0,0,Mathf.Lerp(startValue, 0f, time / fadeDuration));
            time += Time.deltaTime;
            yield return null;
        }
        FadeUI.color = new Color(0, 0, 0, 0f);
    }
    
    void Awake()
    {
        if (instance != null) Destroy(instance);
        else instance = this;
        DontDestroyOnLoad(this);
    }

    public void EnterWinUI()
    {
        HomeTowerUI.SetActive(false);
        PlayerInventoryUI.SetActive(false);
        GameWonUI.SetActive(true);
    }

    public void EnterLoseUI()
    {
        HomeTowerUI.SetActive(false);
        PlayerInventoryUI.SetActive(false);
        GameLostUI.SetActive(true);
    }
    
    public void LeaveTowerUpgradeUI()
    {
        HomeTowerUI.SetActive(true);
        TowerUpgradeUI.SetActive(false);
        AnimationCamera.SetActive(false);
        GestureAnimation.SetActive(false);
    }

    public void HideUpgradeInstructionUI()
    {
        UpgradeInstructionUI.SetActive(false);
    }

    public void ShowGatherInstructionUI()
    {
        GatherInstructionUI.SetActive(true);
    }

    public void EnterGatherUI()
    {
        StartCoroutine(FadeIn(ShowGatherUI));
    }
    
    public void LeaveGatherUI()
    {
        StartCoroutine(FadeIn(HideGatherUI));
    }

    void ShowGatherUI()
    {
        AnimationCamera.SetActive(true);
        GestureAnimation.SetActive(true);
        terrain.SetActive(false);
        
        GatherResourceUI.SetActive(true); 
        GatherInstructionUI.SetActive(false); 
        MapViewUI.SetActive(false);
    }

    void HideGatherUI()
    {
        AnimationCamera.SetActive(false);
        GestureAnimation.SetActive(false);
        terrain.SetActive(true);
        
        MapViewUI.SetActive(true);
        GatherResourceUI.SetActive(false);
    }

    void ShowHomeTowerUI()
    {
        HomeTowerUI.SetActive(true); 
        HomeTowerBackground.color = bgcolor;
        MapViewUI.SetActive(false); 
        PlayerInventoryUI.SetActive(false); 
    }
    
    void HideHomeTowerUI()
    {
        MapViewUI.SetActive(true);
        PlayerInventoryUI.SetActive(true);
        HomeTowerUI.SetActive(false); 
        terrain.SetActive(true);
    }
    
    void HideEnemyTowerUI()
    {
        MapViewUI.SetActive(true); 
        EnemyTowerUI.SetActive(false); 
        terrain.SetActive(true);
    }

    void ShowEnemyTowerUI()
    {
        EnemyTowerUI.SetActive(true); 
        EnemyTowerBackground.color = bgcolor;
        MapViewUI.SetActive(true); 
    }

    public void EnterHomeTowerUI(Color color)
    {
        bgcolor = color;
        StartCoroutine(FadeIn(ShowHomeTowerUI));
    }

    public void LeaveHomeTowerUI()
    {
        StartCoroutine(FadeIn(HideHomeTowerUI));
    }
    
    public void LeaveEnemyTowerUI()
    {
        StartCoroutine(FadeIn(HideEnemyTowerUI));
    }
    
    public void EnterEnemyTowerUI(Color color)
    {
        bgcolor = color;
        StartCoroutine(FadeIn(ShowEnemyTowerUI));
    }
}
