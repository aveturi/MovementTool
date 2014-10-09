using UnityEngine;
using System.Collections;

public class TestChain : MonoBehaviour {

	Movement movement;
	public GameObject marker;

	// Use this for initialization
	void Start () {
		/*movement = new Movement (this.gameObject);

		float dur = 2f;
		Vector2 p0 = new Vector2 (0, 0);
		Vector2 p1 = new Vector2 (10, 10);
		Vector2 d1 = new Vector2 (10, 0);
		movement.AddLine(p0,p1,dur);
		movement.ChainLine (p0, dur);
		movement.ChainSin (p1, dur, 2, 2);
		movement.ChainCurve (p0, dur, d1);
		movement.ChainCounterClockwiseCircle (d1, Mathf.Deg2Rad * 90, dur);

		//movement.setMarker (marker);
		//movement.ToggleTrail ();
		movement.SetRepeat ();
		movement.SaveMovementToFile (Application.dataPath+"/Movements/hello");

*/

		movement = Movement.InitMovementFromUrl (this.gameObject, "http://localhost/?name=theAwesomeMovement");
		movement.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		movement.Update ();
	}

	void func(){}


}
