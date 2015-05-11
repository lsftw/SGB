using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextUpdater : MonoBehaviour {
	public Text weaponText;
	void Update() {
		UpdateWeaponText();
	}
	private void UpdateWeaponText() {
		if (GetLocalPlayer() != null) {
			Weapons playerWeapons = GetLocalPlayer().GetComponent<Weapons>();
			string weaponDescription = playerWeapons.GetWeaponDescription();
			weaponText.text = "TOOL: " + weaponDescription;
		}
	}
	private GameObject GetLocalPlayer() {
		return GameObject.FindWithTag("Player");
	}
}
