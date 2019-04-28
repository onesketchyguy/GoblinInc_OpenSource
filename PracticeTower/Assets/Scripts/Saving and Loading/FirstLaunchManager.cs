using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLaunchManager : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.GetInt("FirstLaunch") != 1)
        {
            PlayerPrefsManager.MasterVolume = 1;
            PlayerPrefsManager.MusicVolume = 1;
            PlayerPrefsManager.SFXVolume = 1;

            PlayerPrefs.SetInt("FirstLaunch", 1);
        }
    }
}
