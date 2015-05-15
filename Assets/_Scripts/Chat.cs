using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Chat : MonoBehaviour {
	public Text chatDisplay;

	//Rollin' our own queue of strings
	// private string curText = "";
	// private string curText1 = "";
	// private string curText2 = "";
	private string curText3 = "";
//	private bool writingText = false;


	private bool typing = false;
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
				if (curText3.Length != 0) {
					curText3 = curText3.Substring(0, curText3.Length - 1);
				}
			} else {
				if (c == '\n' || c == '\r') {
					if (typing){
						SayText(curText3);
						curText3 = "";
					}
					typing = !typing;
				} else {
					if (typing) {
						curText3 += c;
					}
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
		/*
		curText = curText1; 
		curText1 = curText2;
		curText2 = curText3;
		curText3 = text;
		text = curText + "\n" +  curText1 + "\n" + curText2 + "\n" + curText3;
		 */
		//chatDisplay.text += text + '\n' ;
		chatDisplay.text = text;
	}
}
