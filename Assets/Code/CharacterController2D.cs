using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	const float SkinWidht=0.02f;
	const int TotalHorizontalRays = 8;
	const int TotalVerticalRays = 4;

	public LayerMask PlatformMask;
	public Vector2 Velocity{ get { return _velocity; }}

	BoxCollider2D _boxCollider;
	Vector2 _velocity;
	Vector3 _localScale;
	Vector3 _raycastBottomRight;
	Vector3 _raycastBottomLeft;

	Transform _transform;
	float _verticalDistanceBetweenRays;

	void Awake()
	{
		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = transform.localScale;
		_transform = transform;

		var ColliderHeight = _boxCollider.size.y * Mathf.Abs (_localScale.y) - (2 * SkinWidht);
		_verticalDistanceBetweenRays = ColliderHeight / (TotalHorizontalRays - 1);
	}

	public void LateUpdate()
	{
		Move (Velocity*Time.deltaTime);
	}

	private void Move(Vector2 deltaMovement)
	{
		CalculateRayOrigins ();
		MoveHorizontally (ref deltaMovement);
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
			if (isGoingRight)
				deltaMovement.x -= SkinWidht;
			else
				deltaMovement.x += SkinWidht;
			
			Debug.DrawRay (rayVector, rayDirection, Color.yellow);	
		}

	}

	void CalculateRayOrigins()
	{
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y))/2;
		var center = new Vector2 (_boxCollider.offset.x*_localScale.x, _boxCollider.offset.y*_localScale.y);

		_raycastBottomRight = _transform.position + new Vector3 (center.x + size.x - SkinWidht, center.y - size.y + SkinWidht);
		_raycastBottomLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidht, center.y - size.y + SkinWidht);

	}

	public void SetHorizontalForce(float x)
	{
		_velocity.x = x;
	}
}
