using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quad : MonoBehaviour {

	Movement movement;

	// Use this for initialization
	void Start () {
		Dictionary<string, object> list = new Dictionary<string, object>();
		list.Add (MovementParamNames.pointOne, new Vector2(0,0));
		list.Add (MovementParamNames.pointTwo, new Vector2(10,0));
		list.Add (MovementParamNames.pointThree, new Vector2(20,20));
		list.Add (MovementParamNames.pointFour, new Vector2 (0,5));
		movement = new Movement(this.gameObject, Movement.MovementPath.Quad,list, true);
	}
	
	// Update is called once per frame
	void Update () {
		movement.UpdatePosition ();
	
	}
}
