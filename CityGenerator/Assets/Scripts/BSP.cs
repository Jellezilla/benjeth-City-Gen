/*
    Author: Jesper Tjørnelund (jeth@itu.dk)
    The BSP class is the class that initializes the Binary Search Partitioning method and creates Leafs. It takes in x, y, width and depth as parameters. 
*/

using UnityEngine;
using System.Collections.Generic;

public class BSP : MonoBehaviour
{

    uint MAX_LEAF_SIZE = 20;

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
        //GameObject root = new GameObject();
        //root = new Leaf(0, 0, h, w);
        GameObject root = new GameObject();
        //Leaf root = new Leaf();
        root.AddComponent<Leaf>();
        root.GetComponent<Leaf>().Init(0, 0, h, w);
        // root.AddComponent<Leaf>();
        leafs.Add(root);

        bool did_split = true;

       // DidSplit(did_split);
      /*  
        while (did_split)
        {
            did_split = false;
            foreach (GameObject go in leafs)
            {
                Leaf l = go.GetComponent<Leaf>();
                if (l.leftChild == null && l.rightChild == null)
                {
                    if (l.width > MAX_LEAF_SIZE || l.height > MAX_LEAF_SIZE)
                    {
                        if (l.Split())
                        {
                            leafs.Add(l.leftChild);
                            leafs.Add(l.rightChild);

                            did_split = true;
                        }
                    }
                }
            }
        }*/
		int counter = 0;
		List<GameObject> tmp = new List<GameObject>();
		while (did_split) {

			did_split = false;
			for (int i = 0; i < leafs.Count; i++){
				Leaf leaf = leafs[i].GetComponent<Leaf>();
				if (leaf.width > MAX_LEAF_SIZE || leaf.height > MAX_LEAF_SIZE){
					
					if (!leaf.hasBeenSplit() && counter <= 5){ // if leaf has not been split ...
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

    // Update is called once per frame
    void Update()
    {

    }

    void DidSplit(bool did_split)
    {

        //List<Leaf> tmpList = new List<Leaf>();
        //List<GameObject> tmpList = new List<GameObject>();

        did_split = false; 
        foreach (GameObject go in leafs)
        {
            Leaf l = go.GetComponent<Leaf>();
            if(l.leftChild == null && l.rightChild == null)
            {
                if (l.width > MAX_LEAF_SIZE || l.height > MAX_LEAF_SIZE)
                {
                    if (l.Split())
                    {
//                        leafs.Add(l.leftChild);
//                        leafs.Add(l.rightChild);

                        tmpList.Add(l.leftChild);
                        tmpList.Add(l.rightChild);

                        did_split = true;
                        Debug.Log("tykke");
                        DidSplit(did_split);
                    }
                } else {
                    Debug.Log("Space too small, stopping...");
                    return;
                }
            }
        }
       // UpdateList();
    }

    void UpdateList()
    {
        leafs.Add(tmpList[0]);
        leafs.Add(tmpList[1]);
        tmpList.Clear();
        /*
        foreach(GameObject go in tmpList)
        {
            
            leafs.Add(go);
            tmpList.Remove(go);
        }*/
    }
}