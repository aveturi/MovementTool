using UnityEngine;
using System.Collections;

public class TestSin : MonoBehaviour {

	Movement movement;
	public GameObject marker;

	// Use this for initialization
	void Start () {
		movement = new Movement (this.gameObject);

		Vector2 end = new Vector2 (10, -10);
		Vector2 start = new Vector2 (-10, -10);
		movement.AddSine (start, end, 3, 2f, 0.5f , Mathf.Deg2Rad*180);
		movement.ChainCounterClockwiseCircle (Vector2.zero, 90f*Mathf.Deg2Rad, 3);
		movement.ChainSine (new Vector2 (-10,10), 3, 2f, 0.5f);
		movement.ChainCounterClockwiseCircle (Vector2.zero,90f*Mathf.Deg2Rad, 3);

		//Debug.Log (movement[0]);
		//Debug.Log (movement[1]);
		//Debug.Log (movement[2]);

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
