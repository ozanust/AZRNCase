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
		Application.targetFrameRate = 60;
		ConfigLoader.GetConfig(OnGameConfigLoaded);
	}

	private void OnGameConfigLoaded(GameConfig config)
	{
		gameConfigSO.GameConfig = config;
		SceneManager.LoadScene(1);
	}
}
