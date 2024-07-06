using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameConfigSO gameConfigSO;

	private void Start()
	{
		GameConfig config = ConfigLoader.GetConfig();

		if (config == null)
		{
			Debug.LogError("Config file not found!");
			return;
		}

		gameConfigSO.GameConfig = config;
		SceneManager.LoadScene(1);
	}
}
