using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour {

	public float JumpMagnitude = 30f;

	public void ControllerEnter2D(CharacterController2D controller){

		controller.SetVerticalForce (JumpMagnitude);
	}
}
