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
	public List<StochasticLSystemRule> stochasticRules = new List<StochasticLSystemRule>();
	private List<LSystemRule> rules = new List<LSystemRule>();
	public int generations = 1;
	private string result = "";
	public Material mat;
	public float length = 10f;
	public float angle = 90;
	public float defaultAngle;
	public bool useLineRenderer = false;
	VectorLine vecLine; 
	List<VectorLine> lines = new List<VectorLine>();

	LineRenderer bounds;
	Vector3[] boundsVerts = new Vector3[5];

	[SerializeField]
	private LineRenderer line;

	Turtle turtle = new Turtle();

	// Use this for initializations
	void Start () {
		foreach (StochasticLSystemRule rule in stochasticRules){
			rules.Add(rule.getRandomRule());
		}

		defaultAngle = angle;
		line = gameObject.AddComponent<LineRenderer>();
		line.material = mat;
		line.SetWidth(1f, 1f);
		result = axiom;
		turtle.initialize(angle, length);
		generate(generations);
		draw();

		List<RoadSegment> deadEnds = turtle.getDeadEnds(); 
		print ("Dead end count: "+deadEnds.Count);
	}

	void Update(){


		if (Input.GetKeyDown(KeyCode.D)){
			redrawWithinBounds();

		}

		if (Input.GetKeyDown(KeyCode.J)){

			JoinSegments(triangle.ToArray(), 1f);
		}


	}
	List<Vector3> triangle = new List<Vector3>();

	void redrawWithinBounds(){
		

		// get all vertices in l-system
		List<Vector3> verts = new List<Vector3>();
		foreach (RoadSegment segment in turtle.segments){
			verts.Add(segment.start);
			verts.Add(segment.end);
		}

		Bounds tmp = BoundCheck.instance.getBounds(verts);

		// make triangle

		triangle.Add( new Vector2(tmp.min.x-tmp.min.x/2f,  tmp.max.x/2.4f));
		triangle.Add( new Vector2(tmp.max.x/2.4f,  tmp.min.y/2.2f));
		triangle.Add( new Vector2(tmp.min.y/2.2f, tmp.min.x -tmp.min.x/2.6f));
	
		List<Vector3[]> sides = new List<Vector3[]>();

		VectorLine triLine1; 
		Vector3[] side1 = new Vector3[]{triangle[0], triangle[1]};
		triLine1 = new VectorLine("tri line", side1, mat, 15.0f, LineType.Discrete);
		triLine1.Draw();

		VectorLine triLine2; 
		Vector3[] side2 = new Vector3[]{triangle[1], triangle[2]};
		triLine2 = new VectorLine("tri line", side2, mat, 15.0f, LineType.Discrete);
		triLine2.Draw();

		VectorLine triLine3; 
		Vector3[] side3 = new Vector3[]{triangle[2], triangle[0]};
		triLine3 = new VectorLine("tri line", new Vector3[]{triangle[2], triangle[0]}, mat, 15.0f, LineType.Discrete);
		triLine3.Draw();

		sides.Add(side1);
		sides.Add(side2);
		sides.Add(side3);



		// Generate bounds (right now limited to square) from vertices
		Bounds b = BoundCheck.instance.getBounds(triangle);


		float newX = b.extents.x;
		float newY = b.extents.y;


		b.extents = new Vector2(newX, newY);

		boundsVerts[0] = new Vector2(b.min.x, b.min.y);
		boundsVerts[1] = new Vector2(b.max.x, b.min.y);
		boundsVerts[2] = new Vector2(b.max.x, b.max.y);
		boundsVerts[3] = new Vector2(b.min.x, b.max.y);
		boundsVerts[4] = new Vector2(b.min.x, b.min.y);

		print(b.extents);

		// Draw boundaries
		VectorLine boundaryLine; 
		boundaryLine = new VectorLine("Bound segment", boundsVerts, mat, 15.0f, LineType.Continuous);
		boundaryLine.Draw();


		// find and remove all segments outside boundaries
		List<RoadSegment> segmentsToRemove = new List<RoadSegment>();
		foreach(RoadSegment segment in turtle.segments){

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
			segLine = new VectorLine("Road segment", (Vector3[])segment.getLine(), mat, 5.0f, LineType.Discrete);
			segLine.Draw();
			lines.Add(segLine);
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
			print(result);
		}
	}

	void draw(){

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

		if (useLineRenderer){
		line.SetColors(Color.black, Color.blue);
		line.SetVertexCount(turtle.vertices.Count);
		line.SetPositions(turtle.vertices.ToArray());


			// FOR TESTING PURPUSES:

			GameObject go = new GameObject("Bounds");
			LineRenderer boundaryLine = go.AddComponent<LineRenderer>();
			boundaryLine.material = mat;
			Bounds bounds = line.bounds;
			float extension = 5f;
			boundsVerts[0] = new Vector2(bounds.min.x-extension, bounds.min.y-extension);
			boundsVerts[1] = new Vector2(bounds.max.x+extension, bounds.min.y-extension);
			boundsVerts[2] = new Vector2(bounds.max.x+extension, bounds.max.y+extension);
			boundsVerts[3] = new Vector2(bounds.min.x-extension, bounds.max.y+extension);
			boundsVerts[4] = new Vector2(bounds.min.x-extension, bounds.min.y-extension);
			boundaryLine.SetVertexCount(5);
			boundaryLine.SetWidth(1f,1f);

			boundaryLine.SetPositions(boundsVerts);
			
		}
		else {

			foreach(RoadSegment segment in turtle.segments){
				VectorLine segmentLine; 
				segmentLine = new VectorLine("Road segment", (Vector3[])segment.getLine(), mat, 5.0f, LineType.Discrete);
				segmentLine.Draw();
				lines.Add(segmentLine);
			}
		}
	}



	public void JoinSegments (Vector3[] boundVertices, float testLength, bool testBothDirections = true) {

		Debug.Assert( boundVertices.Length > 1, "Vertex count in test bounds are invalid");

		for (int i = 0; i < boundVertices.Length-1; i++){
			Vector2? intersection;

			// Equation to point on line: http://math.stackexchange.com/a/175906

			foreach (RoadSegment segment in turtle.segments){
				Vector2 unit = segment.end - segment.start;
				unit.Normalize();


				Vector2[] testPositions = new Vector2[]{segment.start + (unit * testLength * length), segment.start - (unit * testLength * length)};

				foreach (Vector2 testPos in testPositions){

					if ((intersection = LineLineIntersection.instance.LineIntersection(segment.start, testPos, boundVertices[i], boundVertices[i+1])) != null) {
						Debug.Log("Intersection point: "+intersection);


						// backwards distance
						float dist = Mathf.Sqrt(Mathf.Pow((segment.start.x - ((Vector2)intersection).x),2) + Mathf.Pow((segment.start.y - ((Vector2)intersection).y),2));

						if (Mathf.Sqrt(Mathf.Pow((segment.end.x - ((Vector2)intersection).x),2) + Mathf.Pow((segment.end.y - ((Vector2)intersection).y),2)) < dist){

							segment.end = (Vector2)intersection;

						}
						else {
							segment.start = (Vector2)intersection;


						}




					/*	GameObject go = new GameObject("Road join");
						LineRenderer join = go.AddComponent<LineRenderer>();
						join.material = mat;
						Vector3[] joined = new Vector3[2];

						if (counter == 0) // if we are checking backwards, join from start position instead of end position
							joined[0] = segment.start;
						else 
							joined[0] = segment.end;
				
						joined[1] = (Vector3)intersection;
						join.SetPositions(joined);*/
						break;
					}
				}
			}
		}
		redraw();
		//Vector3[] boundsVerts = new Vector3[5];




	}

}

