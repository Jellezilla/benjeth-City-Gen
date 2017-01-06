using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;

public class VoronoiRegion{

	public Vector2 center;
	public List<RoadSegment> boundary; 
	public bool isBorderRegion = false;
	public Vector2 min;
	public Vector2 max;
}


public class VoronoiRunner : MonoBehaviour
{
    [SerializeField]
    private int
        m_pointCount;

	private bool populated = false;
	public LSystem LSystem;
	public bool showExtraGraphics = false;
	public bool newDiagramOnKeyPress = false;
    private List<Vector2> m_points;
    private float m_mapWidth;
    private float m_mapHeight;
    private List<LineSegment> m_edges = null;
    private List<LineSegment> m_spanningTree;
    private List<LineSegment> m_delaunayTriangulation;
    HeightmapParser hmp;
	List<VoronoiRegion> regions = new List<VoronoiRegion>();
	List<RoadSegment> districtSegments = new List<RoadSegment>();
	List<RoadSegment> borders = new List<RoadSegment>();

    void Awake()
    {
        hmp = GameObject.FindGameObjectWithTag("HeightmapParser").GetComponent<HeightmapParser>();
        m_mapWidth = hmp.heightmap.width;
        m_mapHeight = hmp.heightmap.height;

		RoadSegment seg1 = new RoadSegment();
		seg1.start = new Vector2(0, 0);
		seg1.end = new Vector2(0, m_mapHeight);
		seg1.id = 100001;

		RoadSegment seg2 = new RoadSegment();
		seg2.start = new Vector2(0, 0);
		seg2.end =  new Vector2(m_mapWidth, 0);
		seg2.id = 100002;

		RoadSegment seg3 = new RoadSegment();
		seg3.start = new Vector2(m_mapWidth, 0);
		seg3.end =  new Vector2(m_mapWidth, m_mapHeight);
		seg3.id = 100003;

		RoadSegment seg4 = new RoadSegment();
		seg4.start = new Vector2(0, m_mapHeight);
		seg4.end = new Vector2(m_mapWidth, m_mapHeight);
		seg4.id = 100004;

		borders.Add(seg1);
		borders.Add(seg2);
		borders.Add(seg3);
		borders.Add(seg4);

	
    }
		
	void Start (){
		Run();

	}
    void Update()
    {
		if (Input.anyKeyDown && newDiagramOnKeyPress)
        {
            Run();
        }
		else if (Input.GetKeyDown(KeyCode.N) && !populated){
			StartCoroutine(buildCity(.1f));


		}
    }

	IEnumerator buildCity(float delay){

		populated = true;
		foreach(Vector2 point in m_points){
			//yield return new WaitForSeconds(delay);
			yield return new WaitForEndOfFrame();

			int counter = 0;
			List<RoadSegment> bounds = GetBoundaries(point, 10000);
			VoronoiRegion tmp = regions[counter];
			List<Vector3[]> sides = new List<Vector3[]>();
			List<Vector3> verts = new List<Vector3>();

			foreach(RoadSegment seg in bounds){
				sides.Add(seg.getLine());
				verts.Add(seg.end);
			}

			LSystem clone = (LSystem) Instantiate(LSystem);
			clone.init(point);

		/*	if (!tmp.isBorderRegion){
				clone.center(clone.getCenter(), clone.getCenter(bounds));
				clone.redraw();
			}*/

			clone.confineToBounds(sides);
			//	clone.joinSegments(verts.ToArray(), 1.1f);

			//clone.JoinSegments(verts.ToArray(), 1.1f);

			/*if (!tmp.isBorderRegion){
				while ((!clone.fillBounds(tmp.min, tmp.max) && counter < 1)){
					yield return new WaitForSeconds(0.005f);
					clone.reset();
					clone.init(point);
					counter++;

				}
			}

			clone.confineToBounds(sides);*/
		}

	

	}

    private void Run()
    {
        

        m_pointCount = hmp.amountOfDistricts;

        List<uint> colors = new List<uint>();
        m_points = new List<Vector2>();
        for (int i = 0; i < m_pointCount; i++)
        {
            colors.Add(0);
            //m_points.Add (new Vector2 (
            //       UnityEngine.Random.Range (0, m_mapWidth),
            //     UnityEngine.Random.Range (0, m_mapHeight))
            //);
        }

        m_points = hmp.GetDistrictCenterPoints();


        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
        m_edges = v.VoronoiDiagram();

        m_spanningTree = v.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();

		int counter = 0;
		for (int i = 0; i < m_edges.Count; i++)
		{
			Vector2 left = (Vector2)m_edges[i].p0;
			Vector2 right = (Vector2)m_edges[i].p1;

			RoadSegment segment = new RoadSegment();
			segment.start = left;
			segment.end = right;
			segment.id = counter;
			districtSegments.Add(segment);
			counter++;
		}
	


    }

