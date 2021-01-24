using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public GameObject saveOrDiscardMenu;

    public Slider MasterVolumeSlider;

    public Slider SFXVolumeSlider;

    public Slider MusicVolumeSlider;

    [HideInInspector]
    public bool saveChanges = false;

    public UnityEngine.Events.UnityEvent OnSaveAndQuit;

    private bool ChangesExists()
    {
        if (MasterVolumeSlider.value != PlayerPrefsManager.MasterVolume)
            return true;
        if (MusicVolumeSlider.value != PlayerPrefsManager.MusicVolume)
            return true;
        if (SFXVolumeSlider.value != PlayerPrefsManager.SFXVolume)
            return true;

        return false;
    }

    private void OnEnable()
    {
        //Load the players options onto the items

        if (saveOrDiscardMenu == null)
        {
            FindObjectOfType<MainMenu>().SwapMenu(0);

            Debug.LogError("Discard menu == null! Returning to main menu.");

            saveChanges = false;

            return;
        }
        else
        {
            saveOrDiscardMenu.SetActive(false);
        }

        MasterVolumeSlider.value = PlayerPrefsManager.MasterVolume;
        SFXVolumeSlider.value = PlayerPrefsManager.SFXVolume;
        MusicVolumeSlider.value = PlayerPrefsManager.MusicVolume;
    }

    private void OnDisable()
    {
        if (saveChanges)
        {
            //Save the players options from the items

            PlayerPrefsManager.MasterVolume = MasterVolumeSlider.value;
            PlayerPrefsManager.SFXVolume = SFXVolumeSlider.value;
            PlayerPrefsManager.MusicVolume = MusicVolumeSlider.value;
        }

        saveChanges = false;
    }

    public void OpenDiscardMenu()
    {
        if (ChangesExists())
        {
            saveOrDiscardMenu.SetActive(true);
        }
        else
        {
            SaveAndQuit(false);
        }
    }

    public void SaveAndQuit(bool save)
    {
        saveChanges = save;

        saveOrDiscardMenu.SetActive(false);

        OnSaveAndQuit?.Invoke();
    }
}