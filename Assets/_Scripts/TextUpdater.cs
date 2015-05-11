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
			string selectedWeapon = GetLocalPlayer().GetComponent<Weapons>().getSelectedWeapon();
			weaponText.text = "TOOL: " + selectedWeapon;
		}
	}
	private GameObject GetLocalPlayer() {
		return GameObject.FindWithTag("Player");
	}
}
