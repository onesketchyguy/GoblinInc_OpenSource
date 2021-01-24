using UnityEngine;

public class FirstLaunchManager : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.GetInt("FirstLaunch") != 1)
        {
            PlayerPrefsManager.MasterVolume = 1;
            PlayerPrefsManager.MusicVolume = 0.9f;
            PlayerPrefsManager.SFXVolume = 1;

            PlayerPrefs.SetInt("FirstLaunch", 1);
        }
    }
}