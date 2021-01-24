using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LowEngine
{
    public class QuitManager : MonoBehaviour
    {
        public void CancelQuit()
        {
            FindObjectOfType<OptionsMenuHandler>().viewing = OptionsMenuHandler.Viewing.Main;
        }

        public void QuitWithOutSaving()
        {
            SceneManager.LoadSceneAsync(0);
        }

        public void SaveAndQuit()
        {
            StartCoroutine(SaveAndQuitAction());
        }

        private IEnumerator SaveAndQuitAction()
        {
            GameHandler.instance.SaveGame();

            yield return new WaitForSeconds(1);

            SceneManager.LoadSceneAsync(0);
        }
    }
}