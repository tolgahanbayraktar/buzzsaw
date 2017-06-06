using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float MaxSpeed = 8;

	CharacterController2D _controller;
	float _normalizedHorizontalSpeed;
	bool _isFacingRight;

	void Start()
	{
		_controller = GetComponent<CharacterController2D> ();
		_isFacingRight = transform.localScale.x > 0;
	}

	void Update()
	{
		HandleInput ();
		_controller.SetHorizontalForce (Mathf.Lerp(_controller.Velocity.x,_normalizedHorizontalSpeed*MaxSpeed, Time.deltaTime));
	}

	void HandleInput()
	{
		if (Input.GetKey (KeyCode.D)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight)
				Flip ();
		} else if (Input.GetKey (KeyCode.A)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight)
				Flip ();
		} else {
			_normalizedHorizontalSpeed = 0;
		}
			
	}

	void Flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}

}
