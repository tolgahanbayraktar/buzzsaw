using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointStar : MonoBehaviour, IPlayerRespawnListener {

	public GameObject Effect;
	public int PointToAdd = 10;
	public AudioClip HitStarSound;
	public Animator Animator;
	public SpriteRenderer Renderer;

	private bool _isCollected;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (_isCollected)
			return;
		
		if (other.GetComponent<Player> () == null)
			return;

		AudioSource.PlayClipAtPoint (HitStarSound, transform.position);
		GameManager.Instance.AddPoints (PointToAdd);
		Instantiate (Effect, transform.position, transform.rotation);


		FloatingText.Show (string.Format ("+{0}", PointToAdd), "PointStarText", 
			new FromWorldPointTextPositioner (Camera.main, transform.position, 1.5f, 50));

		if (Animator != null) {
			_isCollected = true;
			Animator.SetTrigger ("Collect");
		}

	}

	public void FinishAnimationEvent()
	{
		Renderer.enabled = false;
		Animator.SetTrigger ("Reset");
	}

	public void OnPlayerRespawnInThisCheckpoint()
	{
		_isCollected = false;
		Renderer.enabled = true;
	}
}
