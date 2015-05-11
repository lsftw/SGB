using UnityEngine;
using System.Collections;

// Locks mouse cursor to center of screen
public class LockMouseCursor : MonoBehaviour {
	public bool lockCursorAtStart = true;

	void Start () {
		if (lockCursorAtStart) {
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	
	void ToggleCursorLock() {
		if (Cursor.lockState == CursorLockMode.None) {
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.lockState = CursorLockMode.None;
		}
		// bool cursorNormal = Cursor.lockState == CursorLockMode.None;
		// Cursor.visible = cursorNormal;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape)) {
			ToggleCursorLock();
		}
	}
}
