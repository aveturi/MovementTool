using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement {

	public enum Type {Line, Curve, Circle, Wait};


	GameObject entity;

	GameObject markerPrefab;

	Type path;
	Dictionary<string, object> list;

	List<MovementPrimitive> movementPrimitivesList;
	int currentMovementidx;

	float quadTimeStart;

	bool periodic;


	bool running = false;
	int repetitions = 0;
	bool trail = false;

	public Movement(GameObject entity, List<MovementPrimitive> listArg, bool periodic = false){
		// the list is a list of primitives, which are two points and a specified motion between the two points
		// along with a duration. This function should execute each of those primitives in order
		this.periodic = periodic;
		this.entity = entity;
		movementPrimitivesList = AdjustTimes(listArg) as List<MovementPrimitive>;
		currentMovementidx = 0;
	}

	public Movement(GameObject entity){
		this.entity = entity;
		this.movementPrimitivesList = new List<MovementPrimitive> ();
	}

	public void AddPrimitive(Movement.Type path, Vector3 start, Vector3 end, float dur,Vector3 dep = default(Vector3), float st = 0){
		this.movementPrimitivesList.Add (new MovementPrimitive (path,start,end,dur,dep,st));
	}

	public void Start(){
		movementPrimitivesList = AdjustTimes(movementPrimitivesList) as List<MovementPrimitive>;
		currentMovementidx = 0;
		this.running = true;
	}

	public void SetRepeat(int num = 0){
		if (num != 0) {
			this.repetitions = num;
		} else {
			this.periodic = true;
		}
	}

	public void ToggleTrail(){
		trail = !trail;
	}

	public void setMarker(GameObject m){
		this.markerPrefab = m;
	}


	public void Update(){
		if (!running)
						return;


		if (trail) {
			//instantiate a marker from prefab and put it here
			GameObject x = GameObject.Instantiate(markerPrefab) as GameObject;
			x.transform.position = entity.transform.position;
		}

		MovementPrimitive current = movementPrimitivesList [currentMovementidx];
		if (Time.time > (current.startTime + current.duration)) { // time to go to next movement primitive

			if(currentMovementidx == movementPrimitivesList.Count-1){

				if(periodic){
					this.resetState();
				} else {

					if(this.repetitions > 1){
						repetitions--;
						this.resetState();
					} else {
						running = false;
					}
				}
			} else {
				currentMovementidx++;
			}
			current = movementPrimitivesList[currentMovementidx];

		}
		RunPrimitive (current);
	}

	private void resetState(){
		currentMovementidx =  0;
		movementPrimitivesList = AdjustTimes(movementPrimitivesList);
	}

	private void RunPrimitive(MovementPrimitive current){
		if (current.path == Type.Line) {
			MoveAlongLineSegment (current.startPoint, current.endPoint, current.startTime, current.duration);
		} else if (current.path == Type.Curve) {
			MoveAlongCurve (current.startPoint, current.endPoint, current.curveDepth, current.startTime, current.duration);
		} else if (current.path == Type.Circle) {
			MoveAlongCircle(current.circleCenter, current.circleRadius, current.startTime, current.duration, current.clockwise);
		}
	}

	private List<MovementPrimitive> AdjustTimes(List<MovementPrimitive> list){
		list[0].startTime = Time.time;
		for(int i=1; i< list.Count; i++){
			list[i].startTime = list[i-1].startTime + list[i-1].duration;
		}

		return list;
	}


	void Circular(Vector3 center, float radius, float radialSpeed){
		Vector3 pos = entity.transform.position;
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
		Vector3 pos = entity.transform.position;
		pos.x += xSpeed;
		pos.y = baseYCoord + Mathf.Sin(Time.time * oscillationSpeed) * maxYCoord;
		entity.transform.position = pos;
	}

	bool MoveAlongLineSegment(Vector3 p1,Vector3 p2, float timeStart, float duration){

		float u = (Time.time - timeStart) / duration;
		if (u <= 1) {
			entity.transform.position = Bezier (u, p1, p2);
			return true;
		} else {
			Vector3 diff = (Vector3)entity.transform.position - p2;
			if(Mathf.Abs (diff.x) < 0.1f && Mathf.Abs (diff.y) < 0.1f){
				return false;
			}
		}

		return false;
	}

	bool MoveAlongCurve(Vector3 start, Vector3 end, Vector3 depth, float timeStart, float duration){
		float u = (Time.time - timeStart) / duration;
		if (u <= 1) {
			entity.transform.position = Bezier (u, start, depth, end);
			return true;
		} else {
			return false;
		}
	}

	bool MoveAlongCircle(Vector3 center, float radius, float timeStart, float duration, bool clockwise){
		float now = Time.time;
		if (now < timeStart + duration) {
			Vector3 pos = entity.transform.position;
			float speed = (2*Mathf.PI)/duration;
			if(clockwise){
				pos.x = center.x + Mathf.Cos ( Mathf.PI*2 - now*speed) * radius;
				pos.y = center.y + Mathf.Sin ( Mathf.PI*2 - now*speed) * radius;
			} else {
				pos.x = center.x + Mathf.Cos (now*speed) * radius;
				pos.y = center.y + Mathf.Sin (now*speed) * radius;
			}
			entity.transform.position = pos;
			return true;
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
	public Movement.Type path;
	public Vector3 startPoint;
	public Vector3 endPoint;
	public float duration;
	public float startTime;
	public Vector3 curveDepth;
	public Vector3 circleCenter;
	public float   circleRadius;
	public bool clockwise;

	public MovementPrimitive (){}
	
	public MovementPrimitive(Movement.Type path, Vector3 start, Vector3 end, float dur,Vector3 dep = default(Vector3), float st = 0){
		this.path = path;
		startPoint = start;
		endPoint = end;
		duration = dur;
		startTime = st;
		curveDepth = dep;
	}

	public MovementPrimitive(Movement.Type path, Vector3 center, float rad, float dur, bool clock,float st = 0){
		this.path = path;
		this.circleCenter = center;
		circleRadius = rad;
		duration = dur;
		startTime = st;
		this.clockwise = clock;
	}
}