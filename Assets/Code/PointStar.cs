using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointStar : MonoBehaviour {

	public GameObject Effect;
	public int PointToAdd = 10;

	public void OnTriggerEnter2D(Collider2D other)
	{
		Instantiate (Effect, transform.position, transform.rotation);
		gameObject.SetActive(false);
	}
}
