using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeightMap : MonoBehaviour {


	public int width = 100;
	public int height = 100;
	public float maxHeight = 30; // in meters 

	public Texture2D densityTex; 
	private Texture2D noiseTex;
	private Texture2D combinedNoiseTex; 
	public float xOrg;
	public float yOrg;
	public float scale = 1.0f;
	private Color[] combinedPix;
	private Color[] pix;
	public GameObject combinedPlane;
	public GameObject perlinPlane;
	private List<GameObject> buildings = new List<GameObject>();
	public GameObject buildingsParent; 
	private Bounds cityBounds;
	public int buildingsPerRow = 20;



	float[,] map;
	// Use this for initialization
	void Start () {
		map = new float[width, height];
		noiseTex = new Texture2D(width, height);
		combinedNoiseTex =  new Texture2D(width, height);
		pix = new Color[noiseTex.width * noiseTex.height];
		combinedPix = new Color[noiseTex.width * noiseTex.height];
		TextureScale.Bilinear (densityTex, width, height);

		CalcNoise();
		Combine();
		PlaceCubes(buildingsPerRow);

		cityBounds = GetBounds(buildings);
		StartCoroutine(SetHeight());
	}


	void CalcNoise() {
		Vector2 offset = new Vector2(Random.Range(0,100),Random.Range(0,100));
		float y = 0.0f;
		while (y < noiseTex.height) {
			float x = 0.0f;
			while (x < noiseTex.width) {
				float xCoord = xOrg + x / noiseTex.width * scale;
				float yCoord = yOrg + y / noiseTex.height * scale;
				float sample = Mathf.PerlinNoise(offset.x + xCoord, offset.y + yCoord) + Time.deltaTime;
				pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
				x++;
			}
			y++;
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}

	IEnumerator SetHeight(){

		Debug.Assert(width * height > buildings.Count, "the height map is too small. There are more buildings than texels");
		yield return new WaitForEndOfFrame();

		int multiplier = width / buildingsPerRow;

		foreach (var item in buildings) {
			yield return new WaitForEndOfFrame();

			float upper_bonus = 0;
			float lower_bonus = 0;
			float h = combinedNoiseTex.GetPixel((int)(item.transform.position.x * multiplier), (int)(item.transform.position.z * multiplier)).grayscale;
		
			print(h);
			if (h > 0.65) {
				upper_bonus = Random.Range(0f, maxHeight/5);
			}
			else if (h < .25) {
				lower_bonus = Random.Range(0f, -(maxHeight * 0.2f));
			}

			item.transform.localScale = new Vector3(0.7f, (combinedNoiseTex.GetPixel((int)(item.transform.position.x * multiplier), (int)(item.transform.position.z * multiplier)).r) * maxHeight + lower_bonus + upper_bonus, 0.7f); // subtract 0.1 because lowest value is around 0.6
			item.transform.position = new Vector3(item.transform.position.x, item.transform.localScale.y/2, item.transform.position.z);
		} 


	}

	void Combine(){
		float y = 0.0f;
		while (y < combinedNoiseTex.height) {
			float x = 0.0f;
			while (x < combinedNoiseTex.width) {
				float xCoord = xOrg + x / noiseTex.width * scale;
				float yCoord = yOrg + y / noiseTex.height * scale;
				combinedPix[(int)(y * noiseTex.width + x)] = ((noiseTex.GetPixel((int)x, (int)y) + densityTex.GetPixel((int)x, (int)y)) / 2);
				x++;
			}
			y++;
		}
		combinedNoiseTex.SetPixels(combinedPix);
		combinedNoiseTex.Apply();

		Renderer rend;
		rend = combinedPlane.GetComponent<Renderer>();
		rend.material.mainTexture = combinedNoiseTex;

		Renderer perlinRend;
		perlinRend = perlinPlane.GetComponent<Renderer>();
		perlinRend.material.mainTexture = noiseTex;
	
	}


	void PlaceCubes(int amount){


		for (int i = 0; i < amount; i++) {
			for (int j = 0; j < amount; j++) {
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.position = new Vector3(i, 0, j);
				cube.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
				buildings.Add(cube);
				cube.transform.SetParent(buildingsParent.transform);
			}

		}
	}

	public Bounds GetBounds(List<GameObject> objs){
		Bounds b = new Bounds();
		b.min = new Vector2(objs[0].transform.position.x, objs[0].transform.position.y);
		b.max = new Vector2(objs[0].transform.position.x, objs[0].transform.position.y);

		Vector2 min = objs[0].transform.position;
		Vector2 max = objs[0].transform.position;


		for (int i = 0; i < objs.Count; i++) {


			if (objs[i].transform.position.x < min.x){
				min.x = objs[i].transform.position.x;
			}
			if (objs[i].transform.position.x > max.x){
				max.x = objs[i].transform.position.x;
			}
			if (objs[i].transform.position.y < min.y){
				min.y = objs[i].transform.position.y;
			}
			if (objs[i].transform.position.y > max.y){
				max.y = objs[i].transform.position.y;
			}

			b.min = min;
			b.max = max;
		}

		return b;
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}
