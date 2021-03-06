﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathedProjectile : MonoBehaviour, ITakeDamage {

	public GameObject DestroyEffect;
	public int PointsToGivePlayer=5;

	Transform _destination;
	float _speed;

	public void Initialize(Transform destination, float speed)
	{
		_destination = destination;
		_speed = speed;
	}

	public void Update()
	{
		transform.position = Vector3.MoveTowards (transform.position, _destination.position, Time.deltaTime * _speed);

		var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
		if (distanceSquared > 0.1f * 0.1f)
			return;

		if (DestroyEffect != null)
			Instantiate (DestroyEffect, transform.position, transform.rotation);
		
		Destroy (gameObject);
	}

	public void TakeDamage (int damage, GameObject instigator)
	{
		if (DestroyEffect != null)
			Instantiate (DestroyEffect, transform.position, transform.rotation);
		
		Destroy (gameObject);
		var projectile = instigator.GetComponent<Projectile> ();

		/**
		 * MiCROfonun çarptığı nesneyi yok etmesi ve puan yazısının çıkması
		 */
		if (projectile != null && projectile.Owner.GetComponent<Player> () != null && PointsToGivePlayer != 0) {
			GameManager.Instance.AddPoints (PointsToGivePlayer);
			FloatingText.Show(string.Format("+{0}!", PointsToGivePlayer), "PointStarText",new FromWorldPointTextPositioner(Camera.main, transform.position,1.5f,50));
		}
	}
}
