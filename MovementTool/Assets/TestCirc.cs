using UnityEngine;
using System.Collections;

public class TestCirc : MonoBehaviour {

	Movement movement;
	public GameObject marker;
	Vector3 c = new Vector3(0,0,0);

	Vector3 start = new Vector3(0,-5,0);
	Vector3 end = new Vector3(0,10,0);
	Vector3 e2 = new Vector3(0,5,0);
	Vector2 s2 = new Vector2 (0, -10);

	float rotationAngle =180f*Mathf.Deg2Rad;
	float dur = 2f;
	// Use this for initialization
	void Start () {

		movement = new Movement (this.gameObject);

		/*movement.AddClockwiseCircle (start, c, rotationAngle, dur);
		movement.AddCounterclockwiseCircle (end, c, rotationAngle, dur);
		movement.AddLine (end, start, dur/4);
		movement.AddCounterclockwiseCircle (start, c, rotationAngle, dur);
		movement.AddLine (e2, s2, dur/4);
		movement.AddCounterclockwiseCircle (s2, c, rotationAngle, dur);
		movement.AddWait (dur);*/

		movement.AddCounterClockwiseCircle ( new Vector2(0,10), new Vector2(0,1), Mathf.Deg2Rad*270f, dur);

		movement.SetRepeat ();
		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.Start ();
	}
	
	// Update is called once per frame
	void Update () {

		movement.Update ();
		}
}
