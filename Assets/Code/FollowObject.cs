using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

	public Vector2 offset;
	public Transform Following;

	public void Update()
	{
		transform.position = Following.transform.position + (Vector3)offset;	
	}
}
