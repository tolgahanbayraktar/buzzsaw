﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public void SpawnPlayer(Player player)
	{
		player.RespawnAt (transform);
	}
}
