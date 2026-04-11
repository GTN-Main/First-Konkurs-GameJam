using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class SavesManager
{
    static string savePath_saveFolder = "/Saves";
    static string savePath_basic = "/Saves/BasicSave.json";

    public static async Task Save()
    {
        DebugUtility.WriteInColor("SavesManager-> Save execution", " #ff9516");

        if (!Directory.Exists(Application.dataPath + savePath_saveFolder))
        {
            Directory.CreateDirectory(Application.dataPath + savePath_saveFolder);
        }

        await SaveToFile<BasicSave>(GetBasicSaveData(), savePath_basic);
    }

    public static async Task Load()
    {
        DebugUtility.WriteInColor("SavesManager-> Load execution", " #ff9516");

        if (!Directory.Exists(Application.dataPath + savePath_saveFolder))
        {
            Directory.CreateDirectory(Application.dataPath + savePath_saveFolder);
        }

        if (isSaveExist(savePath_basic))
        {
            DebugUtility.WriteInColor("SavesManager-> Basic save", " #ff9516");
            await LoadFromFile<BasicSave>(savePath_basic);
            // Make forwarding of data
        }
        else
        {
            DebugUtility.WriteInColor("SavesManager-> Loading start Basic config", " #ff9516");
            await ResetBasicSave();
        }
    }

    public static async Task<T> LoadFromFile<T>(string path)
        where T : class
    {
        if (File.Exists(Application.dataPath + path))
        {
            string json = await File.ReadAllTextAsync(Application.dataPath + path);
            DebugUtility.WriteInColor(
                $"SavesManager-> Loaded data at path {path}\n{json}",
                " #ff9516"
            );
            return JsonUtility.FromJson<T>(json);
        }
        DebugUtility.WriteInColor($"SavesManager-> No file found at path: {path}", " #ff0000");
        return null;
    }

    public static async Task SaveToFile<T>(T data, string path)
        where T : class
    {
        string json = JsonUtility.ToJson(data);
        await File.WriteAllTextAsync(Application.dataPath + path, json);
        DebugUtility.WriteInColor($"SavesManager-> Saved data at path {path}\n{json}", " #ff9516");
    }

    public static bool isSaveExist(string path) => File.Exists(Application.dataPath + path);

    static async Task ResetBasicSave()
    {
        #region Reset bacis save data
        
        #endregion

        await SaveToFile<BasicSave>(GetBasicSaveData(), savePath_basic);
    }

    static void DebugResetError(Type type)
    {
        DebugUtility.WriteInColor(
            "SavesManager-> No default settings for type: " + type.Name,
            " #ff0000"
        );
        Debug.LogError("SavesManager-> No default settings for type: " + type.Name);
    }

    static BasicSave GetBasicSaveData()
    {
        return new BasicSave();
    }

    class BasicSave
    {
        public string someExampleData = "Example data";
    }
}
