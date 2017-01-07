/**
    Author: Jesper Tjørnelund (jeth@itu.dk)
    The Leaf class is used for the Binary Search Partitioning (BSP) and serves as a data type for each leaf node the BSP tree is divided into. 
*/

using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour
{


    public int MIN_LEAF_SIZE = 6;

    public int x, y, height, width;

    //public Leaf leftChild;
    //public Leaf rightChild;
    public GameObject leftChild;
    public GameObject rightChild;

    public void Init(int _x, int _y, int _height, int _width)
    {
        this.x = _x;
        this.y = _y;
        this.height = _height;
        this.width = _width;
    }

    /*    public Leaf(int _x, int _y, int _height, int _width)
        {


            this.x = _x;
            this.y = _y;
            this.height = _height;
            this.width = _width;
        }*/



	public bool hasBeenSplit(){
		return !(leftChild == null && rightChild == null);

	}
    public bool Split()
    {
        if (leftChild || rightChild)
        {
            Debug.Log("Already split. Aborting...!");
            return false; // We're already split! ABORT!
        }

        bool splitH;
        if (Random.Range(0.0f, 1.0f) > 0.5f) // 50-50 for split direction unless height is considerably larger than width or vice verca (see if statement below).
        {
            splitH = false;
        }
        else
        {
            splitH = true;
        }

        if (width > height && width / height >= 1.25)
        {
            splitH = false;
        }
        else if (height > width && height / width >= 1.25f)
        {
            splitH = true;
        }

        int max = (splitH ? height : width) - MIN_LEAF_SIZE;
        if (max <= MIN_LEAF_SIZE)
        {
            Debug.Log("Area too small. Aborting split");
            return false; // area too small
        }

        int split = Random.Range(MIN_LEAF_SIZE, max);

        if (splitH)
        {
            //leftChild = new Leaf(x, y, split, width);
            //rightChild = new Leaf(x, y + split, height - split, width);

            leftChild = new GameObject();
            rightChild = new GameObject();
            leftChild.AddComponent<Leaf>();
            rightChild.AddComponent<Leaf>();
            leftChild.GetComponent<Leaf>().Init(x, y, split, width);
            rightChild.GetComponent<Leaf>().Init(x, y + split, height - split, width);

        }
        else
        {
            //leftChild = new Leaf(x, y, height, split);
            //rightChild = new Leaf(x + split, y, height, width - split);

            leftChild = new GameObject();
            rightChild = new GameObject();
            leftChild.AddComponent<Leaf>();
            rightChild.AddComponent<Leaf>();
            leftChild.GetComponent<Leaf>().Init(x, y, height, split);
            rightChild.GetComponent<Leaf>().Init(x + split, y, height, width - split);
        }

        Debug.Log("Split successful!");
        return true;

    }


}