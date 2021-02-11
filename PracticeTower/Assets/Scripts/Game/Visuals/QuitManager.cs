using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LowEngine
{
    public class QuitManager : MonoBehaviour
    {
        private LoadingScreen loadingScreen;

        public string levelToLoad = "";

        public void QuitWithOutSaving()
        {
            Load();
        }

        public void SaveAndQuit()
        {
            StartCoroutine(SaveAndQuitAction());
        }

        private IEnumerator SaveAndQuitAction()
        {
            yield return GameHandler.instance.StartCoroutine(GameHandler.instance.SaveGameAction());
            yield return new WaitForEndOfFrame();
            Load();
        }

        private void Load()
        {
            loadingScreen = FindObjectOfType<LoadingScreen>();

            if (loadingScreen != null)
            {
                loadingScreen.StartLoad(levelToLoad);
            }
            else
            {
                SceneManager.LoadSceneAsync(levelToLoad);
            }
        }
    }
}