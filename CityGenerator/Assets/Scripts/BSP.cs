/*
    Author: Jesper Tjørnelund (jeth@itu.dk)
    The BSP class is the class that initializes the Binary Search Partitioning method and creates Leafs. It takes in x, y, width and depth as parameters. 
*/

using UnityEngine;
using System.Collections.Generic;

public class BSP : MonoBehaviour
{
    public List<GameObject> buildings;
    public Material mat01;

    uint MAX_LEAF_SIZE = 6;

    //public List<Leaf> leafs = new List<Leaf>();
    public List<GameObject> leafs = new List<GameObject>();

    List<GameObject> tmpList = new List<GameObject>();

    int h;
    int w;

    Leaf root;

    // Use this for initialization
    void Start()
    {
        h = 100;
        w = 100;
        GameObject root = new GameObject();
        root.AddComponent<Leaf>();
        root.GetComponent<Leaf>().Init(0, 0, h, w);
        leafs.Add(root);

        bool did_split = true;
        
		int counter = 0;
		List<GameObject> tmp = new List<GameObject>();
		while (did_split) {
			did_split = false;
			for (int i = 0; i < leafs.Count; i++){
				Leaf leaf = leafs[i].GetComponent<Leaf>();
				if (leaf.width > MAX_LEAF_SIZE || leaf.height > MAX_LEAF_SIZE){
					
					if (!leaf.hasBeenSplit()){ // if leaf has not been split ...
						if (leaf.Split()){ //... and split was successful 
							// add leaf children
							tmp.Add(leaf.leftChild);
							tmp.Add(leaf.rightChild);
							did_split = true;
							counter++;
						}
					}
				}
			}
			foreach (var item in tmp) {
				leafs.Add(item);
			}
			tmp.Clear();
		}
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            AddBuildings();
        }
    }

    void AddBuildings()
    {
        BlockCreator bc = new BlockCreator();
        buildings = new List<GameObject>();
        foreach(GameObject go in leafs)
        {
           
            Leaf l = go.GetComponent<Leaf>();
            if (l.leftChild == null && l.rightChild == null) // make sure only to draw the last level of children to avoid overlaps. 
            {
                GameObject nb = bc.CreateBuilding(
                        new Vector2(l.x, l.y),
                        new Vector2(l.x + l.width, l.y),
                        new Vector2(l.x + l.width, l.y + l.height),
                        new Vector2(l.x, l.y + l.height),
                        Random.Range(10.0f, 30.0f)
                    );
                buildings.Add(nb);
            }
        }

       foreach(GameObject go in buildings)
        {
            
            go.GetComponent<MeshRenderer>().material = mat01;
        }
    }
}