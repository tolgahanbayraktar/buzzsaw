using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	public Vector2 Velocity{ get { return _velocity; }}

	Vector2 _velocity;

	public void LateUpdate()
	{
		Move (Velocity*Time.deltaTime);
	}

	private void Move(Vector2 deltaMovement)
	{
		transform.Translate (deltaMovement, Space.World);
	}

	public void SetHorizontalForce(float x)
	{
		_velocity.x = x;
	}
}
