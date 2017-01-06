using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

[System.Serializable]
public struct LSystemRule{
	public string left;
	public string right;
}

[System.Serializable]
public struct StochasticLSystemRule{

	public LSystemRule[] ruleVariations;

	public LSystemRule getRandomRule(){

		int rand = Random.Range(0, ruleVariations.Length); 
		return ruleVariations[rand];
	}
}



public class LSystem : MonoBehaviour {

	public string axiom = "A";
	public List<float> angles = new List<float>();
	public List<StochasticLSystemRule> stochasticRules = new List<StochasticLSystemRule>();
	private List<LSystemRule> rules = new List<LSystemRule>();
	public int generations = 1;
	private string result = "";
	public Material mat;
	public float length = 10f;
	public float angle = 90;
	public float defaultAngle;
	private float lineWidth;
	private bool created = false;
	private Vector2 position = Vector2.zero;
	VectorLine vecLine; 
	List<VectorLine> lines = new List<VectorLine>();
	Vector3[] boundsVerts = new Vector3[5];

	Turtle turtle = new Turtle();

	// Use this for initializations
	void Awake () {
		foreach (StochasticLSystemRule rule in stochasticRules){
			rules.Add(rule.getRandomRule());
		}

		result = axiom;
		lineWidth = length * 2;
	}


	public void reset(){
		rules.Clear();
		foreach (StochasticLSystemRule rule in stochasticRules){
			rules.Add(rule.getRandomRule());
		}
		result = axiom;

		VectorLine.Destroy(lines);

		lines.Clear();
		turtle = new Turtle();
		created = false;
	}


	public bool fillBounds(Vector2 min, Vector2 max){

		bool left = false;
		bool right = false;
		bool up = false;
		bool down = false;

	

		foreach(RoadSegment seg in turtle.segments){

			float multiplier = 2;

			if (seg.start.x < min.x || seg.end.x < min.x ){
				left = true;
			}

			if (seg.start.x > max.x || seg.end.x > max.x){
				right = true;
			}

			if (seg.start.y < min.y|| seg.end.y < min.y){
				down = true;
			}

			if (seg.start.y > max.y|| seg.end.y > max.y){
				up = true;
			}
		
			if (left && right){
				return true;
			}
		} 


		return false;

	}

	public Vector2 getCenter(List<RoadSegment> seg){

		List<Vector3> verts = new List<Vector3>();
		foreach (RoadSegment s in seg) {
			verts.Add(s.start);
		}

		Bounds b = BoundCheck.instance.getBounds(verts);
		return b.center;

	}

	public Vector2 getCenter(){

		List<Vector3> verts = new List<Vector3>();
		foreach (RoadSegment s in turtle.segments) {
			verts.Add(s.start);
		}

		Bounds b = BoundCheck.instance.getBounds(verts);
		return b.center;

	}



	public void center(Vector2 lSystemCenter, Vector2 polygonCenter){

	
		// Generate bounds from vertices

		Vector2 diff = polygonCenter - lSystemCenter;
		print("centering");
		foreach(RoadSegment seg in turtle.segments){

			seg.start = seg.start + diff;
			seg.end = seg.end + diff;
		}

		redraw();

	}


	void Update(){
		

		if (Input.GetKeyDown(KeyCode.C) && !created){
			init(Vector2.zero);
			created = true;
		}

		if (Input.GetKeyDown(KeyCode.R)){
			reset();
			init(position);

		}

		if (Input.GetKeyDown(KeyCode.J)){
			joinSegments(triangle.ToArray(), 1f);
		}

		if (Input.GetKeyDown(KeyCode.M)){
			center(getCenter(turtle.segments), new Vector2(20f,20f));
		}


	}
	List<Vector3> triangle = new List<Vector3>();

	public void confineToBounds(List<Vector3[]> sides){
		

		// get all vertices in l-system
		List<Vector3> verts = new List<Vector3>();
		foreach (Vector3[] vertices in sides){
			Debug.Assert(vertices.Length == 2, "Number of vertices in line is not 2");
			verts.Add(vertices[1]);
		}
	

		// Generate bounds from vertices

		Bounds b = BoundCheck.instance.getBounds(verts);


		float newX = b.extents.x;
		float newY = b.extents.y;


		b.extents = new Vector2(newX, newY);

		boundsVerts[0] = new Vector2(b.min.x, b.min.y);
		boundsVerts[1] = new Vector2(b.max.x, b.min.y);
		boundsVerts[2] = new Vector2(b.max.x, b.max.y);
		boundsVerts[3] = new Vector2(b.min.x, b.max.y);
		boundsVerts[4] = new Vector2(b.min.x, b.min.y);

		// Draw boundaries

		/*
		VectorLine boundaryLine; 
		boundaryLine = new VectorLine("Bound segment", boundsVerts, mat, lineWidth * 3, LineType.Continuous);
		boundaryLine.Draw3D();
        */

		// find and remove all segments outside boundaries
		List<RoadSegment> segmentsToRemove = new List<RoadSegment>();
		/*foreach(RoadSegment segment in turtle.segments){

			if (!BoundCheck.instance.insideBoundingBox(segment.start, b.min.x, b.max.x, b.min.y, b.max.y) || 
				!BoundCheck.instance.insideBoundingBox(segment.end, b.min.x, b.max.x, b.min.y, b.max.y)){
				segmentsToRemove.Add(segment);

			}

		}
			
		foreach(RoadSegment segment in segmentsToRemove){
			turtle.removeSegment(segment);
		}

		VectorLine.Destroy(lines);
		lines.Clear();

*/


		foreach(RoadSegment segment in turtle.segments){
			if (!BoundCheck.instance.insidePolygon(segment.start, sides) || 
				!BoundCheck.instance.insidePolygon(segment.end, sides)){
				segmentsToRemove.Add(segment);
			}
		}



		foreach(RoadSegment segment in segmentsToRemove){
			turtle.removeSegment(segment);
		}

		redraw();

	}


