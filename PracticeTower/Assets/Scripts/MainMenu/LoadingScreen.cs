using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Text progressText;
    public Slider progressBar;

    public void StartLoad(string levelToLoad)
    {
        StartCoroutine(DisplayLoadingScreen(levelToLoad));
    }

    IEnumerator DisplayLoadingScreen(string level)
    {
        progressText.text = "";

        float LoadProgress = 0;

        AsyncOperation async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            LoadProgress = (int) (async.progress * 100);

            progressBar.value = async.progress;

            progressText.text = $"Loading...{LoadProgress}%";

            yield return null;
        }
    }
}
