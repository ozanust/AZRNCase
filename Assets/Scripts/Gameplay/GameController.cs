using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] private Ball ball;
	[SerializeField] private UIController uiController;
	[SerializeField] private GameConfigSO gameConfigSO;
	[SerializeField] private GoalTarget goalTarget;
	[SerializeField] private GoalPlane goalPlane;

	[SerializeField] private Transform ballPoolParent;
	[SerializeField] private Transform shootingPosition;
	[SerializeField] private float secondsBetweenShoots;

	// Using a list instead of array in sake of ease to use
	List<Ball> ballPool = new List<Ball>();

	private int score;
	private int health;

	bool isShot;
	bool isPlayerMove;
	bool lastShotMade;

	float nextShootTimer;
	float lastShootTimer;

	Ball activeBall;
	GoalTarget activeTarget;

	private GameConfig gameConfig;

	// Here using property setters to overcome duplicate code and also prevent forgetting to update UI everywhere
	private int Health { 
		get { return health; } 
		set 
		{
			health = value;
			uiController.UpdateHealth(health);
		} 
	}

	private int Score
	{
		get { return score; }
		set
		{
			score = value;
			uiController.UpdateScore(score);
		}
	}

	#region Initializers

	private void Awake()
	{
		gameConfig = gameConfigSO.GameConfig;

		Health = gameConfig.PlayerHealth;
		Score = 0;

		InitializeBallPool();
		InputHandler.OnSwipe += OnSwipe;
	}

	private void Start()
	{
		PrepareNewBall();
		CreateRandomTarget();
	}

	private void InitializeBallPool()
	{
		// Adding 1 as a buffer
		int ballsToInstantiate = gameConfig.PlayerHealth + 1;

		for (int i = 0; i < ballsToInstantiate; i++)
		{
			CreateNewBall();
		}
	}

	#endregion

	private void Update()
	{
		HandleNextShootTimer();
		HandleLastShotTimer();
	}

	#region EventListeners

	/// <summary>
	/// Taking action when we get a swipe event from user
	/// </summary>
	/// <param name="swipeVector"></param>
	private void OnSwipe(Vector2 swipeVector)
	{
		if (Health > 0 && isPlayerMove)
		{
			if (IsEligibleToShoot(swipeVector))
			{
				ShootBall(swipeVector);
				isShot = true;
				Health--;
				isPlayerMove = false;

				if (Health == 0)
				{
					lastShotMade = true;
				}
			}
			else
			{
				isPlayerMove = true;
			}
		}
	}

	private void OnBallHitTarget()
	{
		Score += gameConfig.ExtraPointsPerTargetHit;

		// Increasing health as a reward
		Health += 1;

		// Taking it to false if this was last shot
		lastShotMade = false;
		lastShootTimer = 0;
	}

	private void OnBallHitGoal()
	{
		Score += gameConfig.PointsPerGoal;
	}

	private void OnBallDisappear(Ball ball)
	{
		ball.transform.position = shootingPosition.position;
	}

	private void OnBallAppear(Ball ball)
	{
		isPlayerMove = true;
		activeBall = ball;
	}

	#endregion

	#region GameplayMechanics
	private Ball GetBall()
	{
		for (int i = 0; i < ballPool.Count; i++)
		{
			if (!ballPool[i].gameObject.activeInHierarchy)
			{
				return ballPool[i];
			}
		}

		// If all balls are in use, we increase the pool in need
		return CreateNewBall();
	}

	private Ball CreateNewBall()
	{
		Ball tempBall = Instantiate(ball, ballPoolParent);
		tempBall.onHitGoal.AddListener(OnBallHitGoal);
		tempBall.onHitTarget.AddListener(OnBallHitTarget);
		tempBall.onDisappear.AddListener(OnBallDisappear);
		tempBall.onAppear.AddListener(OnBallAppear);
		ballPool.Add(tempBall);

		return tempBall;
	}

	private void ShootBall(Vector2 swipeVector)
	{
		Vector2 swipeDirection = swipeVector.normalized;

		// Here we are giving Y axis of input to both Y and Z with different coefficients to tweak the gameplay. We divide swipe amount by screen resolution to have same force independent from the resolution.
		Vector3 weightedDirection = new Vector3(
			swipeDirection.x * Mathf.Abs(swipeVector.x) * gameConfig.ShootPowerCoefficients.x / Screen.width, 
			swipeDirection.y * Mathf.Abs(swipeVector.y) * gameConfig.ShootPowerCoefficients.y / Screen.height, 
			swipeDirection.y * Mathf.Abs(swipeVector.y) * gameConfig.ShootPowerCoefficients.z / Screen.height);

		activeBall.Shoot(weightedDirection, gameConfig.ShootSpeed);
	}

	private void CreateRandomTarget()
	{
		// Calculating a random position inside the inner abstract rectangle to spawn target. Preventing the target to overlap with goal borders.
		Vector3 randomTargetPosition = new Vector3(
			Random.Range(goalPlane.TopLeft.x + goalTarget.Radius, goalPlane.TopRight.x - goalTarget.Radius),
			Random.Range(goalPlane.TopLeft.y - goalTarget.Radius, goalPlane.BottomLeft.y + goalTarget.Radius),
			goalPlane.TopRight.z);

		// Using the same target if we haven't destroyed it on previous shoot
		if (activeTarget != null)
		{
			activeTarget.gameObject.transform.DOMove(randomTargetPosition, 0.5f);
			return;
		}

		activeTarget = Instantiate(goalTarget, randomTargetPosition, goalTarget.transform.rotation);
	}

	private void PrepareNewBall()
	{
		Ball tempBall = GetBall();
		tempBall.transform.position = shootingPosition.position;
		tempBall.Appear();
	}

	/// <summary>
	/// Check if user input provides suitable information to make a proper shoot
	/// </summary>
	/// <param name="swipeVector">Swipe distance vector</param>
	/// <returns></returns>
	private bool IsEligibleToShoot(Vector2 swipeVector)
	{
		return swipeVector.magnitude > 200 && swipeVector.y > 0;
	}

	#endregion

	#region TimingHandlers

	private void HandleNextShootTimer()
	{
		if (!isShot)
		{
			return;
		}

		nextShootTimer += Time.deltaTime;
		if (nextShootTimer > secondsBetweenShoots)
		{
			PrepareNewBall();
			CreateRandomTarget();
			ResetShootTimer();
		}
	}

	private void HandleLastShotTimer()
	{
		if (!lastShotMade)
		{
			return;
		}

		lastShootTimer += Time.deltaTime;
		if (lastShootTimer > secondsBetweenShoots)
		{
			uiController.ShowGameOverScreen();
			ResetLastShotTimer();
		}
	}

	private void ResetShootTimer()
	{
		nextShootTimer = 0;
		isShot = false;
	}

	private void ResetLastShotTimer()
	{
		lastShootTimer = 0;
		lastShotMade = false;
	}

	#endregion
}
