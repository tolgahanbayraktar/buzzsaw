using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float MaxSpeed = 8;
	public float SpeedAccelerationOnGround=10f;
	public float SpeedAccelerationInAir = 5f;
	public bool IsDead{ get; private set;}

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
		if (!IsDead)
			HandleInput ();
		
		var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

		if (IsDead)
			_controller.SetHorizontalForce (0);
		else
			_controller.SetHorizontalForce (Mathf.Lerp(_controller.Velocity.x,_normalizedHorizontalSpeed*MaxSpeed, Time.deltaTime*movementFactor));
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

		if (_controller.CanJump && Input.GetKeyDown (KeyCode.Space) ) {
			_controller.Jump ();
		}
			
	}

	void Flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}

	public void RespawnAt(Transform spawnPoint)
	{
		if (!_isFacingRight)
			Flip ();

		IsDead = false;
		GetComponent<Collider2D> ().enabled = true;
		_controller.HandleCollisions = true;
		transform.position = spawnPoint.position;
	}

	public void Kill()
	{
		_controller.HandleCollisions = false;
		_controller.SetForce (new Vector2 (0, 10));
		IsDead = true;
		GetComponent<Collider2D> ().enabled = false;
	}
}