	List<RoadSegment> GetBoundaries(Vector2 origin, float dist, int testCount = 360){

		Debug.Assert ( testCount > 0, "testCount has to be a positive non zero value");

		float x = 360/testCount; // degrees
		Vector2 point = new Vector2(origin.x, origin.y + dist); // start checking above origin
		List <RoadSegment> boundary = new List<RoadSegment>();
		bool isBorderRegion = false;
		for (int i = 0; i < testCount; i++){

			float newX = origin.x + (point.x-origin.x) * Mathf.Cos(x) - (point.y-origin.y) * Mathf.Sin(x);
			float newY = origin.y + (point.x-origin.x) * Mathf.Sin(x) + (point.y-origin.y) * Mathf.Cos(x);
			point = new Vector2(newX, newY);

			RoadSegment intersected;
			if ((intersected = LineLineIntersection.instance.MultipleLineSegmentIntersection(origin, point, districtSegments.ToArray())) != null){
		
				if (!boundary.Contains(intersected)){
					boundary.Add(intersected);
				}
			}
			else {
				isBorderRegion = true;
			}
		}
			
		VoronoiRegion vr = new VoronoiRegion();
		vr.center = origin;
		vr.boundary = boundary;
		vr.isBorderRegion = isBorderRegion;

		float minX = origin.x; 
		float maxX = origin.x;

		float minY = origin.y;
		float maxY = origin.y;

		foreach (RoadSegment seg in boundary){

			if (seg.start.x < minX)
				minX = seg.start.x;
			if (seg.end.x < minX)
				minX = seg.end.x;

			if (seg.start.x > maxX)
				maxX = seg.start.x;
			if (seg.end.x > maxX)
				maxX = seg.end.x;

			if (seg.start.y < minY)
				minY = seg.start.y;
			if (seg.end.y < minY)
				minY = seg.end.y;

			if (seg.start.y > maxY)
				maxY = seg.start.y;
			if (seg.end.y > maxY)
				maxY = seg.end.y;
		}
		vr.min = new Vector2(minX, minY);
		vr.max = new Vector2(maxX, maxY);
		//print("pos : " + vr.center+ ", min " + vr.min + ", max: "+ vr.max);

		regions.Add(vr);

		return boundary;

	}

    void OnDrawGizmos()
    {
       

        if (m_edges != null)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < m_edges.Count; i++)
            {
                Vector2 left = (Vector2)m_edges[i].p0;
                Vector2 right = (Vector2)m_edges[i].p1;
                Gizmos.DrawLine((Vector3)left, (Vector3)right);
            }
        }
		if (showExtraGraphics){

		Gizmos.color = Color.red;
		if (m_points != null)
		{
			for (int i = 0; i < m_points.Count; i++)
			{
				if (i == 0)
					Gizmos.color = Color.cyan;
				else 
					Gizmos.color = Color.red;

				Gizmos.DrawSphere(m_points[i], 1f);
			}
		}

	        Gizmos.color = Color.magenta;
	        if (m_delaunayTriangulation != null)
	        {
	            for (int i = 0; i < m_delaunayTriangulation.Count; i++)
	            {
	                Vector2 left = (Vector2)m_delaunayTriangulation[i].p0;
	                Vector2 right = (Vector2)m_delaunayTriangulation[i].p1;
	                Gizmos.DrawLine((Vector3)left, (Vector3)right);
	            }
	        }

	        if (m_spanningTree != null)
	        {
	            Gizmos.color = Color.green;
	            for (int i = 0; i < m_spanningTree.Count; i++)
	            {
	                LineSegment seg = m_spanningTree[i];
	                Vector2 left = (Vector2)seg.p0;
	                Vector2 right = (Vector2)seg.p1;
	                Gizmos.DrawLine((Vector3)left, (Vector3)right);
	            }
	        }

		}
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, m_mapHeight));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(m_mapWidth, 0));
        Gizmos.DrawLine(new Vector2(m_mapWidth, 0), new Vector2(m_mapWidth, m_mapHeight));
        Gizmos.DrawLine(new Vector2(0, m_mapHeight), new Vector2(m_mapWidth, m_mapHeight));
    }


	public List<Vector2> getPoints(){

		return m_points;
	}
}