using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControllerParameters2D {

	public enum JumpBehaviour{
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump
	}
	public JumpBehaviour JumpRestrictions;
	public Vector2 MaxVelocity = new Vector2 (float.MaxValue, float.MaxValue);
	[Range(0,90)]
	public float SlopeLimit = 30;
	public float Gravity = -25f;
	public float JumpFrequency=0.025f;
	public float JumpMagnitude = 16f;
}
