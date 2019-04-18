﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LowEngine.Tasks.Needs;

namespace LowEngine.Saving
{
    public static class SaveManager
    {
        const char DATA_SEPERATOR = '~';

        static readonly string PLAYER_DATA_FOLDER = $"{Application.dataPath}/LocalData/Saves/PlayerData/";
        static readonly string OBJECT_FOLDER = $"{Application.dataPath}/LocalData/Saves/BuildingData/";
        static readonly string WORKER_FOLDER = $"{Application.dataPath}/LocalData/Saves/Workers/";
        static readonly string NAMES_DIRECTORY = $"{Application.dataPath}/LocalData/Names/";

        private static void initialize()
        {
            if (!Directory.Exists(PLAYER_DATA_FOLDER))
            {
                Directory.CreateDirectory(PLAYER_DATA_FOLDER);
            }

            if (!Directory.Exists(OBJECT_FOLDER))
            {
                Directory.CreateDirectory(OBJECT_FOLDER);
            }

            if (!Directory.Exists(WORKER_FOLDER))
            {
                Directory.CreateDirectory(WORKER_FOLDER);
            }

            if (!Directory.Exists(NAMES_DIRECTORY))
            {
                Directory.CreateDirectory(NAMES_DIRECTORY);
            }
        }

        public static void SaveNames(string[] FirstNames, string[] LastNames)
        {
            initialize();

            string seperator = ":";

            string firstNames = string.Join(seperator, FirstNames);

            File.WriteAllText($"{NAMES_DIRECTORY}/FirstNames.txt", firstNames);

            string lastNames = string.Join(seperator, LastNames);

            File.WriteAllText($"{NAMES_DIRECTORY}/LastNames.txt", lastNames);
        }

        public static void LoadNames(out string[] FirstNames, out string[] LastNames)
        {
            initialize();

            FirstNames = null;
            LastNames = null;

            List<string> fnames = new List<string>() { };
            List<string> lnames = new List<string>() { };

            foreach (string filePath in Directory.EnumerateFiles(NAMES_DIRECTORY, "*.txt"))
            {
                if (filePath.Contains("FirstNames"))
                {
                    string contents = File.ReadAllText(filePath);

                    string[] names = contents.Split(':');

                    foreach (var name in names)
                    {
                        fnames.Add(name);
                    }
                }

                if (filePath.Contains("LastNames"))
                {
                    string contents = File.ReadAllText(filePath);

                    string[] names = contents.Split(':');

                    foreach (var name in names)
                    {
                        lnames.Add(name);
                    }
                }
            }

            if (fnames.Count > 0) FirstNames = fnames.ToArray();
            if (lnames.Count > 0) LastNames = lnames.ToArray();
        }

        public static void DeleteSavedGame(SaveData playerData)
        {
            if (playerData == null) return;

            if (Directory.Exists(PLAYER_DATA_FOLDER))
            {
                foreach (string filePath in Directory.EnumerateFiles(PLAYER_DATA_FOLDER, "*.txt"))
                {
                    if (filePath.Contains(playerData.userName))
                    {
                        File.Delete(filePath);
                    }
                }
            }

            if (Directory.Exists(OBJECT_FOLDER))
            {
                foreach (string filePath in Directory.EnumerateFiles(OBJECT_FOLDER, "*.txt"))
                {
                    if (filePath.Contains(playerData.userName))
                    {
                        File.Delete(filePath);
                    }
                }
            }

            if (Directory.Exists(WORKER_FOLDER))
            {
                foreach (string filePath in Directory.EnumerateFiles(WORKER_FOLDER, "*.txt"))
                {
                    if (filePath.Contains(playerData.userName))
                    {
                        File.Delete(filePath);
                    }
                };
            }
        }

        public static void SavePlayerData(SaveData playerData)
        {
            initialize();

            string saveData = JsonUtility.ToJson(playerData);

            File.WriteAllText($"{PLAYER_DATA_FOLDER}/{playerData.userName}_saveData.txt", saveData);
        }

        public static void LoadPlayerData(out SaveData[] playerData)
        {
            initialize();

            string saveData = string.Empty;

            List<SaveData> saves = new List<SaveData>() { };

            foreach (string file in Directory.EnumerateFiles(PLAYER_DATA_FOLDER, "*.txt"))
            {
                string save_data_string = File.ReadAllText(file);

                SaveData save_data = JsonUtility.FromJson<SaveData>(save_data_string);

                saves.Add(save_data);
            }

            playerData = saves.ToArray();
        }

        public static void SaveObjects(string playerData, SavableObject.WorldObject[] Objects)
        {
            initialize();

            string objectsData = "";

            foreach (var obj in Objects)
            {
                string newData;

                SaveGameObject(obj, out newData);

                objectsData += newData;
            }

            File.WriteAllText($"{OBJECT_FOLDER}/{playerData}_objects.txt", objectsData);
        }

