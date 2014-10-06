using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Capsule : MonoBehaviour {

	Movement movement;

	// Use this for initialization
	void Start () {	
		List<MovementPrimitive> list = new List<MovementPrimitive>();
		//list.add(movement(type,startPos,endPos,duration))
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(0,0), new Vector2(5,5),1));
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(5,5), new Vector2(0,0), 1));
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(0,0), new Vector2(0,10), 1));
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(0,10), new Vector2(-20,-20), 1));
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(-20,-20), new Vector2(0,0), 4));
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(0,0), new Vector2(10,-10), 0.5f));
		list.Add (new MovementPrimitive (Movement.MovementPath.Line,new Vector2(10,-10), new Vector2(0,0), 6f));
		movement = new Movement (this.gameObject, list, true);
	}
	
	// Update is called once per frame
	void Update () {
		movement.Update ();
	}
}
