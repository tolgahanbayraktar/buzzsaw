﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITakeDamage {

	public float MaxSpeed = 8;
	public float SpeedAccelerationOnGround=10f;
	public float SpeedAccelerationInAir = 5f;
	public bool IsDead{ get; private set;}
	public GameObject OuchEffect;
	public int MaxHealth=100;
	public int Healt{ get; private set;}
	public Projectile Projectile;
	public Transform ProjectileFireLocation;
	public GameObject FireProjectileEffect;
	public float FireRate=0.5f;


	CharacterController2D _controller;
	float _normalizedHorizontalSpeed;
	bool _isFacingRight;
	float _canFireIn;

	public void Awake()
	{
		Healt = MaxHealth;
	}

	void Start()
	{
		_controller = GetComponent<CharacterController2D> ();
		_isFacingRight = transform.localScale.x > 0;
	}

	void Update()
	{
		_canFireIn -= Time.deltaTime;

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

		if (Input.GetMouseButtonDown (0)) {
			FireProjectile ();
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
		Healt = MaxHealth;
	}

	public void Kill()
	{
		_controller.HandleCollisions = false;
		_controller.SetForce (new Vector2 (0, 10));
		IsDead = true;
		GetComponent<Collider2D> ().enabled = false;
		Healt = 0;
	}

	public void TakeDamage(int damage, GameObject instigator)
	{
		// Playera bir obje çarpar ve hasar verirse
		Instantiate (OuchEffect, transform.position, transform.rotation);
		Healt -= damage;

		if (Healt <= 0)
			LevelManager.Instance.KillPlayer ();

		FloatingText.Show (string.Format ("-{0}", damage), "PlayerTakeDamageText",
			new FromWorldPointTextPositioner (Camera.main, transform.position, 2f, 60f));
	}

	private void FireProjectile ()
	{
		// Sürekli ateş etmemesi için
		if (_canFireIn > 0)
			return;

		if (FireProjectileEffect != null) {
			var effect = (GameObject)Instantiate (FireProjectileEffect, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
			effect.transform.parent = transform;
		}
		var projectile= (Projectile)Instantiate (Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
		var direction = _isFacingRight ? Vector2.right : -Vector2.right;
		projectile.Initialize (gameObject, direction, _controller.Velocity);

		_canFireIn = FireRate;
	}
		
}
