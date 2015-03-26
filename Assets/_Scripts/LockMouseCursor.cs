using UnityEngine;
using System.Collections;

public class LockMouseCursor : MonoBehaviour {
	public bool lockCursorAtStart = true;

	// Use this for initialization
	void Start () {
		if (lockCursorAtStart) {
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
//			Screen.lockCursor = false;
		} else {
			Cursor.lockState = CursorLockMode.Locked;
//			Screen.lockCursor = true;
		}
		bool cursorNormal = Cursor.lockState == CursorLockMode.None;
		Cursor.visible = cursorNormal;
	}
}
