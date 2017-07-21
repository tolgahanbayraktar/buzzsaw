using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	const float SkinWidht=0.02f;
	const int TotalHorizontalRays = 8;
	const int TotalVerticalRays = 4;
	static readonly float SlopeLimitTangant = Mathf.Tan (75f * Mathf.Deg2Rad);

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
	public GameObject StandingOn { get; private set;}
	public bool HandleCollisions{ get; set;}

	BoxCollider2D _boxCollider;
	Vector2 _velocity;
	Vector3 _localScale;
	Vector3 _raycastBottomRight;
	Vector3 _raycastBottomLeft;
	Vector3 _raycastTopLeft;
	Transform _transform;
	ControllerParameters2D _OverrideParameters=null;
	Vector3 _activeGlobalPlatformPoint;
	Vector3 _activeLocalPlatformPoint;
	GameObject _lastStandingOn;

	float _jumpIn;
	float _verticalDistanceBetweenRays;
	float _horizontalDistanceBetweenRays;

	void Awake()
	{
		HandleCollisions = true;
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
		// Reset yapmadan önce ayakları yere değiyorsa.
		var wasGrounded = State.IsCollidingBelow;

		State.Reset ();
		HandlePlatforms ();
		CalculateRayOrigins ();


		if (HandleCollisions) {
			if (deltaMovement.y < 0 && wasGrounded)
				HandleVerticalSlope (ref deltaMovement);

			// Yatay haraketin gereksiz yere bir sürü kez çalışmasını önlemek için
			if (Mathf.Abs (deltaMovement.x) > 0.001f)
				MoveHorizontally (ref deltaMovement);	
		
			MoveVertically (ref deltaMovement);
			CorrectHorizontalPlacement (ref deltaMovement, true);
			CorrectHorizontalPlacement (ref deltaMovement, false);
		}

		_transform.Translate (deltaMovement, Space.World);
		
		if (Time.deltaTime > 0)
			_velocity = deltaMovement / Time.deltaTime;

		if (State.IsMovingUpSlope)
			_velocity.y = 0;

		if (StandingOn != null) {
			_activeGlobalPlatformPoint = transform.position;
			_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint (transform.position);

			if (_lastStandingOn != StandingOn) {
				StandingOn.SendMessage ("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn;
			}
		}
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

			Debug.DrawRay (rayVector, rayDirection, Color.yellow);

			if (i == 0 && HandleHorizontalSlope (ref deltaMovement, Vector2.Angle (raycastHit.normal, Vector2.up), isGoingRight))
				break;

			deltaMovement.x = raycastHit.point.x - rayVector.x;
			if (isGoingRight) {
				deltaMovement.x -= SkinWidht;
				State.IsCollidingRight = true;
			} else {
				deltaMovement.x += SkinWidht;
				State.IsCollidingLeft = true;
			}
			

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

			if (!StandingOn) {
				StandingOn = raycastHit.collider.gameObject;
				//Debug.Log (StandingOn.name);
			}

			deltaMovement.y = raycastHit.point.y - rayVector.y;
			if (isGoingUp) {
				deltaMovement.y -= SkinWidht;
				State.IsCollidingAbove = true;
			} else {
				deltaMovement.y += SkinWidht;
				State.IsCollidingBelow = true;
			}

			if (!isGoingUp && deltaMovement.y > 0.0001f)
				State.IsMovingUpSlope = true;

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
	public void SetForce(Vector2 force)
	{
		_velocity = force;
	}

	public void SetVerticalForce(float y){
		_velocity.y = y;
	}

	bool HandleHorizontalSlope(ref  Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		if(Mathf.RoundToInt(angle) == 90)
			return  false;

		if (angle > Parameters.SlopeLimit) {
			deltaMovement.x = 0;
			return true;
		}

		if (_jumpIn > 0)
			return true;
		
		deltaMovement.y = Mathf.Abs(Mathf.Tan(angle*Mathf.Deg2Rad)) * deltaMovement.x;
		State.IsMovingUpSlope = false;
		State.IsCollidingBelow = true;
				
		return true;
	}

	void HandleVerticalSlope(ref Vector2 deltaMovement)
	{
		var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
		//Debug.Log (center);
		var direction = -Vector2.up;
		var slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
		var slopeRayVector = new Vector2 (center,_raycastBottomRight.y);

		var raycastHit = Physics2D.Raycast (slopeRayVector, direction, slopeDistance, PlatformMask);

		if (!raycastHit)
			return;

		// işaretleri kontrol eder
		var isMovingDownSlope = Mathf.Sign (raycastHit.normal.x) == Mathf.Sign (deltaMovement.x);
		if (!isMovingDownSlope)
			return;

		var angle = Vector2.Angle (raycastHit.normal, Vector2.up);

		// Yokuş açısı çok çok küçükse burdan çık
		if (Mathf.Abs (angle) < .0001f)
			return;

		State.IsMovingDownSlope = true;
		State.SlopeAngle = angle;

		// Playerdan gönderdiğimiz noktanın çarpma noktasından ışını gönderdiğimiz noktayı çkartıyoruz
		// deltamovement.y de ne kadar aşağı ineceğimizi hesaplıyoruz.
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
		Debug.DrawRay (slopeRayVector, direction * slopeDistance, Color.yellow);
	}

	void HandlePlatforms()
	{
		// Altımızda bir platform varmı ?
		if (StandingOn != null) {
			var newGlobalPlatformPoint = StandingOn.transform.TransformPoint (_activeLocalPlatformPoint);
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
			if (moveDistance != Vector3.zero)
				transform.Translate (moveDistance, Space.World);

			StandingOn = null;
		}
	}

	void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
	{
		var halfWidth = (_boxCollider.size.x * _localScale.x) / 2;
		var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;
		if (isRight)
			rayOrigin.x += SkinWidht - halfWidth;
		else
			rayOrigin.x += -SkinWidht + halfWidth;

		var rayDirection = isRight ? Vector2.right : -Vector2.right;
		var offset = 0f;

		for (var i = 0; i < TotalHorizontalRays - 1; i++) {
			var rayVector = new Vector2 (rayOrigin.x + deltaMovement.x, deltaMovement.y + rayOrigin.y + i * _verticalDistanceBetweenRays);

			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, halfWidth, PlatformMask);
			if (!raycastHit)
				continue;

			offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
		}

		deltaMovement.x += offset;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null)
			return;

		_OverrideParameters = parameters.Parameters;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null)
			return;

		_OverrideParameters = null;
	}
}