        public static void LoadAllObjects(string playerData, out SavableObject.WorldObject[] Objects)
        {
            initialize();

            Objects = null;

            List<SavableObject.WorldObject> ObjectList = new List<SavableObject.WorldObject>() { };

            foreach (string filePath in Directory.EnumerateFiles(OBJECT_FOLDER, "*.txt"))
            {
                if (filePath.Contains($"{playerData}_objects"))
                {
                    string contents = File.ReadAllText(filePath);

                    string[] objects = contents.Split(DATA_SEPERATOR);

                    foreach (var objectData in objects)
                    {
                        SavableObject.WorldObject LoadedObject = LoadObject(objectData);

                        if (LoadedObject != null)
                            ObjectList.Add(LoadedObject);
                    }
                }

            }

            if (ObjectList.Count > 0)
            {
                Objects = ObjectList.ToArray();
            }
        }

        public static void SaveWorkers(string playerData, SavableObject.Worker[] workers)
        {
            initialize();

            string workerData = "";

            foreach (var worker in workers)
            {
                string n_Data;

                SaveWorker(worker, out n_Data);

                workerData += n_Data;
            }

            File.WriteAllText($"{WORKER_FOLDER}/{playerData}_worker.txt", workerData);
        }

        public static void LoadWorkers(string playerData, out SavableObject.Worker[] workers)
        {
            initialize();

            workers = null;

            List<SavableObject.Worker> workerList = new List<SavableObject.Worker>() { };

            foreach (string file in Directory.EnumerateFiles(WORKER_FOLDER, "*.txt"))
            {
                if (file.Contains($"{playerData}_worker"))
                {
                    string worker_data = File.ReadAllText(file);

                    string[] objects = worker_data.Split(DATA_SEPERATOR);

                    foreach (var objectData in objects)
                    {
                        SavableObject.Worker loadedWorker = LoadWorker(objectData);

                        if (loadedWorker != null)
                            workerList.Add(loadedWorker);
                    }
                }
            }

            if (workerList.Count > 0) workers = workerList.ToArray();
        }

        //Functions
        private static void SaveWorker(SavableObject.Worker worker, out string data)
        {
            data = $"{DATA_SEPERATOR}{JsonUtility.ToJson(worker)}\n";
        }

        private static SavableObject.Worker LoadWorker(string saveString)
        {
            SavableObject.Worker worker = JsonUtility.FromJson<SavableObject.Worker>(saveString);

            return worker;
        }

        private static void SaveGameObject(SavableObject.WorldObject obj, out string data)
        {
            data = $"{DATA_SEPERATOR}{JsonUtility.ToJson(obj)}\n";
        }

        private static SavableObject.WorldObject LoadObject(string saveString)
        {
            SavableObject.WorldObject worldObject = JsonUtility.FromJson<SavableObject.WorldObject>(saveString);

            return worldObject;
        }


        [System.Serializable]
        public class SavableObject
        {
            public float pVal;
            public Vector2 position;
            public Quaternion rotation;
            public string name;

            public Color color = Color.white;

            [System.Serializable]
            public class WorldObject : SavableObject
            {
                public bool ChangableColor;
                public bool rotatable;

                public string spriteName;
                public int spriteSortingLayer;

                public Vector2 childPos;

                public float wVal;

                public PlacedObjectType objectType;

                public ObjectType type;

                public NeedDefinition fulFills;
            }

            public class Worker : SavableObject
            {
                // --------Visuals----------
                public int headIndex;
                public int eyeIndex;
                public int noseIndex;
                public int mouthIndex;
                public int hairIndex;

                //-----------Stats------------
                public float hunger;
                public float thirst;
                public float skill;
                public float experience;

                public float FlatPay;

                public float pay;

                public Worker(string name, int headIndex, int eyeIndex, int noseIndex, int mouthIndex, int hairIndex, Color hairColor, float skill, float FlatPay, float pay, float experience, float[] needs)
                {
                    this.name = name;
                    this.headIndex = headIndex;
                    this.eyeIndex = eyeIndex;
                    this.noseIndex = noseIndex;
                    this.mouthIndex = mouthIndex;
                    this.hairIndex = hairIndex;
                    this.color = hairColor;
                    this.skill = skill;
                    this.hunger = needs[0];
                    this.thirst = needs[1];
                    this.FlatPay = FlatPay;
                    this.pay = pay;
                    this.experience = experience;
                }

            }
        }

        public class SaveData
        {
            public string userName;

            public float money;

            public int minute;
            public int hour;
            public int day;

            public SaveData(string userName, float money, int minute, int hour, int day)
            {
                this.userName = userName;
                this.money = money;
                this.minute = minute;
                this.hour = hour;
                this.day = day;
            }
        }
    }
}