using System.IO;
using UnityEngine;

public static class FileManager
{
    public static readonly string AppDir = Application.dataPath + "\\user_data";
    public const string fileExtention = ".json";

    public enum Directories
    {
        saves,
        settings
    }

    private static void CheckDirectory(Directories directory)
    {
        string dir = $"{AppDir}/{directory.ToString()}";

        if (Directory.Exists(dir)) return; // The directory exists, so we don't need to make one.

        Directory.CreateDirectory(dir);
    }

    private static void CreateFile(string content, string path)
    {
        File.WriteAllText($"{path}{fileExtention}", content);
    }

    /// <summary>
    /// Will save an abstract item to a specified directory.
    /// </summary>
    /// <param name="dataToSave"></param>
    /// <param name="directory"></param>
    public static void Save(object dataToSave, string fileName, Directories directory)
    {
        CheckDirectory(directory);
        var data = GetData(dataToSave);

        string dir = $"{AppDir}/{directory.ToString()}/{fileName}";
        CreateFile(data, dir);
    }

    /// <summary>
    /// Will convert an abstract item to a string data format.
    /// </summary>
    /// <param name="dataToSave"></param>
    /// <param name="directory"></param>
    public static string GetData(object dataToSave)
    {
        var data = JsonUtility.ToJson(dataToSave);
        return data;
    }

    public static T Load<T>(string fileName, Directories directory)
    {
        string dir = $"{AppDir}/{directory.ToString()}/{fileName}{fileExtention}";

        if (File.Exists(dir) == false)
            return default;

        var data = File.ReadAllText(dir);
        return JsonUtility.FromJson<T>(data);
    }

    public static T Load<T>(string file)
    {
        if (File.Exists(file) == false)
            return default;

        var data = File.ReadAllText(file);
        return JsonUtility.FromJson<T>(data);
    }
}

// Signed 2019 Forrest Hunter Lowe Low Bros Entertainment