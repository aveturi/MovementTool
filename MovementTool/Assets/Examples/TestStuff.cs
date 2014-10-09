using UnityEngine;
using System.Collections;

public class TestStuff : MonoBehaviour {

	public GameObject marker;
	Movement m;
	// Use this for initialization
	void Start () {
		m = new Movement (this.gameObject);

		float dur = 1;
		Vector2 s1 = new Vector2 (-5, 5);
		Vector2 s2 = new Vector2 (0, 5);
		Vector2 s3 = new Vector2 (5, 5);
		Vector2 s4 = new Vector2 (10, 5);
		Vector2 s5 = new Vector2 (2.5f, -15);

		Vector2 d1 = new Vector2 (-2.5f, 10);
		Vector2 d2 = new Vector2 (2.5f, 10);
		Vector2 d3 = new Vector2 (7.5f, 10);
		Vector2 d4 = new Vector2 (12, 0);
		Vector2 d5 = new Vector2 (-7, 0);

		Vector2 c1 = new Vector2 (2.5f, -18);

		m.AddCurve (s1, s2, dur, d1);
		m.AddCurve (s2, s3, dur, d2);
		m.AddCurve (s3, s4, dur, d3);
		m.AddCurve (s4, s5, dur*2, d4);
		m.AddCounterClockwiseCircle (s5, c1, Mathf.Deg2Rad * 360, dur);
		m.AddCurve (s5, s1, dur * 2, d5);


		m.setMarker (marker);
		m.ToggleTrail ();
		m.SetRepeat ();

		m.ShiftMovementByPoint(new Vector2(-5,3));
		//m.PostMovement ("http://localhost/", "HeartThing");
		m.Start ();

	}
	
	// Update is called once per frame
	void Update () {
		m.Update ();
	}
}
