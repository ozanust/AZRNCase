using UnityEngine;
using System.IO;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class ConfigLoader
{
	public static string filePath = "GameConfig.json";

	// Here we can also use some cloud service like Firebase to download the config file. So that we can change game configs even without making a version update
	public static void GetConfig(Action<GameConfig> callback)
	{
		string path = Path.Combine(Application.streamingAssetsPath, filePath);
		string fileContent = string.Empty;

		if (Application.platform == RuntimePlatform.Android)
		{
			ReadFileFromAndroid(path, callback);
		}
		else
		{
			if (File.Exists(path))
			{
				fileContent = File.ReadAllText(path);
				GameConfig config = JsonUtility.FromJson<GameConfig>(fileContent);
				callback?.Invoke(config);
			}
			else
			{
				Debug.LogError("Config file not found at path: " + path);
			}
		}
	}

	async static void ReadFileFromAndroid(string path, Action<GameConfig> callback)
	{
		using (UnityWebRequest www = UnityWebRequest.Get(path))
		{
			var asyncOperation = www.SendWebRequest();

			while (!asyncOperation.isDone)
			{
				await Task.Delay(100);
			}

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError("Failed to load file from StreamingAssets: " + www.error);
			}
			else
			{
				string fileContent = www.downloadHandler.text;
				GameConfig config = JsonUtility.FromJson<GameConfig>(fileContent);
				callback?.Invoke(config);
			}
		}
	}
}
