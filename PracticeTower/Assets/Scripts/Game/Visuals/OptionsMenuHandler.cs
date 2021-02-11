using UnityEngine;
using UnityEngine.UI;
using LowEngine.Saving;
using System.Collections.Generic;
using UnityEngine.Events;

namespace LowEngine
{
    public class OptionsMenuHandler : MonoBehaviour
    {
        [Header("Sliders")]
        public Slider MasterVolumeSlider;

        public Slider SFXVolumeSlider;

        public Slider MusicVolumeSlider;

        [Header("Panels")]
        public GameObject inputRegion;

        [Header("Saving")]
        public Font savesFont;

        public InputField InputField;

        public Transform savesViewContentHold;

        private List<GameObject> savedGameObjects = new List<GameObject>() { };

        private string input = "";
        private AudioLoader audioLoader;

        public UnityEvent onSavedEvent;
        public UnityEvent onLoadEvent;

        private void OnEnable()
        {
            //Load the players options onto the items
            MasterVolumeSlider.value = PlayerPrefsManager.MasterVolume;
            SFXVolumeSlider.value = PlayerPrefsManager.SFXVolume;
            MusicVolumeSlider.value = PlayerPrefsManager.MusicVolume;

            audioLoader = FindObjectOfType<AudioLoader>();

            MasterVolumeSlider.onValueChanged.AddListener(audioLoader.UpdateMaster);
            SFXVolumeSlider.onValueChanged.AddListener(audioLoader.UpdateSFX);
            MusicVolumeSlider.onValueChanged.AddListener(audioLoader.UpdateMusic);
        }

        private void OnDisable()
        {
            //Save the players options from the items

            PlayerPrefsManager.MasterVolume = MasterVolumeSlider.value;
            PlayerPrefsManager.SFXVolume = SFXVolumeSlider.value;
            PlayerPrefsManager.MusicVolume = MusicVolumeSlider.value;

            MasterVolumeSlider.onValueChanged.RemoveListener(audioLoader.UpdateMaster);
            SFXVolumeSlider.onValueChanged.RemoveListener(audioLoader.UpdateSFX);
            MusicVolumeSlider.onValueChanged.RemoveListener(audioLoader.UpdateMusic);

            audioLoader.UpdateValues();
        }

        private void TextChanged(string input)
        {
            if (InputField.gameObject.activeSelf == true)
            {
                this.input = input;
            }
        }

        public void Save()
        {
            if (input.ToCharArray().Length > 0)
            {
                GameHandler.instance.SaveGame(input);

                InputField.onValueChanged.RemoveListener(TextChanged);
            }
        }

        private void SpawnLoadButton(SaveManager.SaveData data, int index)
        {
            Button button = Instantiate(CreateButton(data), savesViewContentHold);

            savedGameObjects.Add(button.gameObject);

            button.onClick.AddListener(() =>
            {
                GameHandler.instance.StartCoroutine(GameHandler.instance.LoadGame(index));
            });
        }

        private void SpawnDeleteSaveButton(SaveManager.SaveData data, int index)
        {
            Button button = Instantiate(CreateButton(data), savesViewContentHold);

            savedGameObjects.Add(button.gameObject);

            button.onClick.AddListener(() =>
            {
                GameHandler.instance.ClearSavedData(index);
                // Swap tab back to main
            });
        }

        private void SpawnOverriteSaveButton(SaveManager.SaveData data, int index)
        {
            Button button = Instantiate(CreateButton(data), savesViewContentHold);

            savedGameObjects.Add(button.gameObject);

            button.onClick.AddListener(() =>
            {
                SaveManager.SaveData OldData = GameHandler.instance.GetGameData(index);

                if (OldData == null)
                {
                    NotificationManager.instance.ShowNotification("Unable to override data!");
                    return;
                }

                input = OldData.userName;

                GameHandler.instance.ClearSavedData(index);

                GameHandler.instance.saveName = input;

                GameHandler.instance.SaveGame();
                onSavedEvent?.Invoke();
            });
        }

        public void LoadGame()
        {
            ClearSaveObjects();

            GameHandler.instance.SetupSaves();

            for (int i = 0; i < GameHandler.instance.savesData.Length; i++)
            {
                SaveManager.SaveData save = GameHandler.instance.savesData[i];
                SpawnLoadButton(save, i);
            }

            if (savedGameObjects.Count == 0)
            {
                NotificationManager.instance.ShowNotification("No saves available to load!");
                return;
            }

            // Swap to load game tabs

            InputField.gameObject.SetActive(false);
            InputField.onValueChanged.RemoveAllListeners();

            inputRegion.SetActive(false);
        }

        public void SaveGame()
        {
            ClearSaveObjects();

            GameHandler.instance.SetupSaves();

            for (int i = 0; i < GameHandler.instance.savesData.Length; i++)
            {
                SaveManager.SaveData save = GameHandler.instance.savesData[i];
                SpawnOverriteSaveButton(save, i);
            }

            // Swap to saved game tabs

            InputField.gameObject.SetActive(true);
            InputField.onValueChanged.AddListener(TextChanged);

            inputRegion.SetActive(true);
        }

        public void ClearSaves()
        {
            ClearSaveObjects();

            GameHandler.instance.SetupSaves();

            for (int i = 0; i < GameHandler.instance.savesData.Length; i++)
            {
                SaveManager.SaveData save = GameHandler.instance.savesData[i];
                SpawnDeleteSaveButton(save, i);
            }

            if (savedGameObjects.Count == 0)
            {
                NotificationManager.instance.ShowNotification("No saves available to delete!");
                return;
            }

            // Swap to load game tabs

            InputField.gameObject.SetActive(false);
            InputField.onValueChanged.RemoveAllListeners();

            inputRegion.SetActive(false);
        }

        private void ClearSaveObjects()
        {
            while (savedGameObjects.Count > 0)
            {
                GameObject go = savedGameObjects[0];

                Destroy(go);

                savedGameObjects.Remove(go);
            }
        }

        private Button CreateButton(SaveManager.SaveData save)
        {
            string name = save != null ? save.userName : "new save file";

            string Money = save != null ? $"${save.money}" : "";

            Button b = new GameObject($"{name}").AddComponent<Button>();

            Text t = b.gameObject.AddComponent<Text>();

            t.font = savesFont;

            t.supportRichText = true;

            t.text = $"<b>{name}</b>\n{Money}";

            b.targetGraphic = t;

            t.alignment = TextAnchor.UpperLeft;

            return b;
        }
    }
}