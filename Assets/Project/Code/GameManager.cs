using UnityEngine;

public class GameManager {

	public static GameManager Instance
	{
		get
		{
			return _instance ?? (_instance = new GameManager ());		
		}
	}
	public int points{ get; private set;}

	static GameManager _instance;

	GameManager()
	{
		
	}

	public void ResetPoints(int _points)
	{
		points = _points;
	}

	public void Reset()
	{
		points = 0;
	}

	public void AddPoints(int pointsToAdd)
	{
		points += pointsToAdd;
	}
}
