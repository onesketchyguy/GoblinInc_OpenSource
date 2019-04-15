using UnityEngine;
using UnityEngine.UI;
using LowEngine.Saving;
using System.Collections.Generic;

namespace LowEngine
{
    public class OptionsMenuHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            showingMain = true;
            Saving = false;

            UpdateView();
        }

        bool Saving;

        bool showingMain;

        public Font font;

        public GameObject MainView;

        public GameObject SavesView;

        public Transform savesViewContentHold;

        List<GameObject> GameObjects = new List<GameObject>() { };

        public GameObject inputRegion;

        public InputField InputField;

        string input = "";

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
                GameHandler.instance.saveName = input;

                GameHandler.instance.SaveGame();

                Saving = false;
                showingMain = true;
                UpdateView();

                InputField.onValueChanged.RemoveListener(TextChanged);
            }
        }
        

        private void SpawnLoadButton(SaveManager.SaveData data, int index)
        {
            Button button = Instantiate(CreateButton(data), savesViewContentHold);

            GameObjects.Add(button.gameObject);

            button.onClick.AddListener( () => { GameHandler.instance.LoadGame(index); showingMain = true; UpdateView(); });
        }

        private void SpawnDeleteSaveButton(SaveManager.SaveData data, int index)
        {
            Button button = Instantiate(CreateButton(data), savesViewContentHold);

            GameObjects.Add(button.gameObject);

            button.onClick.AddListener(() => { GameHandler.instance.ClearSavedData(index); showingMain = true; UpdateView(); });
        }

        private void SpawnOverriteSaveButton(SaveManager.SaveData data, int index)
        {
            Button button = Instantiate(CreateButton(data), savesViewContentHold);

            GameObjects.Add(button.gameObject);

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
                showingMain = true;
                Saving = false;
                UpdateView();
            });
        }

        public void LoadGame()
        {
            GameHandler.instance.SetupSaves();

            for (int i = 0; i < GameHandler.instance.savesData.Length; i++)
            {
                SaveManager.SaveData save = GameHandler.instance.savesData[i];
                SpawnLoadButton(save, i);
            }

            if (GameObjects.Count == 0)
            {
                NotificationManager.instance.ShowNotification("No saves available to load!");
                return;
            }

            showingMain = false;
            UpdateView();
        }
        public void SaveGame()
        {
            GameHandler.instance.SetupSaves();

            for (int i = 0; i < GameHandler.instance.savesData.Length; i++)
            {
                SaveManager.SaveData save = GameHandler.instance.savesData[i];
                SpawnOverriteSaveButton(save, i);
            }

            Saving = true;

            showingMain = false;
            UpdateView();

            InputField.onValueChanged.AddListener(TextChanged);
        }
        public void ClearSaves()
        {
            GameHandler.instance.SetupSaves();

            for (int i = 0; i < GameHandler.instance.savesData.Length; i++)
            {
                SaveManager.SaveData save = GameHandler.instance.savesData[i];
                SpawnDeleteSaveButton(save, i);
            }

            if (GameObjects.Count == 0)
            {
                NotificationManager.instance.ShowNotification("No saves available to delete!");
                return;
            }

            showingMain = false;
            UpdateView();
        }
        public void LeaveGame()
        {
            Application.Quit();
        }

        void UpdateView()
        {
            MainView.SetActive(showingMain);
            SavesView.SetActive(!showingMain);

            InputField.gameObject.SetActive(Saving);
            inputRegion.SetActive(Saving);

            if (showingMain)
            {
                while (GameObjects.Count > 0)
                {
                    GameObject go = GameObjects[0];

                    Destroy(go);

                    GameObjects.Remove(go);
                }
            }
        }

        Button CreateButton(SaveManager.SaveData save)
        {
            string name = save != null ? save.userName : "new save file";

            string Money = save != null ? $"${save.money}" : "";

            Button b = new GameObject($"{name}").AddComponent<Button>();

            Text t = b.gameObject.AddComponent<Text>();

            t.font = font;

            t.supportRichText = true;

            t.text = $"<b>{name}</b>\n{Money}";

            b.targetGraphic = t;

            t.alignment = TextAnchor.UpperLeft;

            return b;
        }
    }
}