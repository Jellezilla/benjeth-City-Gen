using UnityEngine;
using System.Collections;

public class TestLines : MonoBehaviour {

    LineRenderer lr;

   // Mesh mesh;
   // public Vector3[] newVertices;
    // Use this for initialization
    void Start () {
        lr = GetComponent<LineRenderer>();
   //     mesh = GetComponent<MeshFilter>().mesh;


        lr.SetPosition(0, new Vector3(-9, 5, 0));
        lr.SetPosition(1, new Vector3(9, 5, 0));
        
        lr.SetPosition(2, new Vector3(9, 5, 0));
        lr.SetPosition(3, new Vector3(9, -5, 0));

        lr.SetPosition(4, new Vector3(9, -5, 0));
        lr.SetPosition(5, new Vector3(-9, -5, 0));

        lr.SetPosition(6, new Vector3(-9, -5, 0));
        lr.SetPosition(7, new Vector3(-9, 5, 0));

        //    newVertices = new Vector3[4];

        //        newVertices
        //   mesh.vertices = newVertices;
        /*
           Mesh mesh = GetComponent<MeshFilter>().mesh;
           mesh.Clear();
           mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0),
               new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1)
           };
          // mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
           mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };
           */


        GameObject plane = new GameObject("Plane");
        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = CreateMesh(1, 0.2f);
        MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material.shader = Shader.Find("Particles/Additive");
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.green);
        tex.Apply();
        renderer.material.mainTexture = tex;
        renderer.material.color = Color.green;

    }
    Mesh CreateMesh(float width, float height)
    {
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        m.vertices = new Vector3[] {
         new Vector3(-width, -height, 0.01f),
         new Vector3(width, -height, 0.01f),
         new Vector3(width, height, 0.01f),
         new Vector3(-width, height, 0.01f)
        };
        m.uv = new Vector2[] {
         new Vector2 (0, 0),
         new Vector2 (0, 1),
         new Vector2(1, 1),
         new Vector2 (1, 0)
        };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();

        return m;
    }
    // Update is called once per frame
    void Update () {
	
	}
}
