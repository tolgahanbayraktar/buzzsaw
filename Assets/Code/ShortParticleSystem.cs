using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortParticleSystem : MonoBehaviour {

	public string LayerName="Particles";

	public void Start()
	{
		GetComponent<ParticleSystemRenderer> ().sortingLayerName = LayerName;
	}

}
