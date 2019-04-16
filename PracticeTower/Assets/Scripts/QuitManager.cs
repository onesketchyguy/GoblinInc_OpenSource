using System.Collections;
using UnityEngine;

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
            Application.Quit();
        }

        public void SaveAndQuit()
        {
            StartCoroutine("SaveAndQuit_");
        }

        IEnumerator SaveAndQuit_()
        {
            GameHandler.instance.SaveGame();

            yield return new WaitForSeconds(1);

            Application.Quit();
        }
    }
}