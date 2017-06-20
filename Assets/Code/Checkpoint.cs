using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	List<IPlayerRespawnListener> _listeners;

	public void Awake()
	{
		_listeners = new List<IPlayerRespawnListener> ();
	}

	public void SpawnPlayer(Player player)
	{
		player.RespawnAt (transform);

		foreach (var listener in _listeners) {
			listener.OnPlayerRespawnInThisCheckpoint (); 
		}
	}

	public void AssignObjectToCheckpoint(IPlayerRespawnListener listener)
	{
		_listeners.Add (listener);
	}
}
