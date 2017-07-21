using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour {

	ParticleSystem _particleSystem;


	public void Start()
	{
		_particleSystem = GetComponent<ParticleSystem> ();
	}

	public void Update()
	{
		// Particle effectini yaptıktan sonra hierarchy kısmından kaybolacak
		if (_particleSystem.isPlaying)
			return;

		Destroy (gameObject);
	}
}
