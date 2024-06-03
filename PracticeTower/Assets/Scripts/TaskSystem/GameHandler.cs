using UnityEngine;
using LowEngine.Tasks;
using LowEngine.Saving;
using System.IO;
using LowEngine.TimeManagement;
using System.Collections.Generic;
using System.Collections;

namespace LowEngine
{
    public class GameHandler : MonoBehaviour
    {
        public static GameHandler instance;

        public static int GAME_DIFFICULTY = 1;

        public static float MoneyToPayOnPayDay()
        {
            float val = 0;

            foreach (var worker in instance.workers)
            {
                val += worker.workerData.pay;
            }

            return val;
        }

        public static float GetEarnDifficulty()
        {
            return 3.0f / GAME_DIFFICULTY;
        }

        public static int daysUntilPayDay = 6;

        private List<Worker> workers = new List<Worker>();

        public Material gameObjectMaterial;

        private SaveManager.SaveData ActivePlayerData;
        public SaveManager.SaveData[] savesData;

        private MapLayoutManager mapLayoutManager;

        public string saveName = "AutoSave";

        public enum GameState { Default, Placing, Paused }

        public static GameState gameState = GameState.Default;

        public float Money = 50;

        private void Awake()
        {
            instance = this;

            SetupApplicationVersion();
        }

        private void Start()
        {
            mapLayoutManager = FindObjectOfType<MapLayoutManager>();
            TimeScale.DayChanged += NewDay;
        }

        public static void RegisterWorker(Worker worker)
        {
            if (instance.workers.Contains(worker) == false)
                instance.workers.Add(worker);
        }

        public static void DeregisterWorker(Worker worker)
        {
            if (instance.workers.Contains(worker) == true)
                instance.workers.Remove(worker);
        }

        public void NewDay()
        {
            if (daysUntilPayDay > 0)
                daysUntilPayDay--;

            if (daysUntilPayDay == 0)
            {
                // It's pay day!

                int quitWorkers = 0;
                double amountSpent = 0.0;

                for (int i = instance.workers.Count - 1; i >= 0; i--)
                {
                    Worker worker = instance.workers[i];
                    if (Money > 0)
                    {
                        Money -= worker.workerData.pay;
                        amountSpent += worker.workerData.pay;
                    }
                    else
                    {
                        quitWorkers++;
                        Destroy(worker.gameObject);
                    }
                }

                daysUntilPayDay = 7;

                if (quitWorkers > 0)
                {
                    NotificationManager.instance.ShowNotification($"{quitWorkers} workers quit because you couldn't pay them.");
                }

                NotificationManager.instance.ShowNotification($"You spent: ${System.Math.Round(amountSpent, 2)} on saleries this week.");
            }
            else
            {
                if (daysUntilPayDay > 1)
                {
                    NotificationManager.instance.ShowNotification($"{daysUntilPayDay + 1} days left until pay day!");
                }
                else
                {
                    NotificationManager.instance.ShowNotification($"Payday is tomorrow!");
                }
            }

            if (Money < 0)
            {
                NotificationManager.instance.ShowNotification("You've gone broke!");

                // Trigger Game over
            }
        }

        private void SetupApplicationVersion()
        {
            string currentVersion = Application.version;

            string dir = $"{Application.dataPath}";

            File.WriteAllText($"{dir}/version.txt", $"version:{currentVersion}");
        }

        public void SaveGame(string name = "")
        {
            if (string.IsNullOrEmpty(name) == false)
                saveName = name;

            StartCoroutine(SaveGameAction());
        }

        public IEnumerator SaveGameAction()
        {
            // -----------------Save Player data---------------
            ActivePlayerData = new SaveManager.SaveData(saveName, Money, TimeScale.minutes, TimeScale.hours, TimeScale.days);

            SaveManager.DeleteSavedGame(ActivePlayerData);

            SaveManager.SavePlayerData(ActivePlayerData);

            yield return null;
            Debug.Log("Saving workers...");

            // -----------------Save workers---------------
            var savedWorkers = new SaveManager.SavableObject.Worker[workers.Count];

            for (int i = 0; i < workers.Count; i++)
                savedWorkers[i] = workers[i].workerData;

            SaveManager.SaveWorkers(ActivePlayerData.userName, savedWorkers);

            yield return null;
            Debug.Log("Saving objects...");

            // -----------------Save Objects---------------
            var objects = FindObjectsOfType<PlacedObject>();
            var savedObjects = new List<SaveManager.SavableObject.WorldObject>() { };

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].objectData.type == ObjectType.Abstract)
                {
                    continue;
                }

                savedObjects.Add(objects[i].objectData);
            }

            SaveManager.SaveObjects(ActivePlayerData.userName, savedObjects.ToArray());

            yield return null;
            Debug.Log("Save complete.");

            NotificationManager.instance.ShowNotification("Game saved!");
        }

        public void SetupSaves()
        {
            // -----------------Load Player data---------------
            SaveManager.LoadPlayerData(out savesData);
        }

        public IEnumerator LoadGame(int index)
        {
            yield return null;

            ActivePlayerData = savesData[index];

            Money = ActivePlayerData.money;
            saveName = ActivePlayerData.userName;

            TimeScale.minutes = ActivePlayerData.minute;
            TimeScale.hours = ActivePlayerData.hour;
            TimeScale.days = ActivePlayerData.day;

            yield return null;

            // Destroy existing workers
            var workersObjects = instance.workers.ToArray();

            for (int i = workersObjects.Length - 1; i >= 0; i--)
                Destroy(workersObjects[i].gameObject);

            yield return null;

            // -----------------Load workers---------------

            SaveManager.SavableObject.Worker[] loadedWorkers;
            SaveManager.LoadWorkers(ActivePlayerData.userName, out loadedWorkers);

            if (loadedWorkers == null || loadedWorkers.Length == 0)
            {
                Debug.Log("No workers to load!");
            }
            else
            {
                foreach (var data in loadedWorkers)
                {
                    if (data == null) continue;

                    Constructor.GetWorker(data);
                }
            }

            yield return null;

            // -----------------Load objects---------------
            foreach (var item in FindObjectsOfType<PlacedObject>())
                Destroy(item.gameObject);

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

            yield return null;

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