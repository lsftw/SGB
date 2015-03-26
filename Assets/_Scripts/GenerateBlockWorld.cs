﻿using UnityEngine;
using System.Collections;

// Code adapted from answers.unity3d.com/questions/261023/block-based-map-generator.html
public class GenerateBlockWorld : MonoBehaviour {
	public Vector2 worldSize = new Vector2(10, 10);
	private GameObject blockWorld; // parent for all blocks

	//Height multiplies the final noise output
	public float Height = 10.0f;
	//This divides the noise frequency
	public float NoiseSize = 10.0f;
	//Function that inputs the position and spits out a float value based on the perlin noise
	float PerlinNoise(float x, float y) {
		//Generate a value from the given position, position is divided to make the noise more frequent.
		float noise = Mathf.PerlinNoise(x / NoiseSize, y / NoiseSize);
		//Return the noise value
		return noise * Height;
	}

	// Use this for initialization
	void Start () {
		blockWorld = new GameObject ("Terrain");
		blockWorld.transform.position = new Vector3 (worldSize.x / 2, 0, worldSize.y / 2);
		for(int x = 0; x <= worldSize.x; x++) {
			for(int y = 0; y <= worldSize.y; y++) {
				GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
				box.transform.position = new Vector3(x, PerlinNoise(x, y), y);
				box.transform.parent = blockWorld.transform;
			}
		}
		blockWorld.transform.position = Vector3.zero;
	}
}
