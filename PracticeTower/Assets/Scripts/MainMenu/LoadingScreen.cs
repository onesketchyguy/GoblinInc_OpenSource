using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Text progressText;
    public Slider progressBar;
    public Image panel;

    public float fadeTime = 1f;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void StartLoad(string levelToLoad)
    {
        StartCoroutine(DisplayLoadingScreen(levelToLoad));
    }

    private IEnumerator DisplayLoadingScreen(string level)
    {
        panel.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);
        yield return null;

        // Fade in
        float fadeAmount = 0;
        while (fadeAmount < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            fadeAmount += Time.deltaTime;

            float alphaValue = (fadeAmount / fadeTime);

            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, alphaValue);
        }

        progressBar.gameObject.SetActive(true);

        yield return null;

        progressText.text = "";

        float LoadProgress = 0;

        AsyncOperation async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            LoadProgress = (int)(async.progress * 100);

            progressBar.value = async.progress;

            progressText.text = $"Loading...{LoadProgress}%";

            yield return null;
        }

        progressText.text = $"Done!";

        yield return new WaitForEndOfFrame();

        progressBar.gameObject.SetActive(false);

        // Fade out
        fadeAmount = 0;
        while (fadeAmount < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            fadeAmount += Time.deltaTime;

            float alphaValue = 1 - (fadeAmount / fadeTime);

            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, alphaValue);
        }

        panel.gameObject.SetActive(false);
    }
}