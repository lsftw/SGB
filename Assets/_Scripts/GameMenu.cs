using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {

	public bool isQuitButton;
	
	void OnMouseEnter(){
		//change text color
		this.GetComponent<Renderer>().material.color=Color.red;
	}
	
	void OnMouseExit(){
		//change text color
		this.GetComponent<Renderer> ().material.color = Color.white;//new Color(50, 0, 250, 255);
	}

	void OnMouseUp(){
		//is this quit
		if (isQuitButton == true) {
			//quit the game
			Application.Quit();
		}
		else {
			//load level
			//TODO does this work? 
			Application.LoadLevel("Game");
			//Application.LoadLevel(1);
		}
	}
	
	void Update(){
		//quit game if escape key is pressed
		if (Input.GetKey(KeyCode.Escape)) { 
			Application.Quit();
		}
	}
}
