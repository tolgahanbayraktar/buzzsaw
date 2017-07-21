using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour {

	public string levelName;

	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.GetComponent<Player> ();
		if (player == null)
			return;

		LevelManager.Instance.GotoNextLevel (levelName);

	}
}
