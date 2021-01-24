using UnityEngine;
using UnityEngine.Audio;

public class AudioLoader : MonoBehaviour
{
    public AudioMixer mixer;

    private static AudioLoader instance;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            UpdateValues();
            instance = this;
        }
    }

    public void UpdateValues()
    {
        mixer.SetFloat("Master", (1 - PlayerPrefsManager.MasterVolume) * -80);
        mixer.SetFloat("SFX", (1 - PlayerPrefsManager.SFXVolume) * -80);
        mixer.SetFloat("Music", (1 - PlayerPrefsManager.MusicVolume) * -80);
    }

    public void UpdateMaster(float value) => mixer.SetFloat("Master", (1 - value) * -80);

    public void UpdateSFX(float value) => mixer.SetFloat("SFX", (1 - value) * -80);

    public void UpdateMusic(float value) => mixer.SetFloat("Music", (1 - value) * -80);
}