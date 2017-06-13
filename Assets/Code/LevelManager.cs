using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance{ get; private set;}
	public Player Player{ get; private set;}

	List<Checkpoint> _checkpoints;

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		_checkpoints = FindObjectsOfType<Checkpoint> ().OrderBy (t => t.transform.position.x).ToList ();
		Player = FindObjectOfType<Player> ();
	}

	public void KillPlayer()
	{
		StartCoroutine (KillPlayerCo());
	}

	IEnumerator KillPlayerCo()
	{
		Player.Kill ();

		yield return new WaitForSeconds (1.5f);


		_checkpoints [0].SpawnPlayer (Player);

	}
}
