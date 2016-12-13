using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Delaunay;
using Delaunay.Geo;

public class HeightmapParser : MonoBehaviour {


    public Texture2D heightmap;

    private float height;
    private float width;

    [SerializeField]
    private int amountOfDistricts;

    private int
        m_pointCount;

    private List<Vector2> m_points;
    private float m_mapWidth = 100;
    private float m_mapHeight = 50;
    private List<LineSegment> m_edges = null;
    private List<LineSegment> m_spanningTree;
    private List<LineSegment> m_delaunayTriangulation;

    //  Color[,] pixelVals;

    Dictionary<Vector2, float> densityValues = new Dictionary<Vector2, float>();

    List<Vector2> districtCenterPoints = new List<Vector2>();
    
    float maxVal;

	// Use this for initialization
	void Start () {
        m_pointCount = amountOfDistricts;
        height = heightmap.height;
        width = heightmap.width;

        //pixelVals = new Color[(int)width, (int)height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
        //        pixelVals[x, y] = heightmap.GetPixel(x, y);
                densityValues.Add(new Vector2(x, y), heightmap.GetPixel(x, y).grayscale);

            }
        }
        /*
        
        
        Debug.Log("length: " + pixelVals.Length);
        Debug.Log("width: " + width);
        Debug.Log("height: " + height);
        
        maxVal = 0.0f;
        
        foreach (Color c in pixelVals)
        {
            if(c.grayscale > 0.85f)
            {
                Debug.Log("Color: " + c.grayscale.ToString());

            }
            if (c.grayscale > maxVal)
            {
                maxVal = c.grayscale;
                
            }
        }
        Debug.Log("maxval: " + maxVal);
        */


    }

    void Update()
    {
        
        if(Input.anyKeyDown)
        {

            Demo();
          //districtCenterPoints = GetDistrictCenterPoints();
        }
    }


    void Demo()
    {

        List<uint> colors = new List<uint>();
        m_points = new List<Vector2>();

        m_points = GetDistrictCenterPoints();   
        
        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
        m_edges = v.VoronoiDiagram();
        /*
        m_spanningTree = v.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();
        */
    }


    public List<Vector2> GetDistrictCenterPoints()
    {
        List<Vector2> tmpList = new List<Vector2>();

        var myList = densityValues.ToList();
       

          
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

        

        //Debug.Log("list length: " + myList.Count);
        
        // what about offsets to ensure District centers are positioned at a certain spacing?
        for(int i = 0; i <= amountOfDistricts; i++)
        {
            //Vector2 tmp;

            int rand = Random.Range(1, 100);
            if (rand > 35) // tier 1
            {
                tmpList.Add(GetTierCoordinate(0.85f));
                
            } else if (rand < 35 && rand > 10) // tier 2
            {
                tmpList.Add(GetTierCoordinate(0.75f));
            } else // tier 3
            {
                tmpList.Add(GetTierCoordinate(0.55f));
            }

        }
        return tmpList;
    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.Log("sa:" + districtCenterPoints.Count);
        for (int i = 0; i < districtCenterPoints.Count; i++)
        {
            Gizmos.DrawSphere(districtCenterPoints[i], GetValFromCoordinates(districtCenterPoints[i]));
        }

    }
    */
    float GetValFromCoordinates(Vector2 pos)
    {
        return densityValues[pos];
    }

    Vector2 GetTierCoordinate(float threshold)
    {

        Vector2 tmp = new Vector2(Random.Range(0, 255), Random.Range(0, 255));

        //int x = Random.Range(0, 255);
        //int y = Random.Range(0, 255);
        if (GetValFromCoordinates(tmp) > threshold)
        {
            return tmp;
        }
        else
        {
//            Debug.Log("Coordinate value did not match threshold. Trying again...");
            return GetTierCoordinate(threshold);
        }

        //return tmp;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_points != null)
        {
            for (int i = 0; i < m_points.Count; i++)
            {
                Gizmos.DrawSphere(m_points[i], 0.2f);
            }
        }

        if (m_edges != null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < m_edges.Count; i++)
            {
                Vector2 left = (Vector2)m_edges[i].p0;
                Vector2 right = (Vector2)m_edges[i].p1;
                Gizmos.DrawLine((Vector3)left, (Vector3)right);
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, m_mapHeight));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(m_mapWidth, 0));
        Gizmos.DrawLine(new Vector2(m_mapWidth, 0), new Vector2(m_mapWidth, m_mapHeight));
        Gizmos.DrawLine(new Vector2(0, m_mapHeight), new Vector2(m_mapWidth, m_mapHeight));
    }
}
