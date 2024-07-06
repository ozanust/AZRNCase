using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private UIController uiController;
    [SerializeField] private GameConfigSO gameConfig;
    [SerializeField] private GoalTarget goalTarget;
    [SerializeField] private GoalPlane goalPlane;

    [SerializeField] private Transform ballPoolParent;
    [SerializeField] private Transform shootingPosition;
    [SerializeField] private float secondsBetweenShoots;

    // Using a list instead of array in sake of ease to use
    List<Ball> ballPool = new List<Ball>();

    private int score;
    private int health;
    bool isShootStarted;
    bool isShot;
    bool isPlayerMove;
    bool lastShotMade;
    float shootingTimer;
    float nextShootTimer;
    float lastShootTimer;

    Ball activeBall;
    GoalTarget activeTarget;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Vector2 swipeDirection;

    private void Start()
	{
        InitializeBallPool();
        health = gameConfig.PlayerHealth;

        Ball tempBall = GetBall();
        tempBall.transform.position = shootingPosition.position;
        tempBall.Appear();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (isPlayerMove && touch.phase == TouchPhase.Began)
            {
                Debug.Log("touch began");
                startTouchPosition = touch.position;
                isShootStarted = true;
            }
            
            if (isShootStarted && touch.phase == TouchPhase.Ended)
            {
                Debug.Log("touch ended");
                endTouchPosition = touch.position;
                // Shoot if there is enough distance dragged with mouse
                if ((endTouchPosition - startTouchPosition).magnitude > 2)
                {
                    swipeDirection = (endTouchPosition - startTouchPosition).normalized;
                    activeBall.Shoot(swipeDirection, gameConfig.ShootSpeed);
                    isShot = true;
                    health -= 1;

                    if (health == 0)
                    {
                        lastShotMade = true;
                    }
                }
                isShootStarted = false;
            }
        }

#if UNITY_EDITOR
        Vector3 mousePosition = Input.mousePosition;

        if (health > 0 && isPlayerMove && Input.GetMouseButtonDown(0)) // Check if mouse click began
        {
            Debug.Log("Mouse click began");
            startTouchPosition = mousePosition;
            isShootStarted = true;
        }

        if (isShootStarted && Input.GetMouseButtonUp(0)) // Check if mouse click ended
        {
            Debug.Log("Mouse click ended");
            endTouchPosition = mousePosition;
            if ((endTouchPosition - startTouchPosition).magnitude > 200)
            {
                ShootBall();
                isShot = true;
                health -= 1;

                if (health == 0)
                {
                    lastShotMade = true;
                }
            }
            isShootStarted = false;
        }
#endif

        if (isShootStarted)
		{
            shootingTimer += Time.deltaTime;
            if (shootingTimer >= gameConfig.ShootingTime)
            {
                Debug.Log("shooting cancelled");
                isShootStarted = false;
                shootingTimer = 0;
            }
		}

		if (isShot)
		{
            nextShootTimer += Time.deltaTime;
            if (nextShootTimer > secondsBetweenShoots)
            {
                Ball tempBall = GetBall();
                tempBall.gameObject.transform.position = shootingPosition.position;
                tempBall.Appear();
                CreateRandomTarget();
                nextShootTimer = 0;
                isShot = false;
            }
		}

		if (lastShotMade)
		{
            lastShootTimer += Time.deltaTime;
            if (lastShootTimer > secondsBetweenShoots)
            {
                uiController.ShowGameOverScreen();
                lastShootTimer = 0;
                lastShotMade = false;
            }
        }
    }

    private void InitializeBallPool()
	{
        // Adding 1 as a buffer
        int ballsToInstantiate = gameConfig.PlayerHealth + 1;

        for(int i = 0; i < ballsToInstantiate; i++)
		{
            CreateNewBall();
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

    private void OnBallDisappear(Ball ball)
	{
        ball.transform.position = shootingPosition.position;
	}

    private void OnBallAppear(Ball ball)
    {
        isPlayerMove = true;
        activeBall = ball;
        Debug.Log("player can make move");
    }

    private Ball GetBall()
	{
        for(int i = 0; i < ballPool.Count; i++)
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

    private void ShootBall()
	{
        Vector3 deltaVector = endTouchPosition - startTouchPosition;
        swipeDirection = deltaVector.normalized;
        Vector3 weightedDirection = new Vector3(swipeDirection.x * Mathf.Abs(deltaVector.x) * 3 / Screen.currentResolution.width, swipeDirection.y * Mathf.Abs(deltaVector.y) / Screen.currentResolution.height, swipeDirection.y * Mathf.Abs(deltaVector.y) / Screen.currentResolution.height);
        activeBall.Shoot(weightedDirection, gameConfig.ShootSpeed);
    }

    private void CreateRandomTarget()
    {
        Vector3 randomTargetPosition = new Vector3(
            Random.Range(goalPlane.TopLeft.x + goalTarget.Radius, goalPlane.TopRight.x - goalTarget.Radius),
            Random.Range(goalPlane.TopLeft.y - goalTarget.Radius, goalPlane.BottomLeft.y + goalTarget.Radius),
            goalPlane.TopRight.z);

        if (activeTarget != null)
        {
            activeTarget.gameObject.transform.position = randomTargetPosition;
            return;
        }

        activeTarget = Instantiate(
            goalTarget,
            new Vector3(
            Random.Range(goalPlane.TopLeft.x + goalTarget.Radius, goalPlane.TopRight.x - goalTarget.Radius),
            Random.Range(goalPlane.TopLeft.y - goalTarget.Radius, goalPlane.BottomLeft.y + goalTarget.Radius),
            goalPlane.TopRight.z),
            goalTarget.transform.rotation);
    }
}
