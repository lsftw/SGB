using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DupeText : MonoBehaviour {
	public Text original;
	public Text shadowUp;
	public Text shadowDown;
	public Text shadowRight;
	public Text shadowLeft;

	// Update is called once per frame
	void Update () {
		shadowUp.text = original.text;
		shadowDown.text = original.text;
		shadowRight.text = original.text;
		shadowLeft.text = original.text;
	}
}
