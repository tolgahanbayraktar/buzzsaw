using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance{ get; private set;}
	public Player Player{ get; private set;}

	List<Checkpoint> _checkpoints;
	int _currentCheckPointIndex;

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		_checkpoints = FindObjectsOfType<Checkpoint> ().OrderBy (t => t.transform.position.x).ToList ();

		//En az bir adet checkpoint var ise
		_currentCheckPointIndex = _checkpoints.Count > 0 ? 0 : -1;

		Player = FindObjectOfType<Player> ();
	}

	public void Update()
	{
		// En son checkpointe geldi mi eğer geldiyse retun de çık
		var isAtLastCheckpoint = _currentCheckPointIndex + 1 >= _checkpoints.Count;
		if (isAtLastCheckpoint)
			return;

		// Birsonraki checkpointin mesafesini ölçüyor.
		var distanceToNextCheckpoint = _checkpoints [_currentCheckPointIndex + 1].transform.position.x - Player.transform.position.x;

		Debug.Log (distanceToNextCheckpoint);

		// Bir sonraki checkpointe gelmiş ise current indexi 1 arttır
		if (distanceToNextCheckpoint >= 0)
			return;

		_currentCheckPointIndex++;
	}

	public void KillPlayer()
	{
		StartCoroutine (KillPlayerCo());
	}

	IEnumerator KillPlayerCo()
	{
		Player.Kill ();

		yield return new WaitForSeconds (1.5f);


		_checkpoints [_currentCheckPointIndex].SpawnPlayer (Player);

	}
}
