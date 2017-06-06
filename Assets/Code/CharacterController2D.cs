using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	public LayerMask PlatformMask;
	public Vector2 Velocity{ get { return _velocity; }}

	const float SkinWidht=0.02f;

	BoxCollider2D _boxCollider;
	Vector2 _velocity;
	Vector3 _localScale;
	Vector3 _raycastBottomRight;
	Transform _transform;

	void Awake()
	{
		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = transform.localScale;
		_transform = transform;
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
		var rayOrigin = _raycastBottomRight;
		var rayVector = rayOrigin;
		var raycastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);

		if (!raycastHit)
			return;

		deltaMovement.x = raycastHit.point.x - rayVector.x;
		deltaMovement.x -= SkinWidht;

		Debug.DrawRay (rayVector, rayDirection, Color.yellow);
		Debug.Log (rayDistance);
	}

	void CalculateRayOrigins()
	{
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y));
		var center = new Vector2 (_boxCollider.offset.x*_localScale.x, _boxCollider.offset.y*_localScale.y);

		_raycastBottomRight = _transform.position + new Vector3 (center.x + size.x - SkinWidht, center.y - size.y + SkinWidht, 0);
		//Debug.Log (_raycastBottomRight);
	}

	public void SetHorizontalForce(float x)
	{
		_velocity.x = x;
	}
}
