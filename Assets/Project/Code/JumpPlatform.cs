using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour {

	public float JumpMagnitude = 30f;
	public AudioClip JumpSounds;

	public void ControllerEnter2D(CharacterController2D controller){

		if (JumpSounds != null)
			return;

		AudioSource.PlayClipAtPoint (JumpSounds, transform.position);
		controller.SetVerticalForce (JumpMagnitude);
	}
}
