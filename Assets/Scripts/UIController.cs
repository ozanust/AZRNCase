using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
	[SerializeField] private Button restartButton;
	[SerializeField] private RectTransform gameOverPanel;
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private TMP_Text healthText;
	[SerializeField] private TMP_Text scoreTextOnGameOver;

	private void Start()
	{
		restartButton.onClick.AddListener(OnClickRestart);
	}

	public void UpdateScore(int newScore)
	{
		scoreText.text = string.Format("{0}: {1}", "Score", newScore.ToString());
		scoreTextOnGameOver.text = string.Format("{0}: {1}", "Score", newScore.ToString());
	}

	public void UpdateHealth(int newHealth)
	{
		healthText.text = string.Format("{0}: {1}", "Health", newHealth.ToString());
	}

	public void ShowGameOverScreen()
	{
		RectTransform canvasRect = gameOverPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		Vector2 canvasCenter = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);

		gameOverPanel.DOMove(canvasCenter, 0.5f).SetEase(Ease.InOutQuad);
	}

	private void OnClickRestart()
	{
		SceneManager.LoadScene(0);
	}
}
