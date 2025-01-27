using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTarget : MonoBehaviour
{
	[SerializeField] private ParticleSystem explosionEffect;

	// Assuming this is a cylinder
	public float Radius => transform.localScale.x / 2f;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == Constants.BALL_TAG)
		{
			explosionEffect.gameObject.transform.SetParent(null);
			explosionEffect.gameObject.transform.localScale = new Vector3(1, 1, 1);
			explosionEffect.Play();
			Destroy(this.gameObject, 0.1f);
		}
	}
}
