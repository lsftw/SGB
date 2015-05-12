using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public enum Weapon {
	HAND, // melee, single block destruction
	DRILL, // melee, multiple block destruction
	PELLET, // high ranged, high speed, single block destruction
	CODE425, // high ranged, I am become Death, destroyer of Blocks
	ICARUS // into the air
};
// Handles weapon selection for a single player
public class Weapons : MonoBehaviour {
	public GameObject prefabPellet;
	public GameObject prefabCode425;
	private Weapon selectedWeapon = Weapon.HAND;
	private Camera raycastCamera;
	// Ammo: Negative numbers mean unlimited ammo
	private Dictionary<Weapon, int> ammo = new Dictionary<Weapon, int>();
	private Dictionary<Weapon, double> COOLDOWNS = new Dictionary<Weapon, double>();
	private Dictionary<Weapon, double> cooldown = new Dictionary<Weapon, double>();

	private Weapon[] GetAllWeapons() {
		return (Weapon[])System.Enum.GetValues(typeof(Weapon));
	}
	public Weapons() {
		ammo.Add(Weapon.HAND, -1);
		COOLDOWNS.Add(Weapon.HAND, .25);
		ammo.Add(Weapon.DRILL, -1);
		COOLDOWNS.Add(Weapon.DRILL, .35);
		ammo.Add(Weapon.PELLET, 300);
		COOLDOWNS.Add(Weapon.PELLET, .5);
		ammo.Add(Weapon.CODE425, 1);
		COOLDOWNS.Add(Weapon.CODE425, 5);
		ammo.Add(Weapon.ICARUS, -1);
		COOLDOWNS.Add(Weapon.ICARUS, 30);
		foreach (Weapon weapon in GetAllWeapons()) {
			cooldown.Add(weapon, 0);
		}
	}
	public bool TryFiring() {
		bool haveAmmo = ammo[selectedWeapon] != 0;
		bool offCooldown = cooldown[selectedWeapon] <= 0;
		if (!haveAmmo || !offCooldown) {
			return false;
		} else {
			if (ammo[selectedWeapon] > 0) {
				ammo[selectedWeapon]--;
			}
			cooldown[selectedWeapon] += COOLDOWNS[selectedWeapon];
			return true;
		}
	}
	// Used for GUI text display of selected weapon
	public string GetWeaponDescription() {
		string weaponName = selectedWeapon.ToString();
		string ammoText = ammo[selectedWeapon].ToString();
		if (ammo[selectedWeapon] < 0) { // negative = unlimited ammo
			return string.Format("{0}", weaponName);
		}
		return string.Format("{0} x{1}", weaponName, ammoText);
	}
	// TODO use constructor instead? Camera is mandatory for raycasting
	public void setCamera(Camera raycastCamera) {
		this.raycastCamera = raycastCamera;
	}

	void Update() {
		CooldownWeapons(Time.deltaTime);
		HandleWeaponSwap();
		HandleMouseClick();
	}

	private void CooldownWeapons(float amount) {
		Weapon[] weapons = (Weapon[])System.Enum.GetValues(typeof(Weapon));
		foreach (Weapon weapon in weapons) {
			cooldown[weapon] -= amount;
			if (cooldown[weapon] < 0) {
				cooldown[weapon] = 0;
			}
		}
	}

	private void HandleWeaponSwap() {
		if (Input.GetKeyDown(KeyCode.Q)) { // previous weapon
			selectedWeapon = selectedWeapon.Prev();
		} else if (Input.GetKeyDown(KeyCode.E)) { // next weapon
			selectedWeapon = selectedWeapon.Next();
		}
	}
	private void HandleMouseClick() {
		//CODE FOR DESTROYING BLOCKS 
		// - NOTE: code involving mouse clicks is updated per frame, so this belongs in Update()
		// - NOTE: may have to propogate physics over to FixedUpdate if we want to add any...
		// - For now, testing with raycast for individual blocks that mouse is over (ie player looking at)
		// - May need to implement a cooldown between clicks/successful destroy commands
		if (Input.GetMouseButton(0)) {
			FireWeapon();
		}
	}

