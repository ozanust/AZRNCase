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
    public UnityEvent onDisappear;

    public Rigidbody BallRigidbody => ballRigidbody;

	int targetLayerIndex = LayerMask.NameToLayer("Target");
	int goalLayerIndex = LayerMask.NameToLayer("Goal");

	LayerMask targetLayer;
	LayerMask goalLayer;

	bool isShot;
	float timer;

	private void Start()
	{
		targetLayer = 1 << targetLayerIndex;
		goalLayer = 1 << goalLayerIndex;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Target")
		{
			onHitTarget.Invoke();

			// To prevent having duplicate collisions
			ballCollider.excludeLayers += targetLayer;
		}

		if (collision.gameObject.tag == "Goal")
		{
			onHitGoal.Invoke();

			// To prevent having duplicate collisions
			ballCollider.excludeLayers += goalLayer;
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
		ballCollider.excludeLayers -= targetLayer;
		ballCollider.excludeLayers -= goalLayer;
		onDisappear.Invoke();
	}

	public void Shoot(Vector3 direction, float power)
	{
		ballRigidbody.AddForce(direction * power, ForceMode.Impulse);
		isShot = true;
	}
}
