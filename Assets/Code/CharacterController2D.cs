using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	const float SkinWidht=0.02f;
	const int TotalHorizontalRays = 8;
	const int TotalVerticalRays = 4;

	public LayerMask PlatformMask;
	public Vector2 Velocity{ get { return _velocity; }}
	public ControllerState2D State{ get; private set;}
	public ControllerParameters2D DefaultParameters;
	public ControllerParameters2D Parameters{ get { return _OverrideParameters ?? DefaultParameters; }}
	public bool CanJump
	{
		get 
		{ 
			if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehaviour.CanJumpAnywhere) {
				return _jumpIn <= 0;
			}

			if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehaviour.CanJumpOnGround) {
				return State.IsGrounded;
			}

			return false;
		}
	}

	BoxCollider2D _boxCollider;
	Vector2 _velocity;
	Vector3 _localScale;
	Vector3 _raycastBottomRight;
	Vector3 _raycastBottomLeft;
	Vector3 _raycastTopLeft;
	Transform _transform;
	ControllerParameters2D _OverrideParameters=null;

	float _jumpIn;
	float _verticalDistanceBetweenRays;
	float _horizontalDistanceBetweenRays;

	void Awake()
	{
		State = new ControllerState2D ();
		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = transform.localScale;
		_transform = transform;

		var ColliderHeight = _boxCollider.size.y * Mathf.Abs (_localScale.y) - (2 * SkinWidht);
		_verticalDistanceBetweenRays = ColliderHeight / (TotalHorizontalRays - 1);

		var ColliderWidth = _boxCollider.size.x * Mathf.Abs (_localScale.x) - (2 * SkinWidht);
		_horizontalDistanceBetweenRays = ColliderWidth / (TotalVerticalRays - 1);
	}

	public void LateUpdate()
	{
		_jumpIn -= Time.deltaTime;
		_velocity.y += Parameters.Gravity * Time.deltaTime;
		Move (Velocity*Time.deltaTime);
	}

	private void Move(Vector2 deltaMovement)
	{
		CalculateRayOrigins ();
		MoveHorizontally (ref deltaMovement);
		MoveVertically (ref deltaMovement);

		_transform.Translate (deltaMovement, Space.World);
	}

	void MoveHorizontally(ref Vector2 deltaMovement)
	{
		
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidht;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;



		for(var i=0; i < TotalHorizontalRays; i++)
		{
			var rayVector = new Vector2 (rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);

			if (!raycastHit)
				continue;

			deltaMovement.x = raycastHit.point.x - rayVector.x;
			if (isGoingRight) {
				deltaMovement.x -= SkinWidht;
				State.IsCollidingRight = true;
			} else {
				deltaMovement.x += SkinWidht;
				State.IsCollidingLeft = true;
			}
			
			Debug.DrawRay (rayVector, rayDirection, Color.yellow);	
		}

	}

	void MoveVertically(ref Vector2 deltaMovement)
	{

		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidht;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
		rayOrigin.x += deltaMovement.x;

		for (var i = 0; i < TotalVerticalRays; i++) {

			var rayVector = new Vector2 (rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);

			if (!raycastHit)
				continue;
		
			deltaMovement.y = raycastHit.point.y - rayVector.y;
			if (isGoingUp) {
				deltaMovement.y -= SkinWidht;
				State.IsCollidingAbove = true;
			} else {
				deltaMovement.y += SkinWidht;
				State.IsCollidingBelow = true;
			}
			
			Debug.DrawRay (rayVector, rayDirection, Color.yellow);
		}
	}

	void CalculateRayOrigins()
	{
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y))/2;
		var center = new Vector2 (_boxCollider.offset.x*_localScale.x, _boxCollider.offset.y*_localScale.y);

		_raycastBottomRight = _transform.position + new Vector3 (center.x + size.x - SkinWidht, center.y - size.y + SkinWidht);
		_raycastBottomLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidht, center.y - size.y + SkinWidht);
		_raycastTopLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidht, center.y + size.y - SkinWidht);
	}

	public void SetHorizontalForce(float x)
	{
		_velocity.x = x;
	}

	public void Jump()
	{
		AddForce (new Vector2(0, Parameters.JumpMagnitude));
		_jumpIn = Parameters.JumpFrequency;
	}

	public void AddForce(Vector2 force)
	{
		_velocity += force;
	}


}