	private void FireWeapon() {
		if (TryFiring()) {
			switch (selectedWeapon) {
				case Weapon.HAND:
				DestroyOneFromCursor(5);
				break;
				case Weapon.DRILL:
				ExplodeMultipleFromCursor(5, .5f);
				break;
				case Weapon.PELLET:
				GameObject blockTarget = GetOneFromCursor(1000);
				if (blockTarget != null) {
					Vector3 origin = raycastCamera.gameObject.transform.position;
					Vector3 target = blockTarget.GetComponent<Renderer>().bounds.center;
					FirePellet(origin, target);
				}
				break;
				case Weapon.CODE425:
				blockTarget = GetOneFromCursor(1000);
				if (blockTarget != null) {
					Vector3 origin = raycastCamera.gameObject.transform.position;
					Vector3 target = blockTarget.GetComponent<Renderer>().bounds.center;
					FireCode425(origin, target);
				}
				break;
				case Weapon.ICARUS:
				gameObject.transform.position = new Vector3(0, 100, 0);
				break;
				default:
				Debug.Log("Unrecognized weapon: " + selectedWeapon);
				break;
			}
		}
	}

	private GameObject GetOneFromCursor(int maxDistance) {
		RaycastHit objectHit;
		Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out objectHit, maxDistance)) {
			//if they left click, objectHit will have the targeted object's data
			GameObject block = objectHit.collider.gameObject;
			return block;
		}
		return null;
	}
	private void DestroyOneFromCursor(int maxDistance) {
		RaycastHit objectHit;
		Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out objectHit, maxDistance)) {
			//if they left click, objectHit will have the targeted object's data
			GameObject block = objectHit.collider.gameObject;
			DestroyEntity(block);
		}
	}
	System.Collections.Generic.IEnumerable<GameObject> GetAllInRange(GameObject centerObject, float radius) {
		Vector3 center = centerObject.GetComponent<Renderer>().bounds.center;
        Collider[] colliders = Physics.OverlapSphere(center, radius);
		return colliders.Select(collider => collider.gameObject);
    }
	private void ExplodeMultipleFromCursor(int maxDistance, float radius) {
		RaycastHit objectHit;
		Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out objectHit, maxDistance)) {
			//if they left click, objectHit will have the targeted object's data
			GameObject initialBlock = objectHit.collider.gameObject;
			foreach (GameObject block in GetAllInRange(initialBlock, radius)) {
				DestroyEntity(block);
			}
		}
	}

	void DestroyEntity(GameObject entity) {
		// only allow destroying blocks
		if (entity.tag == "Block") { // YOU CAN USE == FOR STRING EQUALITY C# MVP
			DestroyBlock (entity.GetComponent<NetworkView> ().viewID);
		} else if (entity.tag == "Player") {
			KnockbackPlayer(entity.GetComponent<NetworkView>().viewID, 5);
		}
	}
	[RPC] void KnockbackPlayer(NetworkViewID playerID, int force){
		Rigidbody r = NetworkView.Find (playerID).gameObject.GetComponent<Rigidbody> ();
		r.AddForce (force * Vector3.back);
	}
	[RPC] void DestroyBlock(NetworkViewID blockId) {
		Network.Destroy(blockId);
	}
	[RPC] void FirePellet(Vector3 origin, Vector3 target) {
		GameObject projectile = (GameObject)Network.Instantiate(prefabPellet, origin, Quaternion.identity, 0);
		projectile.GetComponent<Pellet>().setTarget(target);
	}
	[RPC] void FireCode425(Vector3 origin, Vector3 target) {
		GameObject projectile = (GameObject)Network.Instantiate(prefabCode425, origin, Quaternion.identity, 0);
		projectile.GetComponent<Code425>().setTarget(target);
	}

}
