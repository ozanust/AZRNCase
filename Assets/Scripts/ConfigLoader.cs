using UnityEngine;
using System.IO;

public static class ConfigLoader
{
    public static string filePath = "GameConfig.json";

    // Here we can also use some cloud service like Firebase to download the config file. So that we can change game configs even without making a version update.
    public static GameConfig GetConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, filePath);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameConfig>(json);
        }
        else
        {
            Debug.LogError("Config file not found at path: " + path);
            return null;
        }
    }
}
