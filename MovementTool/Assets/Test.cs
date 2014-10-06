using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	Movement movement;

	public GameObject marker;

	void Awake(){
		this.transform.position = Vector3.zero;
	}

	// Use this for initialization
	void Start () {
	
		float dur = 1;
			
		Vector2 p0 = new Vector2 (0, 0);
		Vector2 p1 = new Vector2 (5, 0);
		Vector2 p2 = new Vector2 (5, -5);
		Vector2 p3 = new Vector2 (-5, 0);
		Vector2 p4 = new Vector2 (-5, 5);

		Vector2 d1 = new Vector2 (10,0);
		Vector2 d2 = new Vector2 (-10, 0);

		movement = new Movement (this.gameObject);
		movement.AddPrimitive (Movement.Type.Line, p0, p1, dur);
		movement.AddPrimitive (Movement.Type.Curve, p1, p2, dur, d1);
		movement.AddPrimitive (Movement.Type.Line, p2, p0, dur);
		movement.AddPrimitive (Movement.Type.Line, p0, p3, dur);
		movement.AddPrimitive (Movement.Type.Curve, p3, p4, dur, d2);
		movement.AddPrimitive (Movement.Type.Line, p4, p0, dur);

		movement.SetRepeat ();
		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.Start ();

	}

	void FixedUpdate () {
		movement.Update ();
	}
}
