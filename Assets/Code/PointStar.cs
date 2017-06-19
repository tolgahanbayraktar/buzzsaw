using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointStar : MonoBehaviour, IPlayerRespawnListener {

	public GameObject Effect;
	public int PointToAdd = 10;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Player> () == null)
			return;

		GameManager.Instance.AddPoints (PointToAdd);
		Instantiate (Effect, transform.position, transform.rotation);
		gameObject.SetActive(false);
	}

	public void OnPlayerRespawnInThisCheckpoint()
	{
		gameObject.SetActive (true);
	}
}
