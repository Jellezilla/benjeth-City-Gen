/*
    Author: Jesper Tjørnelund (jeth@itu.dk)
    The BSP class is the class that initializes the Binary Search Partitioning method and creates Leafs. It takes in x, y, width and depth as parameters. 
*/

using UnityEngine;
using System.Collections.Generic;

public class BSP : MonoBehaviour {

    uint MAX_LEAF_SIZE = 20;

    //public List<Leaf> leafs = new List<Leaf>();
    public List<GameObject> leafs = new List<GameObject>();
    int h;
    int w;

    Leaf root;
    
	// Use this for initialization
	void Start () {
        h = 100;
        w = 100;
        //GameObject root = new GameObject();
        //root = new Leaf(0, 0, h, w);
        GameObject root = new GameObject();
        //Leaf root = new Leaf();
        root.AddComponent<Leaf>();
        root.GetComponent<Leaf>().Init(0, 0, h, w);
       // root.AddComponent<Leaf>();
        leafs.Add(root);

        bool did_split = true;

        while(did_split)
        {
            did_split = false;
            foreach(GameObject go in leafs)
            {
                Leaf l = go.GetComponent<Leaf>();
                if(l.leftChild == null && l.rightChild == null)
                {
                    if(l.width > MAX_LEAF_SIZE || l.height > MAX_LEAF_SIZE)
                    {
                        if(l.Split())
                        {
                            leafs.Add(l.leftChild);
                            leafs.Add(l.rightChild);
                            
                            did_split = true;
                        }
                    } 
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
