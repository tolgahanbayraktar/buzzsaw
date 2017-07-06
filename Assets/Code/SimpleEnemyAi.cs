using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyAi : MonoBehaviour {
	public float Speed=9;
	CharacterController2D _controller;
	Vector2 _direction;

	public void Start()
	{
		_controller = GetComponent<CharacterController2D> ();
		_direction = new Vector2 (-1, 0);
	}

	public void Update()
	{
		_controller.SetHorizontalForce (_direction.x * Speed);

		if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight)) {
			_direction = -_direction;
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}



}