	public void redraw(){

		VectorLine.Destroy(lines);
		lines.Clear();

		// redraw the segments inside the boundaries 
		foreach(RoadSegment segment in turtle.segments){
			VectorLine segLine; 
			segLine = new VectorLine("Road segment", (Vector3[])segment.getLine(), mat, lineWidth, LineType.Discrete);
			segLine.Draw3D();
			lines.Add(segLine);
			segLine.depth = 2;
		}
	}

	bool almostEqual(float a, float b, float epsilon = 0.001f){

		if (Mathf.Abs(a - b) < epsilon){
			return true;
		}
		return false;

	}

	void generate(int iterations){

		for (int i = 0; i < iterations; i++){
			string next = "";
			for (int j = 0; j < result.Length; j++){
				bool found = false;

				rules.Clear();
				foreach (StochasticLSystemRule rule in stochasticRules){
					rules.Add(rule.getRandomRule());
				}
				string current = result[j].ToString();
				foreach (LSystemRule rule in rules){
					if (current == rule.left){
						next += rule.right;
						found = true;
						break;
					}
				}
				if (!found){
					next += current;
				}
			}
			result = next;
			//print(result);
		}
	}

	public void init(Vector2 position){

		this.position = position;
		if (Random.Range(1,100) < 50){
			this.defaultAngle = angles[Random.Range(0,angles.Count-1)];
		}
		else {
			this.defaultAngle = angle;
		}
		turtle.initialize(defaultAngle, angle, length, position);
		generate(generations);
		draw();

	}

	void draw(){

		//print("drawing");
		for (int i = 0; i < result.Length; i++){
			string current = result[i].ToString();

			switch (current){
			case "F":
				turtle.forward();
				break;
			case "-":
				turtle.rotate(-angle);
				break;
			case "+":
				turtle.rotate(angle);
				break;
			case "[":
				turtle.push();
				break;
			case  "]":
				turtle.pop();
				break;
			default: 
				break;
			}
		}


		foreach(RoadSegment segment in turtle.segments){
			VectorLine segmentLine; 
			segmentLine = new VectorLine("Road segment", (Vector3[])segment.getLine(), mat, lineWidth, LineType.Discrete);
			segmentLine.Draw3D();
			lines.Add(segmentLine);
		
		}
	}

	public void joinSegments (Vector3[] boundVertices, float testLength, bool testBothDirections = true) {

		List<Vector2> joinPoints = new List<Vector2>();
		Debug.Assert( boundVertices.Length > 1, "Vertex count in test bounds are invalid");

		for (int i = 0; i <= boundVertices.Length-1; i++){
			Vector2? intersection;

			// Equation to point on line: http://math.stackexchange.com/a/175906

			foreach (RoadSegment segment in turtle.segments){
				Vector2 unit = segment.end - segment.start;
				unit.Normalize();


				Vector2[] testPositions = new Vector2[]{segment.start + (unit * testLength * length), segment.start - (unit * testLength * length)};

				foreach (Vector2 testPos in testPositions){


					Vector2 p0 = boundVertices[i];
					Vector2 p1;// = boundVertices[i+1];

					if (i == boundVertices.Length-1)
						p1 = boundVertices[0];
					else 
						p1 = boundVertices[i+1]; 


					if ((intersection = LineLineIntersection.instance.LineIntersection(segment.start, testPos, p0, p1)) != null) {

						if (joinPoints.Contains((Vector2)intersection)){
							continue;
						}

						joinPoints.Add((Vector2)intersection);

						// backwards distance
						float dist = Mathf.Sqrt(Mathf.Pow((segment.start.x - ((Vector2)intersection).x),2) + Mathf.Pow((segment.start.y - ((Vector2)intersection).y),2));
						if (Mathf.Sqrt(Mathf.Pow((segment.end.x - ((Vector2)intersection).x),2) + Mathf.Pow((segment.end.y - ((Vector2)intersection).y),2)) < dist){

							segment.end = (Vector2)intersection;

						}
						else {
							segment.start = (Vector2)intersection;
						}
						break;
					}
				}
			}
		}
		redraw();
	}

}

