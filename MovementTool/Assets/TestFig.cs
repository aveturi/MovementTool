﻿using UnityEngine;
using System.Collections;

public class TestFig : MonoBehaviour {

	public GameObject marker;
	Movement movement;

	// Use this for initialization
	void Start () {
		movement = new Movement (this.gameObject);

		movement.AddClockwiseCircle (Vector2.zero, new Vector2 (5, 0), Mathf.Deg2Rad * 360f, 3);
		movement.ChainCounterClockwiseCircle (new Vector2 (-5, 0), Mathf.Deg2Rad * 360f, 3);

		Debug.Log (movement [0]);
		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.SetRepeat ();
		movement.Start ();
	}
	
	// Update is called once per frame
	void Update () {

		movement.Update ();
	}
}
