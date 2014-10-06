using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sphere : MonoBehaviour {

	
	Vector2 center = new Vector2 (0, 0);
	float radius = 3f;
	float radialSpeed = 5f;
	
	Movement movement;
	// Use this for initialization
	void Start () {
		// define params of movement
		Dictionary<string, object> list = new Dictionary<string, object>();
		list.Add (MovementParamNames.center, center);
		list.Add (MovementParamNames.radius, radius);
		list.Add (MovementParamNames.radialSpeed, radialSpeed);
		movement = new Movement(this.gameObject, Movement.MovementPath.Circular,list);
		
	}
	
	// Update is called once per frame
	void Update () {
		movement.UpdatePosition ();
	}
}
