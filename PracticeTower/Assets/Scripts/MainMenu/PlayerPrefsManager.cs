using UnityEngine;

public class PlayerPrefsManager
{
    private const string SFX_Volume = "SFX_VOLUME";

    private const string Music_Volume = "MUSIC_VOLUME";

    private const string Master_Volume = "MASTER_VOLUME";

    public static float MasterVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(Master_Volume);
        }
        set
        {
            PlayerPrefs.SetFloat(Master_Volume, value);
        }
    }

    public static float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(Music_Volume) * MasterVolume;
        }
        set
        {
            PlayerPrefs.SetFloat(Music_Volume, value);
        }
    }

    public static float SFXVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(SFX_Volume) * MasterVolume;
        }
        set
        {
            PlayerPrefs.SetFloat(SFX_Volume, value);
        }
    }
}