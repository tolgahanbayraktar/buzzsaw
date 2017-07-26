using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform player;
	public Vector3 _min, _max;
	public BoxCollider2D bounds;
	public Vector2 margin;
	public Vector2 smooting;

	public void Start()
	{
		_min = bounds.bounds.min;
		_max = bounds.bounds.max;

	}

	void Update()
	{
		var x = transform.position.x;
		var y = transform.position.y;

		x = Mathf.Lerp (x, player.position.x, smooting.x * Time.deltaTime);
		y = Mathf.Lerp (y, player.position.y, smooting.x * Time.deltaTime);

		var cameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);
			
		if(Mathf.Abs(x-player.position.x) > margin.x)
			x = Mathf.Clamp (x, _min.x+cameraHalfWidth, _max.x-cameraHalfWidth);

		if(Mathf.Abs(y-player.position.y) > margin.y)
		 	y = Mathf.Clamp (y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

		transform.position = new Vector3 (x, y, transform.position.z);
	}
}
