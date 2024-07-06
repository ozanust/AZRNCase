using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private UIController uiController;
    [SerializeField] private GameConfigSO gameConfig;
    [SerializeField] private GoalTarget goalTarget;

    [SerializeField] private Transform ballPoolParent;
    [SerializeField] private Transform shootingPosition;
    [SerializeField] private Transform ballPosition;
    [SerializeField] private float secondsBetweenShoots;

    // Using a list instead of array in sake of ease to use
    List<Ball> ballPool = new List<Ball>();

    private int score;
    private int health;

	private void Start()
	{
       
	}

    private void InitializeBallPool()
	{
        // Adding 1 as a buffer
        int ballsToInstantiate = gameConfig.PlayerHealth + 1;

        for(int i = 0; i < ballsToInstantiate; i++)
		{
            Ball tempBall = Instantiate(ball, ballPoolParent);
            ballPool.Add(tempBall);
		}
	}

    private void OnBallHitTarget()
	{
        score += gameConfig.ExtraPointsPerTargetHit;
        uiController.UpdateScore(score);
	}

    private void OnBallHitGoal()
	{
        score += gameConfig.PointsPerGoal;
        uiController.UpdateScore(score);
    }

    private void OnBallDisappear()
	{

	}
}
