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

	/**
	 * Levelmanager tarafından çağırılıyor
	 * 
	*/
	public void PlayerHitCheckpoint()
	{
		
		StartCoroutine (PlayerHitCheckpointCo(LevelManager.Instance.CurrentTimeBonus));
	}

	public IEnumerator PlayerHitCheckpointCo(int bonus)
	{
		FloatingText.Show ("Checkpoint!", "CheckpointText", new CenteredTextPositioner(.5f));
		yield return new WaitForSeconds (.5f);

		FloatingText.Show (string.Format ("+{0} Time bonus", bonus), "CheckPointText", new CenteredTextPositioner (.5f));
	}
}
