using UnityEngine;
using System.Collections;

public class LineLineIntersection {

	private static LineLineIntersection _instance;
	private LineLineIntersection() {}

	public static LineLineIntersection instance{
		get {
			if (_instance == null){
				_instance = new LineLineIntersection();
			}
			return _instance;
		}
	}

	// http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
	public Vector2? LineIntersection(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2){
		Vector2 r = p2 - p;
		Vector2 s = q2 - q;

		float t = Cross((q - p), s / Cross(r,s));
		float u = Cross((q - p), r / Cross(r,s));

	//	Debug.Log("t: " + t + " u: "+u);
		if ((t >= 0 && t <= 1) && (u >= 0 && u <= 1)){
			Vector2 intersection = p + (t * r); // same as q + (u * s)
			return intersection;
		}


		if (floatEqual(Cross(r,s), 0f) && floatEqual(Cross((q - p), r), 0f)){ // they are colinear
		//	Debug.Log("they are colinear");
			// for this application, we dont want to do anything if they are colinear
			return null;
		}
		if (floatEqual(Cross(r,s), 0f) && !floatEqual(Cross((q - p), r), 0f)){ // they are parallel and not intersection
		//	Debug.Log("they are parallel and not intersection");
			return null;
		}

	//	Debug.Log("the to lines are not parallel and does not intersect");
		// if not the above, the to lines are not parallel and does not intersect
		return null;
	}


	private float Cross(Vector2 a, Vector2 b){
		return a.x * b.y - a.y * b.x;
	}

	private bool floatEqual(float a, float b, float epsilon = 0.001f) {
		float absA = Mathf.Abs(a);
		float absB = Mathf.Abs(b);
		float diff = Mathf.Abs(a - b);

		if (a == b) { // shortcut, handles infinities
			return true;

		} else if (a == 0 || b == 0 || diff <  double.Epsilon) {
			// a or b is zero or both are extremely close to it
			// relative error is less meaningful here
			return diff < epsilon;
		} else { // use relative error
			return diff / (absA + absB) < epsilon;
		}
	}
}
