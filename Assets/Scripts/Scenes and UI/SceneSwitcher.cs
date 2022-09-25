using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup canvasGroup;
    
    public void LoadMapView() => StartCoroutine(TransitionToMapView());

    IEnumerator TransitionToMapView()
    {
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(1, 1));

        AsyncOperation loading = SceneManager.LoadSceneAsync("Map View");
        float progress = 0;

        while (!loading.isDone)
        {
            progress = Mathf.Clamp01(loading.progress / 0.9f);
            if (progressBar) progressBar.value = progress;
            if (progressText) progressText.text = Mathf.Round(progress * 100) + "%";
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(0, 1));
        loadingScreen.SetActive(false);
    }

    IEnumerator FadeLoadingScreen(float target, float duration)
    {
        float startValue = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    public void LoadSettings() => SceneManager.LoadScene("Settings");
    public void LoadMainMenu() => SceneManager.LoadScene("Main Menu");
    public void LoadIntroduction() => SceneManager.LoadScene("Introduction");
    public void LoadEnemyTower() => SceneManager.LoadScene("EnemyTowerAR");

    public void LoadNextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void LoadPreviousScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
}
