using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {

	public GameObject marker;
	Movement m;
	// Use this for initialization
	void Start () {
		m = new Movement (this.gameObject);

		float dur = 1;
		Vector2 s1 = new Vector2 (0, 0);
		Vector2 s2 = new Vector2 (-5, 5);
		Vector2 s3 = new Vector2 (5, 5);


		Vector2 c1 = new Vector2 (0, -2);
		Vector2 c2 = new Vector2 (0, -3);
		Vector2 c3 = new Vector2 (-10, 5);
		Vector2 c4 = new Vector2 (10, 5);
		Vector2 c5 = new Vector2 (-8, 5);
		Vector2 c6 = new Vector2 (8, 5);


		m.AddLine (s1, s2, dur);
		m.ChainCounterClockwiseCircle (s1, Mathf.Deg2Rad*270f, dur*2);
		m.ChainLine (s1, dur);
		m.ChainCounterClockwiseCircle (c1, Mathf.Deg2Rad * 360f, dur);
		m.ChainCounterClockwiseCircle (c2, Mathf.Deg2Rad * 360f, dur);
		m.ChainSine (s3, dur, 2f, 2f);
		m.ChainSine (s1, dur, 2f, 2f);
		m.ChainSine (s2, dur, 2f, 2f);
		m.ChainSine (s1, dur, 2f, 2f);
		m.ChainCounterClockwiseCircle (c3, Mathf.Deg2Rad * 360f, dur*4);
		m.ChainCounterClockwiseCircle (c4, Mathf.Deg2Rad * 360f, dur * 4);
		m.ChainCounterClockwiseCircle (s2, Mathf.Deg2Rad * 360f, dur * 3);
		m.ChainCounterClockwiseCircle (s3, Mathf.Deg2Rad * 360f, dur * 3);
		m.ChainCounterClockwiseCircle (c5, Mathf.Deg2Rad * 360f, dur * 3);
		m.ChainCounterClockwiseCircle (c6, Mathf.Deg2Rad * 360f, dur * 3);
		m.ChainCurve (new Vector2 (0, -20), dur*3, new Vector2 (-10, -10));
		m.ChainCurve (new Vector2 (0, 0), dur*3, new Vector2 (10, -10));




		m.setMarker (marker);
		m.SetRepeat ();
		m.ToggleTrail ();

		//m.PostMovement ("http://localhost/", "UnnecessarilyComplex");

		m.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		m.Update ();
	}
}
