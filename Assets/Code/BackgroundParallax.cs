﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour {

	public Transform[] Backgrounds;
	public float ParallaxScale=0.5f;
	public float Smoothing = 2f;
	public float ParallaxReductionFactor = 3f;

	Vector3 _lastPosition;

	public void Start()
	{
		_lastPosition = transform.position;
	}

	public void Update()
	{
		var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;

		for (var i = 0; i < Backgrounds.Length; i++) {
			var backgroundTargetPosition = Backgrounds [i].position.x + parallax*(i*ParallaxReductionFactor+1);
			Backgrounds [i].position = Vector3.Lerp (Backgrounds[i].position, new Vector3(backgroundTargetPosition, Backgrounds[i].position.y, Backgrounds[i].position.z), Smoothing*Time.deltaTime);
		}
			
		_lastPosition = transform.position;
	}
}
