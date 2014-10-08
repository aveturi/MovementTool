using UnityEngine;
using System.Collections;

public class TestCirc : MonoBehaviour {

	Movement movement;
	public GameObject marker;
	Vector3 c = new Vector3(0,0,0);

	Vector3 start = new Vector3(0,-5,0);
	Vector3 end = new Vector3(0,10,0);

	float rotationAngle =180f*Mathf.Deg2Rad;
	float dur = 2f;
	// Use this for initialization
	void Start () {

		movement = new Movement (this.gameObject);

		movement.Add2DCircular (start, c, rotationAngle, dur);
		movement.Add2DCircular (end, c, rotationAngle, dur);


		movement.setMarker (marker);
		movement.ToggleTrail ();
		movement.Start ();
	}
	
	// Update is called once per frame
	void Update () {

		movement.Update ();
		}
}
