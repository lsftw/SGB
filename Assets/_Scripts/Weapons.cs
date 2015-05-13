using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityStandardAssets.Characters.FirstPerson;

public enum Weapon {
	HAND, // melee, single block destruction
	DRILL, // melee, multiple block destruction
	PELLET, // high ranged, high speed, single block destruction
	CODE425, // high ranged, I am become Death, destroyer of Blocks
	ICARUS // into the air
};
// Handles weapon selection for a single player
public class Weapons : MonoBehaviour {
	public const int LAYER_LOCAL_PLAYER = 8;

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
		COOLDOWNS.Add(Weapon.HAND, .2);
		ammo.Add(Weapon.DRILL, -1);
		COOLDOWNS.Add(Weapon.DRILL, .3);
		ammo.Add(Weapon.PELLET, 425);
		COOLDOWNS.Add(Weapon.PELLET, .4);
		ammo.Add(Weapon.CODE425, 2);
		COOLDOWNS.Add(Weapon.CODE425, 60);
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
		string ammoText = " x" + ammo[selectedWeapon].ToString();
		string cdText = cooldown[selectedWeapon].ToString("n1");
		if (ammo[selectedWeapon] < 0) { // negative = unlimited ammo
			ammoText = "";
		}
		if (cooldown[selectedWeapon] <= 0) {
			cdText = "";
		} else {
			cdText = " [" + cdText + "s]";
		}
		return string.Format("{0}{1}{2}", weaponName, ammoText, cdText);
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
				Vector3 origin = raycastCamera.gameObject.transform.position;
				Vector3 point;
				if (GetRayFromCursor(out point)) {
					FirePellet(origin, point);
				}
				break;
				case Weapon.CODE425:
				origin = raycastCamera.gameObject.transform.position;
				if (GetRayFromCursor(out point)) {
					FireCode425(origin, point);
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

	private bool GetRayFromCursor(out Vector3 point) {
		RaycastHit objectHit;
		Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out objectHit, Mathf.Infinity, ~LAYER_LOCAL_PLAYER)) {
			point = objectHit.point;
			return true;
		}
		Debug.Log ("none");
		point = new Vector3(0, 0, 0);
		return false;
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
			Debug.Log("Hit object!");
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

		//
		//
		Debug.Log ("destroy entity!");
		Debug.Log (entity.gameObject.GetComponent<ModifiedFirstPerson> () != null);
		if (entity.tag == "Block") { // YOU CAN USE == FOR STRING EQUALITY C# MVP
			DestroyBlock (entity.GetComponent<NetworkView> ().viewID);
		} else if (entity.gameObject.GetComponent<ModifiedFirstPerson>() != null) {
			Debug.Log ("hit player!");
			KnockbackPlayer(entity.GetComponent<NetworkView>().viewID, 1000);
		}
		//
		//
	}
	[RPC] void KnockbackPlayer(NetworkViewID playerID, int force){
		//TODO not working yet!
		//Rigidbody r = NetworkView.Find (playerID).gameObject.GetComponent<Rigidbody> ();
		//r.isKinematic = false;
		//r.MovePosition (r.transform.position + force * new Vector3 (1, 1, 1));
		//r.AddForce (force * new Vector3(1, 1, 1)); //can't add force with player is kinematic
		//r.isKinematic = true;
	}
	[RPC] void DestroyBlock(NetworkViewID blockId) {
		Network.Destroy(blockId);
	}
	[RPC] void FirePellet(Vector3 origin, Vector3 direction) {
		GameObject projectile = (GameObject)Network.Instantiate(prefabPellet, origin, Quaternion.identity, 0);
		projectile.GetComponent<Pellet>().setTarget(direction);
	}
	[RPC] void FireCode425(Vector3 origin, Vector3 direction) {
		GameObject projectile = (GameObject)Network.Instantiate(prefabCode425, origin, Quaternion.identity, 0);
		projectile.GetComponent<Code425>().setTarget(direction);
	}

}
