using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DupeText : MonoBehaviour {
	public Text original;
	public Text copy;

	// Update is called once per frame
	void Update () {
		copy.text = original.text;
	}
}
