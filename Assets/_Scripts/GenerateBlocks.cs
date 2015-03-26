using UnityEngine;
using System.Collections;

public class GenerateBlocks : MonoBehaviour {

	public Transform block;

	// Use this for initialization
	void Start () {
		for (int x = -5; x < 5; ++x) {
			for (int y = 0; y < 3; ++y) {
				for (int z = -5; z < 5; ++z){
					Instantiate(block, new Vector3(x, y, z), Quaternion.identity);
				}
			}
		}
	}

}
