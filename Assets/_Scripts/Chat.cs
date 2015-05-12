using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Chat : MonoBehaviour {
	public Text chatDisplay;
	private string curText = "";
//	private bool writingText = false;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		// if (Input.GetKeyDown(KeyCode.Return)) {
			// if (writingText) {
				// SayText(curText);
			// } else {
				// writingText = true;
			// }
		// }
		// if (writingText) {
			// foreach (char c in Input.inputString) {
				// if (c == "\b"[0]) {
					// if (curText.Length != 0) {
						// curText = curText.Substring(0, curText.Length - 1);
					// } else {
						// curText += c;
					// }
				// }
				// /* if (c == "\b"[0])
					// if (gt.text.Length != 0)
						// gt.text = gt.text.Substring(0, gt.text.Length - 1);
					
				// else
					// if (c == "\n"[0] || c == "\r"[0])
						// print("User entered his name: " + gt.text);
					// else
						// gt.text += c; */
			// }
		// }
		foreach (char c in Input.inputString) {
			if (c == '\b') {
				if (curText.Length != 0) {
					curText = curText.Substring(0, curText.Length - 1);
				}
			} else {
				if (c == '\n' || c == '\r') {
					SayText(curText);
					curText = "";
				} else {
					curText += c;
				}
			}
		}
	}

	// [RPC] void SayText(string text) {
	void SayText(string text) {
		GetComponent<NetworkView>().RPC ("UpdateText", RPCMode.Others, text);
		UpdateText(text);
	}
	[RPC] void UpdateText(string text) {
		Debug.Log(text);
		// chatDisplay.text += text + '\n';
		chatDisplay.text = text;
	}
}
