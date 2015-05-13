using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class TextUpdater : MonoBehaviour {
	public Text weaponText;

	public Text Player1Score;
	public Text Player2Score;


	void Update() {
		UpdateWeaponText();
		UpdateScoreText ();
	}
	private void UpdateWeaponText() {
		if (GetLocalPlayer() != null) {
			Weapons playerWeapons = GetLocalPlayer().GetComponent<Weapons>();
			string weaponDescription = playerWeapons.GetWeaponDescription();
			weaponText.text = "TOOL: " + weaponDescription;
		}
	}
	/*
	public void registerWin(){
		myScore += 1;
	}
	public void registerLose(){
		theirScore += 1;
	}
	*/


	void UpdateScoreText(){

		if (GetSharedData () != null) { //should never be null after game starts
			Player1Score.text = "My Score: " + GetSharedData ().GetComponent<SharedData> ().getMyScore ();
			Player2Score.text = "Opponent's Score: " + GetSharedData ().GetComponent<SharedData> ().getTheirScore (); 
		} else {
			Debug.Log("Shared data is null!");
		}
	}

	private GameObject GetLocalPlayer() {
		return GameObject.FindWithTag("Player");
	}
	
	private GameObject GetSharedData(){
		return GameObject.FindGameObjectWithTag ("SharedData");
	}
}
