﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	public Player player;
	public Transform ForegroundSprite;
	public Color MaxHealthColor=new Color(255/255f,63/255f,63/255f);
	public Color MinHealthColor=new Color(64/255f,137/255f,255/255f);
	public SpriteRenderer ForegroundRenderer;


	public void Update()
	{
		var healthPercent = player.Healt / (float)player.MaxHealth;
		ForegroundSprite.localScale = new Vector3 (healthPercent, 1, 1);
		ForegroundRenderer.color = Color.Lerp (MaxHealthColor, MinHealthColor, healthPercent);
	}
}
