using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement {

	public enum MovementPath {Circular, Quad, ZigZag, Line};


	GameObject entity;
	MovementPath path;
	Dictionary<string, object> list;

	List<MovementPrimitive> movementPrimitivesList;
	int currentMovementidx;

	float timeDuration = 3;
	float quadTimeStart;

	bool periodic;

	public Movement(GameObject entity, MovementPath path, Dictionary<string,object> list, bool periodic = false){
		this.entity = entity;
		this.path = path;
		this.list = list;
		this.periodic = periodic;
		if (path == MovementPath.Quad) {
			quadTimeStart = Time.time;
		}
	}

	public Movement(GameObject entity, List<MovementPrimitive> listArg, bool periodic = false){
		// the list is a list of primitives, which are two points and a specified motion between the two points
		// along with a duration. This function should execute each of those primitives in order
		this.periodic = periodic;
		this.entity = entity;
		movementPrimitivesList = AdjustTimes(listArg) as List<MovementPrimitive>;
		currentMovementidx = 0;
	}

	public void Update(){
		MovementPrimitive current = movementPrimitivesList [currentMovementidx];
		if (Time.time > (current.startTime + current.duration)) { // time to go to next movement primitive

			if(currentMovementidx == movementPrimitivesList.Count-1){

				if(periodic){
				currentMovementidx =  0;
				movementPrimitivesList = AdjustTimes(movementPrimitivesList);

				}
			} else {
				currentMovementidx++;
			}
			current = movementPrimitivesList[currentMovementidx];

		}
		RunPrimitive (current);
	}

	private void RunPrimitive(MovementPrimitive current){
		if (current.path == MovementPath.Line) {
			MoveAlongLineSegment(current.startPoint,current.endPoint,current.startTime,current.duration);
		}
	}

	public void UpdatePosition(){
		if (path == MovementPath.Circular) {
			Vector2 center = (Vector2)list [MovementParamNames.center];
			float radius = (float)list [MovementParamNames.radius];
			float radialSpeed = (float)list [MovementParamNames.radialSpeed];
			Circular (center, radius, radialSpeed);
		} else if (path == MovementPath.Quad) {
			List<Vector3> positions = new List<Vector3>();
			positions.Add((Vector2)list [MovementParamNames.pointOne]);
			positions.Add((Vector2)list [MovementParamNames.pointTwo]);
			positions.Add((Vector2)list [MovementParamNames.pointThree]);
			positions.Add((Vector2)list [MovementParamNames.pointFour]);
			Quad(positions, ref quadTimeStart, timeDuration, periodic);
		}
	}

	private List<MovementPrimitive> AdjustTimes(List<MovementPrimitive> list){
		list[0].startTime = Time.time;
		for(int i=1; i< list.Count; i++){
			list[i].startTime = list[i-1].startTime + list[i-1].duration;
		}

		return list;
	}


	void Circular(Vector2 center, float radius, float radialSpeed){
		Vector2 pos = entity.transform.position;
		pos.x = center.x+Mathf.Cos (Time.time * radialSpeed) * radius;
		pos.y = center.y+Mathf.Sin(Time.time * radialSpeed) * radius;
		entity.transform.position = pos;
	}

	void Quad(List<Vector3> positions, ref float timeStart, float duration, bool periodic = false){

		float timePerSegment = duration / 4;
		float now = Time.time;

		float timeEndOne = timeStart + timePerSegment;
		float timeEndTwo = timeStart + 2 * timePerSegment;
		float timeEndThree = timeStart + 3 * timePerSegment;
		float timeEndFour = timeStart + 4 * timePerSegment;

		if (now < timeEndOne) {
			MoveAlongLineSegment (positions [0], positions [1], timeStart, timePerSegment);
		} else if (now > timeEndOne && now < timeEndTwo) {
			MoveAlongLineSegment (positions [1], positions [2], timeEndOne, timePerSegment);
		} else if (now > timeEndTwo && now < timeEndThree) {
			MoveAlongLineSegment (positions [2], positions [3], timeEndTwo, timePerSegment);
		} else if (now > timeEndThree && now < timeEndFour) {
			MoveAlongLineSegment (positions [3], positions [0], timeEndThree, timePerSegment);
		} else if (now > timeEndFour && periodic) {
			timeStart = Time.time;
		}
	}

	void ZigZag(float xSpeed, float baseYCoord, float oscillationSpeed, float maxYCoord){
		Vector2 pos = entity.transform.position;
		pos.x += xSpeed;
		pos.y = baseYCoord + Mathf.Sin(Time.time * oscillationSpeed) * maxYCoord;
		entity.transform.position = pos;
	}

	bool MoveAlongLineSegment(Vector2 p1,Vector2 p2, float timeStart, float duration){

		float u = (Time.time - timeStart) / duration;
		if (u <= 1) {
			entity.transform.position = Bezier (u, p1, p2);
			return true;
		} else {
			Vector2 diff = (Vector2)entity.transform.position - p2;
			if(diff.x < 0.1f && diff.y < 0.1f){
				return false;
			}
		}

		return false;
	}
	
	// THESE FUNCTIONS ARE TAKEN FROM THE TEXTBOOK-------------------
	static public Vector3 Lerp(Vector3 vFrom, Vector3 vTo, float u){
		Vector3 res = (1 - u) * vFrom + u * vTo;
		return res;
	}

	static public Vector3 Bezier(float u, List<Vector3> vList){
		if (vList.Count == 1) {
			return( vList[0]);		
		}

		List<Vector3> vListR = vList.GetRange (1, vList.Count - 1);

		List<Vector3> vListL = vList.GetRange (0, vList.Count - 1);

		Vector3 res = Lerp (Bezier (u, vListL), Bezier (u, vListR), u);

		return res;
	}

	static public Vector3 Bezier(float u, params Vector3[] vecs){
		return (Bezier (u, new List<Vector3> (vecs)));
	}
	// ----------------------------------------------------------------

	                          
}


public sealed class MovementParamNames{
	public static readonly string radius = "radius";
	public static readonly string center = "center";
	public static readonly string radialSpeed = "radialSpeed";
	public static readonly string pointOne = "pointOne";
	public static readonly string pointTwo = "pointTwo";
	public static readonly string pointThree = "pointThree";
	public static readonly string pointFour = "pointFour";
}

public class MovementPrimitive{
	public Movement.MovementPath path;
	public Vector2 startPoint;
	public Vector2 endPoint;
	public float duration;
	public float startTime;
			
	public MovementPrimitive (){}
	
	public MovementPrimitive(Movement.MovementPath p, Vector2 s, Vector2 e, float d, float st = 0){
		Debug.Log("init prim "+p+" "+s+" "+e+" "+d+" "+st);
		path = p;
		startPoint = s;
		endPoint = e;
		duration = d;
		startTime = st;
	}
}

