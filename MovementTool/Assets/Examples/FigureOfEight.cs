using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FigureOfEight : MonoBehaviour {

	Movement movement;
	public GameObject marker;

	// Use this for initialization
	void Start () {
		float duration = 0.5f;
		Vector3 p1 = new Vector3(0,0,0);
		Vector3 p2 = new Vector3(-10,-10,0);
		Vector3 p3 = new Vector3 (-10,10, 0);
		Vector3 p4 = new Vector3 (10, -10, 0);
		Vector3 p5 = new Vector3 (10, 10, 0);

		Vector3 d1 = new Vector3 (-30, 0, 0);
		Vector3 d2 = new Vector3 (30, 0, 0);
		Vector3 d3 = new Vector3 (0, -10, 0);
		Vector3 d4 = new Vector3 (0, 10, 0);

		movement = new Movement (this.gameObject);

		movement.AddCurve ( p1, p2, duration, d3);
		movement.AddCurve(p2,p3,duration*2,d1);
		movement.AddCurve(p3,p1,duration,d4);
		movement.AddCurve(p1,p4,duration,d3);
		movement.AddCurve(p4,p5,duration*2,d2);
		movement.AddCurve(p5,p1,duration,d4);

		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.SetRepeat ();

		movement.Start ();

		movement.PostMovement ("http://localhost/", "FigofEightish");
	}

	void FixedUpdate () {
	 movement.Update ();
	}
}
