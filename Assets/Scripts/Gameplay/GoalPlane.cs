using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlane : MonoBehaviour
{
	[SerializeField] Collider planeCollider;

	Vector3 topLeft;
	Vector3 topRight;
	Vector3 bottomLeft;
	Vector3 bottomRight;

	public Vector3 TopLeft => topLeft;
	public Vector3 TopRight => topRight;
	public Vector3 BottomLeft => bottomLeft;
	public Vector3 BottomRight => bottomRight;

	private void Awake()
	{
		CalculateVertices();
	}

	private void CalculateVertices()
	{
		Vector3 center = planeCollider.bounds.center;
		Vector3 size = planeCollider.bounds.size;

		// Calculate the global corner positions of the BoxCollider
		topLeft = center + new Vector3(-size.x, size.y, -size.z) * 0.5f;
		topRight = center + new Vector3(size.x, size.y, -size.z) * 0.5f;
		bottomLeft = center + new Vector3(-size.x, -size.y, -size.z) * 0.5f;
		bottomRight = center + new Vector3(size.x, -size.y, -size.z) * 0.5f;
	}
}
