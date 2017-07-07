using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyAi : MonoBehaviour, ITakeDamage, IPlayerRespawnListener {
	public float Speed=9;
	public Projectile Projectile;
	public float FireRate=1;
	public GameObject DestroyedEffect;
	public Vector2 _startPosition;
	public int PointsToGivePlayer = 15;

	CharacterController2D _controller;
	Vector2 _direction;
	float _CanFireIn;

	public void Start()
	{
		_controller = GetComponent<CharacterController2D> ();
		_direction = new Vector2 (-1, 0);
		_startPosition = transform.position;
	}

	public void Update()
	{
		
		_controller.SetHorizontalForce (_direction.x * Speed);

		if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight)) {
			_direction = -_direction;
			transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}

		if ((_CanFireIn -= Time.deltaTime) > 0)
			return;

		var raycast = Physics2D.Raycast (transform.position, _direction, 30f, 1 << LayerMask.NameToLayer ("Player"));
		if (!raycast)
			return;

		var projectile = (Projectile)Instantiate (Projectile, transform.position, transform.rotation);
		projectile.Initialize (gameObject, _direction, _controller.Velocity);
		_CanFireIn = FireRate;
	}

	/*
	* Balık zarar görsün interface ile yapıyoruz.
	*/
	public void TakeDamage (int damage, GameObject instigator)
	{
		if (PointsToGivePlayer != 0) {

			// Zarar veren kim ise Projectile diye bir companenti varmı ?
			var projectile = instigator.GetComponent<Projectile> ();

			// Projectile diye bir companenti varsa ve çarpan nesneyi göndere Player mı?
			if (projectile != null && projectile.Owner.GetComponent<Player> () != null) {
				GameManager.Instance.AddPoints (PointsToGivePlayer);
				FloatingText.Show (string.Format ("+{0}!", PointsToGivePlayer), "PointStarText", new FromWorldPointTextPositioner (Camera.main, transform.position, 1.5f, 50));
			}
		}
		Instantiate (DestroyedEffect, transform.position, transform.rotation);
		gameObject.SetActive (false);
	}

	public void OnPlayerRespawnInThisCheckpoint()
	{
		// Badfish öldüğünde sağa bakıyor olabilir respawn olduğunda sola bakacak şekilde ayarlıyoruz.
		_direction = new Vector2 (-1, 0);
		transform.localScale = new Vector3 (1, 1, 1);
		transform.position = _startPosition;
		gameObject.SetActive (true);
	}

}
