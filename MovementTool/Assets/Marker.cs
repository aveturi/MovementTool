﻿using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {

	int life = 100;
	// Use this for initialization
	void Start () {
	
	}
	

	void FixedUpdate () {
		life--;
		if (life == 0) {
			Destroy(this.gameObject);
		}
	}
}
