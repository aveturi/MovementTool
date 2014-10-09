using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Movement {

	private enum Type {Line, Curve, Circle, Wait, Sin};

	GameObject entity;

	GameObject markerPrefab;

	Dictionary<string, object> list;

	List<MovementPrimitive> movementPrimitivesList;
	int currentMovementidx;


	bool periodic;


	bool running = false;
	int repetitions = 0;
	bool trail = false;

	/// <summary>
	/// Initializes a new instance of the <see cref="Movement"/> class.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public Movement(GameObject entity){
		this.entity = entity;
		this.movementPrimitivesList = new List<MovementPrimitive> ();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Movement"/> class
	/// from a data string.
	/// </summary>
	/// <param name="entity">Entity.</param>
	/// <param name="data">Data.</param>
	public Movement(GameObject entity, string data){
		this.entity = entity;
		this.movementPrimitivesList = new List<MovementPrimitive> ();
		var dataLines = data.Split ('\n');

		for(int i=1; i<dataLines.Length; ++i ){

			var line = dataLines[i];
			if(line.Length == 0){
			} else if(line.Contains("PERIODIC")){
				var sp = line.Split('|');
				this.periodic = (sp[1] == "True");
			} else if(line.Contains("REPETITIONS")){
				var sp = line.Split('|');
				int.TryParse(sp[1],out repetitions);
			} else {
				this.movementPrimitivesList.Add(new MovementPrimitive(line));
			}
		}
	}

	/// <summary>
	/// Gets the <see cref="Movement"/> at the specified index.
	/// </summary>
	/// <param name="i">The index.</param>
	public object this[int i]
	{
		get { return this.GetPrimitiveAsString(i); }
	}

	/// <summary>
	/// Sets the delta tuning value for the primitive at the specified index.
	/// </summary>
	/// <param name="idx">Index.</param>
	/// <param name="delta">Delta.</param>
	public void SetPrimitiveDelta(int idx, float delta){
		movementPrimitivesList [idx].timeDelta = delta;
	}

	/// <summary>
	/// Inits the movement from file. The file must have been created by this class.
	/// </summary>
	/// <returns>The movement from file.</returns>
	/// <param name="entity">Entity.</param>
	/// <param name="filename">Filename.</param>
	public static Movement InitMovementFromFile(GameObject entity, string filename){
		var data = File.ReadAllText (filename);
		return new Movement (entity, data);
	}

	/// <summary>
	/// GETs the movement state from a URL. The state must have been stored by this class.
	/// </summary>
	/// <returns>The movement from URL.</returns>
	/// <param name="entity">Entity.</param>
	/// <param name="url">URL.</param>
	public static Movement InitMovementFromUrl(GameObject entity, string url){
		WWW www = new WWW (url);

		while (!www.isDone) {
		//TODO:There MUST be a better way than this.
			var err = www.error;
			if(err != null){
				Debug.Log("GET movement failed! "+err);
				return null;
			}
		} 

		if (www.isDone) {
			var data = Encoding.UTF8.GetString (www.bytes, 0, www.bytes.Length);
			return new Movement (entity, data);
		} else {
			throw new UnityException("GET on url failed!");
		}
	}

	/// <summary>
	/// Saves the movement to file.
	/// </summary>
	/// <param name="filename">Filename.</param>
	public void SaveMovementToFile(string filename){
		var file = File.CreateText (filename);
		file.Write (StringifyMovement ());
		file.Flush ();
		file.Close ();
	}

	/// <summary>
	/// POSTs the movement to the specified URL.
	/// </summary>
	/// <param name="url">URL.</param>
	/// <param name="movementName">Movement name.</param>
	public void PostMovement(string url, string movementName){
		WWWForm f = new WWWForm ();
		f.AddField ("name", movementName);
		f.AddField ("movement", StringifyMovement());
		WWW x = new WWW (url,f);
	}

	/// <summary>
	/// Gets the primitive state.
	/// </summary>
	/// <returns>The primitive as string.</returns>
	/// <param name="index">Index.</param>
	public string GetPrimitiveAsString(int index){
		var prim = movementPrimitivesList [index];
		if (prim.path == Type.Line) {
				return prim.path +
						" start " + prim.startPoint + 
						" end " + prim.endPoint + 
						" duration " + prim.duration;
		} else if (prim.path == Type.Curve) {
				return prim.path + 
						" start " + prim.startPoint + 
						" end " + prim.endPoint + 
						" depth " + prim.curveDepth + 
						" duration " + prim.duration;
		} else if (prim.path == Type.Circle) {
				return prim.path + 
						" start " + prim.startPoint + 
						" end " + prim.endPoint + 
						" rotAngle " + prim.rotationAngle + 
						" ccw " + prim.ccw + 
						" radius " + prim.radius + 
						" duration " + prim.duration;
		} else if (prim.path == Type.Wait) {
				return prim.path + 
						" duration " + prim.duration;
		} else if (prim.path == Type.Sin) {
				return prim.path + 
						" start " + prim.startPoint + 
						" end " + prim.endPoint + 
						" ampl " + prim.amplitude + 
						" freq " + prim.frequency +
						" duration " + prim.duration + 
						" phase " + prim.phase;
		} else {
			return "InternalError";
		}
	}

	/// <summary>
	/// Chains a LINE primitive to the current movement
	/// </summary>
	/// <param name="end">End.</param>
	/// <param name="dur">Dur.</param>
	public void ChainLine(Vector3 end, float dur){
		if (movementPrimitivesList.Count == 0) {
			throw new UnityException ("Can't chain a motion event to an empty event set!There should be at least one movement first");
		}
		this.AddLine (movementPrimitivesList [movementPrimitivesList.Count - 1].endPoint, end, dur);
	}

	/// <summary>
	/// Chains a WAIT primitive to the current movement
	/// </summary>
	/// <param name="dur">Dur.</param>
	public void ChainWait(float dur){
		this.AddWait (dur, movementPrimitivesList [movementPrimitivesList.Count - 1].endPoint);
	}

	/// <summary>
	/// Chains a SIN primitive to the current movement
	/// </summary>
	/// <param name="end">End.</param>
	/// <param name="dur">Dur.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="freq">Freq.</param>
	/// <param name="phase">Phase.</param>
	public void ChainSin(Vector3 end, float dur, float amplitude, float freq, float phase=0){
		if (movementPrimitivesList.Count == 0) {
			throw new UnityException("Can't chain a motion event to an empty event set!There should be at least one movement first");
		}
		this.AddSin (movementPrimitivesList [movementPrimitivesList.Count - 1].endPoint , end, dur, amplitude, freq, phase);
	}

	/// <summary>
	/// Chains a CURVE primitive to the current movement
	/// </summary>
	/// <param name="end">End.</param>
	/// <param name="dur">Dur.</param>
	/// <param name="dep">Dep.</param>
	public void ChainCurve(Vector3 end, float dur,Vector3 dep = default(Vector3)){
		if (movementPrimitivesList.Count == 0) {
			throw new UnityException("Can't chain a motion event to an empty event set!There should be at least one movement first");
		}
		this.AddCurve (movementPrimitivesList [movementPrimitivesList.Count - 1].endPoint, end, dur, dep);
	}

	/// <summary>
	/// Chains a counter clock wise CIRCLE primitive to the current movement
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="radians">Radians.</param>
	/// <param name="duration">Duration.</param>
	public void ChainCounterClockwiseCircle(Vector3 center, float radians, float duration){
		if (movementPrimitivesList.Count == 0) {
			throw new UnityException("Can't chain a motion event to an empty event set!There should be at least one movement first");
		}
		this.AddCounterClockwiseCircle (movementPrimitivesList [movementPrimitivesList.Count - 1].endPoint, center, radians, duration);
	}

	/// <summary>
	/// Chains a clock wise CIRCLE primitive to the current movement
	/// </summary>
	/// <param name="center">Center.</param>
	/// <param name="radians">Radians.</param>
	/// <param name="duration">Duration.</param>
	public void ChainClockwiseCircle(Vector3 center, float radians, float duration){
		if (movementPrimitivesList.Count == 0) {
			throw new UnityException("Can't chain a motion event to an empty event set!There should be at least one movement first");
		}
		this.AddClockwiseCircle (movementPrimitivesList [movementPrimitivesList.Count - 1].endPoint, center, radians, duration);
	}

	/// <summary>
	/// Adds a LINE primitive to the current movement.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	/// <param name="dur">Dur.</param>
	public void AddLine(Vector3 start, Vector3 end, float dur){
		this.movementPrimitivesList.Add (new MovementPrimitive (Movement.Type.Line,start,end,dur));
	}

	/// <summary>
	/// Adds a SIN primitive to the current movement.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	/// <param name="dur">Dur.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="freq">Freq.</param>
	/// <param name="phase">Phase.</param>
	public void AddSin(Vector3 start, Vector3 end, float dur, float amplitude, float freq, float phase=0){
		this.movementPrimitivesList.Add (new MovementPrimitive (Movement.Type.Sin, start, end, dur));
		movementPrimitivesList [movementPrimitivesList.Count - 1].amplitude = amplitude;
		movementPrimitivesList [movementPrimitivesList.Count - 1].frequency = freq;
		movementPrimitivesList [movementPrimitivesList.Count - 1].phase = phase;
	}

	/// <summary>
	/// Adds a CURVE primitive to the current movement.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	/// <param name="dur">Dur.</param>
	/// <param name="dep">Dep.</param>
	public void AddCurve(Vector3 start, Vector3 end, float dur,Vector3 dep = default(Vector3)){
		this.movementPrimitivesList.Add (new MovementPrimitive (Movement.Type.Curve,start,end,dur,dep));
	}

	/// <summary>
	/// Adds a WAIT primitive to the current movement.
	/// </summary>
	/// <param name="waitTime">Wait time.</param>
	/// <param name="waitPoint">Wait point.</param>
	public void AddWait(float waitTime, Vector3 waitPoint = default(Vector3)){
		this.movementPrimitivesList.Add (new MovementPrimitive(Movement.Type.Wait,waitPoint,Vector3.zero,waitTime));
	}

	/// <summary>
	/// Adds a counter clock wise CIRCLE primitive to the current movement.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="center">Center.</param>
	/// <param name="radians">Radians.</param>
	/// <param name="duration">Duration.</param>
	public void AddCounterClockwiseCircle(Vector3 start,Vector3 center, float radians, float duration){
		if (radians > Mathf.PI * 2) {
			throw new UnityException("Can't rotate more than 2*PI, use multiple circles or concat if necessary");
		}
		var radius = Vector3.Distance (start, center);

		var startAngle = CircleHelperAngle (start, center, radius, true);
		var end = CircleHelperPoint (radians+startAngle, center, radius);

		this.movementPrimitivesList.Add( new MovementPrimitive(Movement.Type.Circle,start,end, 
		                                                       duration,Vector3.zero, center,radians));
		movementPrimitivesList [movementPrimitivesList.Count - 1].ccw = true;
	}

	/// <summary>
	/// Adds a clock wise CIRCLE primitive to the current movement.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="center">Center.</param>
	/// <param name="radians">Radians.</param>
	/// <param name="duration">Duration.</param>
	public void AddClockwiseCircle(Vector3 start, Vector3 center, float radians, float duration){
		if (radians > Mathf.PI * 2) {
			throw new UnityException("Can't rotate more than 360 degrees, use multiple circles or concat if necessary");
		}

		var radius = Vector3.Distance (start, center);
		
		var startAngle = CircleHelperAngle (start, center, radius, false);
		var end = CircleHelperPoint (radians+startAngle, center, radius);
		
		this.movementPrimitivesList.Add( new MovementPrimitive(Movement.Type.Circle,start,end, 
		                                                       duration,Vector3.zero, center,radians));
		movementPrimitivesList [movementPrimitivesList.Count - 1].ccw = false;
	}


	/// <summary>
	/// Start this instance. Begins the movement timers. Movement.Update() needs to be called for motion to actually occur.
	/// </summary>
	public void Start(){
		movementPrimitivesList = AdjustTimes(movementPrimitivesList) as List<MovementPrimitive>;
		currentMovementidx = 0;
		entity.transform.position = movementPrimitivesList [0].startPoint;
		this.running = true;
	}

	/// <summary>
	/// Sets the number of repetitions of this movement. If left empty, the movement repeats infinitely.
	/// </summary>
	/// <param name="num">Number.</param>
	public void SetRepeat(int num = 0){
		if (num != 0) {
			this.repetitions = num;
		} else {
			this.periodic = true;
		}
	}

	/// <summary>
	/// Toggles the trail left behind by the gameObject that the movement is attached to.
	/// </summary>
	public void ToggleTrail(){
		trail = !trail;
	}

	/// <summary>
	/// Sets the marker that will be left behind if trail is turned on.
	/// </summary>
	/// <param name="m">M.</param>
	public void setMarker(GameObject m){
		this.markerPrefab = m;
	}

	/// <summary>
	/// Update this instance movement. Must be called from Monobehaviour::Update()
	/// </summary>
	public void Update(){
		if (!running)
						return;

		MovementPrimitive current = movementPrimitivesList [currentMovementidx];

		if (Time.time >= (current.startTime + current.duration)) { // time to go to next movement primitive
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

		if (trail) {
			//instantiate a marker from prefab and put it here
			GameObject x = GameObject.Instantiate(markerPrefab) as GameObject;
			x.transform.position = entity.transform.position;
		}

		RunPrimitive (current);
	}

	private void resetState(){
		//Debug.Log ("State Reset " + Time.time);
		running = false;
		currentMovementidx =  0;
		movementPrimitivesList = AdjustTimes(movementPrimitivesList);
		entity.transform.position = movementPrimitivesList [0].startPoint;
		running = true;
	}


	private void RunPrimitive(MovementPrimitive current){

		UpdateDeltaDuration (current);

		if (current.path == Type.Line) {
				MoveAlongLineSegment (current.startPoint, current.endPoint, current.startTime, current.duration + current.timeDeltaSum);
		} else if (current.path == Type.Curve) {
				MoveAlongCurve (current.startPoint, current.endPoint, current.curveDepth, current.startTime, current.duration + current.timeDeltaSum);
		} else if (current.path == Type.Circle) {
				MoveAlongCircle (current.startPoint, current.endPoint, current.startTime, current.duration + current.timeDeltaSum, 
       current.circleCenter, current.rotationAngle, current.radius, current.ccw);
		} else if (current.path == Type.Sin) {
				MoveAlongSineWave (current.startPoint, current.endPoint, current.startTime, current.duration + current.timeDeltaSum,
	                  current.amplitude, current.frequency, current.phase);
		} else if (current.path == Type.Wait) {
			entity.transform.position = current.startPoint;
		}
	}

	private void UpdateDeltaDuration(MovementPrimitive m){
		m.timeDeltaSum += m.timeDelta;
	}


	private List<MovementPrimitive> AdjustTimes(List<MovementPrimitive> list){
		list[0].startTime = Time.time;
		list [0].timeDeltaSum = 0;
		for(int i=1; i< list.Count; i++){
			list[i].startTime = list[i-1].startTime + list[i-1].duration;
			list[i].timeDeltaSum = 0;
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

	void MoveAlongSineWave(Vector3 start, Vector3 end, float timeStart,float dur, float amplitude, float freq, float phase){

		//update the linear position first
		MoveAlongLineSegment (start, end, timeStart,dur);

		//change the y coord
		var y = amplitude * Mathf.Sin (2 * Mathf.PI * freq * (Time.time - timeStart) + phase);
		var pos = entity.transform.position;
		pos.y += y;
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

	void MoveAlongCircle(Vector3 start, Vector3 end,float timeStart, float duration,
	                     Vector3 center, float angleOfRotation, float radius, bool ccw){

		if (ccw) {
			var startAngle = CircleHelperAngle (start, center, radius, ccw);
			Vector3 pos = entity.transform.position;
			var speed = angleOfRotation / duration;
			var arg = (Time.time - timeStart) * speed + startAngle;
			pos.x = center.x+Mathf.Cos (arg) * radius;
			pos.y = center.y+Mathf.Sin(arg) * radius;
			entity.transform.position = pos;

		} else {
			var startAngle = CircleHelperAngle (start, center, radius, ccw);
			Vector3 pos = entity.transform.position;
			var speed = angleOfRotation / duration;
			pos.x = center.x+Mathf.Cos ( (2*Mathf.PI) - ( (Time.time - timeStart) * speed + startAngle)) * radius;
			pos.y = center.y+Mathf.Sin((2*Mathf.PI) - ((Time.time - timeStart) * speed + startAngle)) * radius;
			entity.transform.position = pos;
		}

	}

	//This can be tweaked in many ways depending on how you want to define your circular movement.
	// for now it just assumes the stupid way.
	float CircleHelperAngle(Vector3 point,Vector3 center, float radius, bool ccw){

		if (point == center)
			return 0;

		var acos = Mathf.Acos((point.x - center.x) / radius);
		var asin = Mathf.Asin ((point.y - center.y) / radius);

		if (ccw) {
			float angle;
			if (asin > 0 || double.IsNaN(asin)) { //top-half
				angle = acos;
			} else { //bottom-half
				angle = Mathf.PI * 2 - acos;
			}
			return angle%(2*Mathf.PI);
		} else {
			float angle;
			if (asin > 0) { //bottom-half
				angle = acos;
			} else { //top-half
				angle = Mathf.PI * 2 - acos;
			}
			return angle%(2*Mathf.PI);
		}
	}


	Vector3 CircleHelperPoint(float angle, Vector3 center, float radius){
		Vector3 p = new Vector3 (0, 0, 0);
		p.x = center.x + radius * Mathf.Cos (angle);
		p.y = center.y + radius * Mathf.Sin (angle);
		return p;
	}

	
	private string StringifyMovement(){
		string buffer = "";
		buffer += (MovementPrimitive.FirstLine ())+'\n';
		foreach (var item in movementPrimitivesList) {
			buffer+=(item.GetFullState() +'\n');
		}
		
		buffer += ("PERIODIC|" + periodic+'\n');
		buffer += ("REPETITIONS|" + repetitions+'\n');
		return buffer;
	}

	// THESE FUNCTIONS ARE FROM GIBSONS BOOK------------------------
	static private Vector3 Lerp(Vector3 vFrom, Vector3 vTo, float u){
		Vector3 res = (1 - u) * vFrom + u * vTo;
		return res;
	}

	static private Vector3 Bezier(float u, List<Vector3> vList){
		if (vList.Count == 1) {
			return( vList[0]);		
		}

		List<Vector3> vListR = vList.GetRange (1, vList.Count - 1);

		List<Vector3> vListL = vList.GetRange (0, vList.Count - 1);

		Vector3 res = Lerp (Bezier (u, vListL), Bezier (u, vListR), u);

		return res;
	}

	static private Vector3 Bezier(float u, params Vector3[] vecs){
		return (Bezier (u, new List<Vector3> (vecs)));
	}
	// ----------------------------------------------------------------

	private class MovementPrimitive{

		public Movement.Type path;
		public Vector3 startPoint;
		public Vector3 endPoint;
		public float duration;
		public float startTime;
		public Vector3 curveDepth;
		public Vector3 circleCenter;
		public float rotationAngle;
		public float radius;
		public bool ccw;
		public float amplitude;
		public float frequency;
		public float phase;
		public float timeDelta;
		public float timeDeltaSum;

		public MovementPrimitive (){}
		
		public MovementPrimitive(Movement.Type path, Vector3 start, Vector3 end, float dur,Vector3 dep = default(Vector3), Vector3 center = default(Vector3),float rotationAngle = 0){
			this.path = path;
			startPoint = start;
			endPoint = end;
			duration = dur;
			this.circleCenter = center;
			curveDepth = dep;
			this.rotationAngle = rotationAngle;
			this.startTime = 0;
			this.radius = Vector3.Distance (circleCenter, startPoint);
			this.timeDelta = 0;
			this.timeDeltaSum = 0;
		}

		private Vector3 StringToVector(string str){
			
			var split = str.TrimStart ('(').TrimEnd (')').Split (',');
			
			float x, y, z=0;
			float.TryParse (split [0], out x);
			float.TryParse (split [1], out y);
			if (split.Length == 3) {
				float.TryParse (split [2], out z);
			}
			return new Vector3(x,y,z);
		}
		public MovementPrimitive(string stateString){
			var split = stateString.Split('|');
			if(split[0] == "Line"){
				path = Type.Line;
			} else if(split[0] == "Circle"){
				path = Type.Circle;
			} else if (split[0] == "Sin"){
				path = Type.Sin;
			} else if (split[0] == "Curve"){
				path = Type.Curve;
			} else if (split[0] == "Wait"){
				path = Type.Wait;
			}

			startPoint = StringToVector(split[1]);
			endPoint = StringToVector(split[2]);
			float.TryParse(split[3],out duration);
			float.TryParse(split[4],out startTime);
			curveDepth = StringToVector(split[5]);
			circleCenter = StringToVector(split[6]);
			float.TryParse(split[7],out rotationAngle);
			float.TryParse(split[8],out radius);
			this.ccw = (split[9] == "True");
			float.TryParse(split[10],out amplitude);
			float.TryParse(split[11],out frequency);
			float.TryParse(split[12],out phase);
			float.TryParse(split[13],out timeDelta);
			float.TryParse(split[14],out timeDeltaSum);
		}

		public string GetFullState(){
			return path 
					+ "|" + startPoint 
					+ "|" + endPoint 
					+ "|" + duration 
					+ "|" + startTime 
					+ "|" + curveDepth 
					+ "|" + circleCenter 
					+ "|" + rotationAngle 
					+ "|" + radius 
					+ "|" + ccw 
					+ "|" + amplitude 
					+ "|" + frequency 
					+ "|" + phase 
					+ "|" + timeDelta 
					+ "|" + timeDeltaSum;
		}

		public static string FirstLine(){
			return "Type|startPoint|endPoint|duration|startTime|curveDepth|circleCenter|rotationAngle|radius|ccw|amplitude|frequency|phase|timeDelta|timeDeltaSum";
		}
	}                         
}
