using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoundCheck  {

	private static BoundCheck _instance;
	private BoundCheck() {}

	public static BoundCheck instance{
		get {
			if (_instance == null){
				_instance = new BoundCheck();
			}
			return _instance;
		}
	}


	// http://stackoverflow.com/a/218081
	public bool insideBoundingBox(Vector2 p, float xMin, float xMax, float yMin, float yMax){

		if (p.x < xMin || p.x > xMax || p.y < yMin || p.y > yMax) {
			return false;
		}
		return true;
	}


	// http://stackoverflow.com/a/218081
	public bool insidePolygon(Vector2 p, List<Vector3[]> sides){


		foreach(Vector3[] side in sides){
			if (side.Length != 2){
				Debug.LogError("More than two vertices in polygon side");
				return false;
			}
		}


		Vector2 outside = new Vector2(-100000f, -100000f); // do this more sophisticated

		int counter = 0;
		for (int i = 0; i < sides.Count; i++){
			if (LineLineIntersection.instance.LineIntersection(p, outside, sides[i][0], sides[i][1]) != null){
				counter++;
			}
		}

		if (counter % 2 == 1 ){ // if odd number of intersections
			return true;
		}
		return false;
	}

	public Bounds getBounds(List<Vector3> verts){
		Bounds b = new Bounds();

		b.min = new Vector2(verts[0].x, verts[0].y);
		b.max = new Vector2(verts[0].x, verts[0].y);

		Vector2 min = verts[0];
		Vector2 max = verts[0];
		foreach(Vector2 p in verts){
			if (p.x < b.min.x)
				min.x = p.x;
			if (p.x > b.max.x)
				max.x = p.x;
			if (p.y < b.min.y)
				min.y = p.y;
			if (p.y > b.max.y)
				max.y = p.y;

			b.min = min;
			b.max = max;
		}
		return b;
	}

}
