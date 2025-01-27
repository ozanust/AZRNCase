using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
	[SerializeField] Rigidbody ballRigidbody;
	[SerializeField] Collider ballCollider;
	[SerializeField] float disappearTimer;

	public UnityEvent onHitTarget;
	public UnityEvent onHitGoal;
	public UnityEvent<Ball> onDisappear;
	public UnityEvent<Ball> onAppear;

	public Rigidbody BallRigidbody => ballRigidbody;

	int targetLayerIndex;
	int goalLayerIndex;

	bool isShot;
	float timer;

	private void Awake()
	{
		targetLayerIndex = LayerMask.NameToLayer(Constants.TARGET_LAYER);
		goalLayerIndex = LayerMask.NameToLayer(Constants.GOAL_LAYER);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == Constants.TARGET_TAG)
		{
			onHitTarget.Invoke();

			// To prevent having duplicate collisions
			Physics.IgnoreLayerCollision(gameObject.layer, targetLayerIndex, true);
		}

		if (collision.gameObject.tag == Constants.GOAL_TAG)
		{
			onHitGoal.Invoke();

			// To prevent having duplicate collisions
			Physics.IgnoreLayerCollision(gameObject.layer, goalLayerIndex, true);
		}
	}

	private void Update()
	{
		if (isShot)
		{
			timer += Time.deltaTime;
			if (timer > disappearTimer)
			{
				transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad).onComplete += OnFinishDisappear;
				isShot = false;
				timer = 0;
			}
		}
	}

	private void OnFinishDisappear()
	{
		ballRigidbody.velocity = new Vector3(0, 0, 0);
		ballRigidbody.angularVelocity = new Vector3(0, 0, 0);
		gameObject.SetActive(false);
		onDisappear.Invoke(this);
	}

	private void OnAppear()
	{
		ballRigidbody.useGravity = true;
		Physics.IgnoreLayerCollision(gameObject.layer, targetLayerIndex, false);
		Physics.IgnoreLayerCollision(gameObject.layer, goalLayerIndex, false);
		onAppear.Invoke(this);
	}

	public void Shoot(Vector3 direction, float power)
	{
		ballRigidbody.AddForce(direction * power, ForceMode.Impulse);
		ballRigidbody.AddTorque(direction * power, ForceMode.Impulse);
		isShot = true;
	}

	public void Appear()
	{
		gameObject.SetActive(true);
		ballRigidbody.useGravity = false;
		transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f).SetEase(Ease.OutBack).onComplete += OnAppear;
	}
}
