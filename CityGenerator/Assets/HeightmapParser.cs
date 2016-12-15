/*
    Author: Jesper Tjørnelund (jeth@itu.dk)
    The heightmapParser takes in a heightmap and stores each pixel in a dictionary with a vector2 (position index) as a key and the grayscale intensity as value. The value is to be used as density values. 
    
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HeightmapParser : MonoBehaviour {


    public Texture2D heightmap;

    public int tier1Chance;
    public float tier1Threshold;
    public int tier2Chance;
    public float tier2Threshold;
    public int tier3Chance;
    public float tier3Threshold;


    private float height;
    private float width;

    [SerializeField]
    public int amountOfDistricts;

    
    Dictionary<Vector2, float> densityValues = new Dictionary<Vector2, float>();

    List<Vector2> districtCenterPoints = new List<Vector2>();
    
    float maxVal;

	// Use this for initialization
	void Start () {
        height = heightmap.height;
        width = heightmap.width;
        
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                densityValues.Add(new Vector2(x, y), heightmap.GetPixel(x, y).grayscale);

            }
        }
        
    }
    

    public List<Vector2> GetDistrictCenterPoints()
    {
        List<Vector2> tmpList = new List<Vector2>();

        var myList = densityValues.ToList();
       

          
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));



        //Debug.Log("list length: " + myList.Count);

        // what about offsets to ensure District centers are positioned at a certain spacing?
        for(int i = 0; i < amountOfDistricts; i++)
        {
            //Vector2 tmp;
//            Debug.Log("I: " + i);
            int rand = Random.Range(1, 100);
            if (rand > tier1Chance) // tier 1
            {
                tmpList.Add(GetTierCoordinate(tier1Threshold));
                
            } else if (rand < tier1Chance && rand > tier2Chance) // tier 2
            {
                tmpList.Add(GetTierCoordinate(tier2Threshold));
            } else // tier 3
            {
                tmpList.Add(GetTierCoordinate(tier3Threshold));
            }
        }
        return tmpList;
    }

    float GetValFromCoordinates(Vector2 pos)
    {

        try
        {
           
            return densityValues[pos];
        }
        catch (KeyNotFoundException)
        {
            Debug.Log("Density count: " + densityValues.Count + " - trying to reach pos: (" + pos.x + "," + pos.y + "). Value: " + densityValues[pos]);
            return 0.0f;
        }
    }

    Vector2 GetTierCoordinate(float threshold)
    {

        Vector2 tmp = new Vector2(Random.Range(20, 235), Random.Range(20, 235));


        if (GetValFromCoordinates(tmp) > threshold)
        {
            return tmp;
        }
        else
        {
            return GetTierCoordinate(threshold);
            //return Vector2.zero;
        }
    }
        
}
