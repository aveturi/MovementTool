using UnityEngine;
using System.Collections;

public class TestDownload : MonoBehaviour {

	public GameObject marker;
	Movement movement;
	// Use this for initialization
	void Start () {
		movement = Movement.InitMovementFromUrl(this.gameObject,"http://localhost/?name=FigOfEightish");

		movement.ShiftMovementByPoint (new Vector2 (-10, -10));
		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.Start ();

	}
	
	// Update is called once per frame
	void Update () {
		movement.Update ();
	}
}
