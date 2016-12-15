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
	private float lineWidth;
	private Vector2 position = Vector2.zero;
	VectorLine vecLine; 
	List<VectorLine> lines = new List<VectorLine>();

	LineRenderer bounds;
	Vector3[] boundsVerts = new Vector3[5];

	Turtle turtle = new Turtle();

	// Use this for initializations
	void Awake () {
		foreach (StochasticLSystemRule rule in stochasticRules){
			rules.Add(rule.getRandomRule());
		}

		//defaultAngle = angle;
		result = axiom;
		lineWidth = length;
	
		//init(Vector2.zero);
	}

	void Update(){


		if (Input.GetKeyDown(KeyCode.D)){
			//redrawWithinBounds();

		}

		if (Input.GetKeyDown(KeyCode.J)){
			JoinSegments(triangle.ToArray(), 1f);
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
			
		// make triangle
		/*
		triangle.Add( new Vector2(tmp.min.x-tmp.min.x/2f,  tmp.max.x/2.4f));
		triangle.Add( new Vector2(tmp.max.x/2.4f,  tmp.min.y/2.2f));
		triangle.Add( new Vector2(tmp.min.y/2.2f, tmp.min.x -tmp.min.x/2.6f));
	*/

		/*VectorLine triLine1; 
		Vector3[] side1 = new Vector3[]{triangle[0], triangle[1]};
		triLine1 = new VectorLine("tri line", side1, mat, lineWidth * 3, LineType.Discrete);
		triLine1.Draw();

		VectorLine triLine2; 
		Vector3[] side2 = new Vector3[]{triangle[1], triangle[2]};
		triLine2 = new VectorLine("tri line", side2, mat, lineWidth * 3, LineType.Discrete);
		triLine2.Draw();

		VectorLine triLine3; 
		Vector3[] side3 = new Vector3[]{triangle[2], triangle[0]};
		triLine3 = new VectorLine("tri line", new Vector3[]{triangle[2], triangle[0]}, mat, lineWidth * 3, LineType.Discrete);
		triLine3.Draw();*/


		/*sides.Add(side1);
		sides.Add(side2);
		sides.Add(side3);*/


		// Generate bounds (right now limited to square) from vertices

		Bounds b = BoundCheck.instance.getBounds(verts);


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
		boundaryLine = new VectorLine("Bound segment", boundsVerts, mat, lineWidth * 3, LineType.Continuous);
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
			segLine = new VectorLine("Road segment", (Vector3[])segment.getLine(), mat, lineWidth, LineType.Discrete);
			segLine.Draw3D();
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

	public void init(Vector2 position){

		this.position = position;
		turtle.initialize(angle, length, position);
		generate(generations);
		draw();

	}

	void draw(){

		print("drawing");
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



	public void JoinSegments (Vector3[] boundVertices, float testLength, bool testBothDirections = true) {

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

