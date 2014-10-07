using UnityEngine;
using System.Collections;

public class TestCirc : MonoBehaviour {

	Movement movement;
	public GameObject marker;

	// Use this for initialization
	void Start () {



		float duration = 2f;
		movement = new Movement (this.gameObject);

		Vector3 start = new Vector3 (7, 10, 0);
		Vector3 center = new Vector3 (7, 0, 0);
		Vector3 end = new Vector3 (0, -10, 0);

		movement.AddLine (center,start,duration);
		movement.Add2DCircular (start, center, 180f,duration);
		//movement.Add2DCircular (end, center, 180f,duration,true);
		movement.setMarker (marker);
		//movement.SetRepeat ();
		movement.ToggleTrail ();
		movement.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		movement.Update ();
	}
}
