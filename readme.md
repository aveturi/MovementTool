Movement Composition Library

This tool allows you to "compose" complex 2d motion by chaining simple
2D movements together in configurable ways!

All your interaction will be with the Movement class.
There are 5 types of simple movements, I will call these "primitives" :

{Line, Curve, Circle, Wait, Sin}

Complex movements are formed by chaining primitives.
For all the methods listed below, there are more detailed usage notes near the function itself in the code.

There are also a bunch of examples under the Assets/Examples folder.

NOTE: "Teleportation" can be achieved by setting a waitPoint in the WAIT primitive.

-------------- CONSTRUCTORS

public Movement(GameObject entity);

public Movement(GameObject entity, string data);

public static Movement InitMovementFromFile(GameObject entity, string filename);

public static Movement InitMovementFromUrl(GameObject entity, string url);

-------------- ADD METHODS
/// These methods allow you to add the aforementioned primitives to the current movement 
/// set. Primitives will be executed in the order in which they are added.

public void AddLine(Vector3 start, Vector3 end, float dur);

public void AddSin(Vector3 start, Vector3 end, float dur, float amplitude, float freq, float phase=0);

public void AddCurve(Vector3 start, Vector3 end, float dur,Vector3 dep = default(Vector3));

public void AddWait(float waitTime, Vector3 waitPoint = default(Vector3));

public void AddCounterClockwiseCircle(Vector3 start,Vector3 center, float radians, float duration);

public void AddClockwiseCircle(Vector3 start, Vector3 center, float radians, float duration);


-------------- CHAIN METHODS
/// The Chain methods are similar to the  Add* methods, but they start off
/// where the previous primitive ends. Running Chain* on an empty primitive
/// set will cause an error.

public void ChainLine(Vector3 end, float dur);

public void ChainWait(float dur);

public void ChainSin(Vector3 end, float dur, float amplitude, float freq, float phase=0);

public void ChainCurve(Vector3 end, float dur,Vector3 dep = default(Vector3));

public void ChainCounterClockwiseCircle(Vector3 center, float radians, float duration);

public void ChainClockwiseCircle(Vector3 center, float radians, float duration);


--------------

// this allows you to shift the entire movement by the point shift
public void ShiftMovementByPoint(Vector3 shift);

public void Start();

public void SetRepeat(int num = 0);

public void ToggleTrail();

// this is the gameObject that will be instantiated on the screen as a trail marker.
// It is prudent to give that gameobject some sort of timer so that it destroys
// itself after a while. For testing purposes only. Once you've constructed your
// movement, you should not ideally be using this trail.
public void setMarker(GameObject m);

public void Update();


-------------- AUXILIARY METHODS

// returns the state of the primitive at the specified index
public object this[int i];

public void SaveMovementToFile(string filename);

public void PostMovement(string url, string movementName);

public string GetPrimitiveAsString(int index);


-------------- OTHER WEIRD TUNING AVAILABLE

// This is an experimental tuning feature that allows the user to add a
// damping effect to the motion of the object by tuning the delta value.
// However, slightly higher values make the frame rate look very choppy
// and slightly lower than necessary values make the damping unnoticeable.
// Furthermore, if the values aren't "just right" (and this is found by trying
// a bunch of small values), then the movement begins to fail in all sorts of
// funny ways. 

// IF you can do a LOT of fiddling with deltas , then you can achieve
// a decent looking damping effect on your motion.

public void SetPrimitiveDelta(int idx, float delta);
