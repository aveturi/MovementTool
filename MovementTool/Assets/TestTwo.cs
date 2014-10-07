using UnityEngine;
using System.Collections;

public class TestTwo : MonoBehaviour {

	Movement movement;
	public GameObject marker;

	// Use this for initialization
	void Start () {
		movement = new Movement (this.gameObject);

		float dur = 0.5f;

		Vector2 p1 = new Vector2 (-7.5f, 5);
		Vector2 p2 = new Vector2 (-2.5f, 5);
		Vector2 p3 = new Vector2 (2.5f, 5);
		Vector2 p4 = new Vector2 (7.5f, 5);

		Vector2 d1 = new Vector2 (-10f, 10);
		Vector2 d2 = new Vector2 (0f, 10);
		Vector2 d3 = new Vector2 (10f, 10);
		Vector2 d4 = new Vector2 (0f, -20);

		movement.AddPrimitive (Movement.Type.Curve, p1, p2, dur, d1);
		movement.AddPrimitive (Movement.Type.Curve, p2, p3, dur, d2);
		movement.AddPrimitive (Movement.Type.Curve, p3, p4, dur, d3);
		movement.AddPrimitive (Movement.Type.Curve, p4, p1, dur*2, d4);

		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.SetRepeat ();
		movement.Start ();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		movement.Update ();
	}
}
