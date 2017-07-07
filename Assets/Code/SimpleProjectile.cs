using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : Projectile, ITakeDamage {

	public float TimeToLive = 10f;
	public GameObject DestroyedEffect;
	public int Damage=10;
	public int PointsToGivePlayer=5;

	public void TakeDamage(int damage, GameObject instigator)
	{
		var projectile = instigator.GetComponent<Projectile> ();

		if (projectile != null && projectile.Owner.GetComponent<Player> () != null && PointsToGivePlayer != 0) {
			GameManager.Instance.AddPoints (PointsToGivePlayer);
			FloatingText.Show(string.Format("+{0}!", PointsToGivePlayer), "PointStarText",new FromWorldPointTextPositioner(Camera.main, transform.position,1.5f,50));
		}
	}

	public void Update()
	{
		if ((TimeToLive -= Time.deltaTime) <= 0) {
			DestroyProjectile ();
			return;
		}

		transform.Translate (Direction * ((Mathf.Abs (InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);
	}

	private void DestroyProjectile()
	{
		if (DestroyedEffect != null)
			Instantiate (DestroyedEffect, transform.position, transform.rotation);

		Destroy (gameObject);
	}

	protected override void OnCollideOther(Collider2D other)
	{
		DestroyProjectile ();
	}

	protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
	{
		takeDamage.TakeDamage (Damage, gameObject);
		DestroyProjectile ();
	}
}
