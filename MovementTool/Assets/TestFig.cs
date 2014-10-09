using UnityEngine;
using System.Collections;

public class TestFig : MonoBehaviour {

	public GameObject marker;
	Movement movement;



	// Use this for initialization
	void Start () {
		movement = new Movement (this.gameObject);


		movement.AddClockwiseCircle (Vector2.zero, new Vector2 (5, 0), Mathf.Deg2Rad * 360f, 3);
		movement.ChainCounterClockwiseCircle (new Vector2 (-5, 0), Mathf.Deg2Rad * 360f, 3);
		movement.ChainWait (3);
		var x = movement[0];
		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.SetRepeat (2);

		//movement.SaveMovementToFile ("asda");
		//movement.Start ();

		//movement.PostMovement ("http://localhost/", "theAwesomeMovement");

		//Movement m = Movement.InitMovementFromUrl (this.gameObject, "http://localhost");
	}
	
	// Update is called once per frame
	void Update () {

		//movement.Update ();
	}
}
