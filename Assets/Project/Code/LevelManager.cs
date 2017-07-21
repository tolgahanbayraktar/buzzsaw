using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance{ get; private set;}
	public Player Player{ get; private set;}
	public int BonusCutoffSeconds=10;
	public int BonusSecondMultiplier=3;
	public int CurrentTimeBonus {
		get
		{ 
			var secondDifference = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
			return Mathf.Max (0, secondDifference) * BonusSecondMultiplier;
		}
	}
	public TimeSpan RunningTime{
		get
		{
			return DateTime.UtcNow - _started;
		}
	}

	List<Checkpoint> _checkpoints;
	int _currentCheckPointIndex;
	DateTime _started;
	int _savePoints;


	public void Awake()
	{
		_savePoints = GameManager.Instance.points;
		Instance = this;
	}

	public void Start()
	{
		_started = DateTime.UtcNow;

		_checkpoints = FindObjectsOfType<Checkpoint> ().OrderBy (t => t.transform.position.x).ToList ();

		//En az bir adet checkpoint var ise
		_currentCheckPointIndex = _checkpoints.Count > 0 ? 0 : -1;

		Player = FindObjectOfType<Player> ();

		var listeners = FindObjectsOfType<MonoBehaviour> ().OfType<IPlayerRespawnListener> ();
		foreach (var listener in listeners) {
			for (var i = _checkpoints.Count - 1; i >= 0; i--) {
				var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints [i].transform.position.x;
				if (distance < 0)
					continue;

				_checkpoints [i].AssignObjectToCheckpoint (listener);
				break;
			}
		}
	}

	public void Update()
	{
		
		// En son checkpointe geldi mi eğer geldiyse retun de çık
		var isAtLastCheckpoint = _currentCheckPointIndex + 1 >= _checkpoints.Count;
		if (isAtLastCheckpoint)
			return;

		// Birsonraki checkpointin mesafesini ölçüyor.
		var distanceToNextCheckpoint = _checkpoints [_currentCheckPointIndex + 1].transform.position.x - Player.transform.position.x;

		//Debug.Log (distanceToNextCheckpoint);


		if (distanceToNextCheckpoint >= 0)
			return;
		
		// Bir sonraki checkpointe gelmiş ise current indexi 1 arttır
		_currentCheckPointIndex++;
		_checkpoints [_currentCheckPointIndex].PlayerHitCheckpoint ();

		GameManager.Instance.AddPoints (CurrentTimeBonus);
		_savePoints = GameManager.Instance.points;
		_started = DateTime.UtcNow;
	}

	public void GotoNextLevel(string levelName)
	{
		StartCoroutine (GotoNextLevelCo (levelName));
	}

	private IEnumerator GotoNextLevelCo(string levelName)
	{
		Player.FinishLevel ();
		GameManager.Instance.AddPoints (CurrentTimeBonus);

		FloatingText.Show ("Level Complete!", "CheckpointText", new CenteredTextPositioner (.2f));
		yield return new WaitForSeconds (1f);

		FloatingText.Show (string.Format ("{0} points!", GameManager.Instance.points), "CheckpointText", new CenteredTextPositioner (.10f));
		yield return new WaitForSeconds (5f);

		if (string.IsNullOrEmpty (levelName))
			Application.LoadLevel ("_StartScreen");

		Application.LoadLevel (levelName);
	}

	public void KillPlayer()
	{
		StartCoroutine (KillPlayerCo());
	}

	IEnumerator KillPlayerCo()
	{
		Player.Kill ();
		yield return new WaitForSeconds (1.5f);

		if (_currentCheckPointIndex != -1)
			_checkpoints [_currentCheckPointIndex].SpawnPlayer (Player);

		_started = DateTime.UtcNow;
		GameManager.Instance.ResetPoints (_savePoints);
	}
}
