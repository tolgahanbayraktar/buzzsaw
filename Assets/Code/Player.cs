using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private CharacterController2D _controller;
	private float _normalizedHorizontalSpeed;

	void Start()
	{
		_controller = GetComponent<CharacterController2D> ();
	}

	void Update()
	{
		HandleInput ();
	}

	void HandleInput()
	{
		if (Input.GetKey (KeyCode.D)) {
			_normalizedHorizontalSpeed = 1;
		} else if (Input.GetKey (KeyCode.A)) {
			_normalizedHorizontalSpeed = -1;
		} else {
			_normalizedHorizontalSpeed = 0;
		}

		_controller.SetHorizontalForce (_normalizedHorizontalSpeed);
	}
}
