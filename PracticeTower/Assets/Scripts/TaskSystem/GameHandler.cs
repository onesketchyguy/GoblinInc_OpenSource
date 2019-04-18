using UnityEngine;
using LowEngine.Tasks;
using LowEngine.Saving;
using System.IO;
using LowEngine.TimeManagement;
using System.Collections.Generic;

namespace LowEngine
{
    public class GameHandler : MonoBehaviour
    {
        #region Public instance
        public static GameHandler instance;

        private void Awake()
        {
            instance = this;

            SetupApplicationVersion();
        }
        #endregion

        public static float MoneyToPayOnPayDay()
        {
            float val = 0;

            foreach (var worker in FindObjectsOfType<Worker>())
            {
                val += worker.workerData.pay;
            }

            return val;
        }

        public static int daysUntilPayDay = 6;

        public Material gameObjectMaterial;

        SaveManager.SaveData ActivePlayerData;
        public SaveManager.SaveData[] savesData;

        public string saveName = "Forrest";

        public enum GameState { Default, Placing, Paused }

        public static GameState gameState = GameState.Default;

        public float Money = 50;

        private void Start()
        {
            TimeScale.DayChanged += NewDay;
        }

        public void NewDay()
        {
            if (daysUntilPayDay > 0)
                daysUntilPayDay--;

            if (daysUntilPayDay == 0)
            {
                //It's pay day!

                Money -= MoneyToPayOnPayDay();

                daysUntilPayDay = 7;

                NotificationManager.instance.ShowNotification($"You spent: ${MoneyToPayOnPayDay()} on saleries this week.");
            }
            else
            {
                if (daysUntilPayDay > 1)
                {
                    NotificationManager.instance.ShowNotification($"{daysUntilPayDay} days left until pay day!");
                }
                else
                {
                    NotificationManager.instance.ShowNotification($"Payday is tomorrow!");
                }
            }

            if (Money < 0)
            {
                NotificationManager.instance.ShowNotification("You've gone broke!");
            }
        }

        void SetupApplicationVersion()
        {
            string currentVersion = Application.version;

            string dir = $"{Application.dataPath}";

            File.WriteAllText($"{dir}/version.txt", $"version:{currentVersion}");
        }

        public void SaveGame()
        {
            // -----------------Save Player data---------------
            ActivePlayerData = new SaveManager.SaveData(saveName, Money, TimeScale.minutes, TimeScale.hours, TimeScale.days);

            SaveManager.DeleteSavedGame(ActivePlayerData);

            SaveManager.SavePlayerData(ActivePlayerData);

            // -----------------Save workers---------------
            Worker[] workers = FindObjectsOfType<Worker>();
            SaveManager.SavableObject.Worker[] savedWorkers = new SaveManager.SavableObject.Worker[workers.Length];

            for (int i = 0; i < workers.Length; i++)
            {
                savedWorkers[i] = workers[i].workerData;
            }

            SaveManager.SaveWorkers(ActivePlayerData.userName, savedWorkers);

            // -----------------Save Objects---------------
            PlacedObject[] objects = FindObjectsOfType<PlacedObject>();
            List<SaveManager.SavableObject.WorldObject> savedObjects = new List<SaveManager.SavableObject.WorldObject>() { };

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].objectData.type == ObjectType.Abstract)
                {
                    continue;
                }

                savedObjects.Add(objects[i].objectData);
            }

            SaveManager.SaveObjects(ActivePlayerData.userName, savedObjects.ToArray());

            NotificationManager.instance.ShowNotification("Game saved!");
        }

        public void SetupSaves()
        {
            // -----------------Load Player data---------------
            SaveManager.LoadPlayerData(out savesData);
        }

        public void LoadGame(int index)
        {
            MapLayoutManager map = FindObjectOfType<MapLayoutManager>();

            SetupSaves();

            ActivePlayerData = savesData[index];

            Money = ActivePlayerData.money;
            saveName = ActivePlayerData.userName;

            TimeScale.minutes = ActivePlayerData.minute;
            TimeScale.hours = ActivePlayerData.hour;
            TimeScale.days = ActivePlayerData.day;

            // -----------------Load workers---------------
            foreach (var item in FindObjectsOfType<Worker>())
            {
                Destroy(item.gameObject);
            }

            SaveManager.SavableObject.Worker[] workers;

            SaveManager.LoadWorkers(ActivePlayerData.userName, out workers);

            if (workers == null || workers.Length == 0)
            {
                Debug.Log("No workers to load!");
            }
            else
            {
                foreach (var data in workers)
                {
                    if (data == null) continue;

                    Constructor.GetWorker(data);
                }
            }

            // -----------------Load objects---------------
            foreach (var item in FindObjectsOfType<PlacedObject>())
            {
                Destroy(item.gameObject);
            }

            SaveManager.SavableObject.WorldObject[] objects;

            SaveManager.LoadAllObjects(ActivePlayerData.userName, out objects);

            if (objects == null || objects.Length == 0)
            {
                Debug.Log("No objects to load!");
            }
            else
            {
                foreach (var data in objects)
                {
                    if (data == null) continue;

                    MapLayoutManager.ReplaceTileInDictionary(data.position, Constructor.GetObject(data));
                }
            }

            NotificationManager.instance.ShowNotification("Game Loaded!");
        }

        public SaveManager.SaveData GetGameData(int index)
        {
            var data = savesData[index];

            return data;
        }

        public void ClearSavedData(int index)
        {
            SaveManager.DeleteSavedGame(savesData[index]);

            NotificationManager.instance.ShowNotification("Saved data deleted!");
        }
    }
}