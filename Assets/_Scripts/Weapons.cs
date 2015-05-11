using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public enum Weapon {
	HAND, // melee, single block destruction
	DRILL, // melee, multiple block destruction
	CODE425 // melee but high ranged, I am become Death, destroyer of Blocks
};
// Handles weapon selection for a single player
public class Weapons : MonoBehaviour {
	private Weapon selectedWeapon = Weapon.HAND;
	private Camera raycastCamera;
	public GameObject prefabCode425;

	// Used for GUI text display of selected weapon
	public string getSelectedWeapon() {
		return selectedWeapon.ToString();
	}
	// TODO use constructor instead? Camera is mandatory for raycasting
	public void setCamera(Camera raycastCamera) {
		this.raycastCamera = raycastCamera;
	}

	void Start() {
	}
	
	void Update() {
		HandleWeaponSwap();
		HandleMouseClick();
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
		if (Input.GetMouseButtonDown(0)) {
			FireWeapon();
		}
	}

	private void FireWeapon() {
		switch (selectedWeapon) {
			case Weapon.HAND:
			DestroyOneFromCursor(5);
			break;
			case Weapon.DRILL:
			ExplodeMultipleFromCursor(5, .5f);
			break;
			case Weapon.CODE425:
			GameObject blockTarget = GetOneFromCursor(15);
			if (blockTarget != null) {
				Vector3 origin = raycastCamera.gameObject.transform.position;
				Vector3 target = blockTarget.GetComponent<Renderer>().bounds.center;
				FireCode425(origin, target);
			}
			// ExplodeMultipleFromCursor(15, 7f);
			break;
			default:
			Debug.Log("Unrecognized weapon: " + selectedWeapon);
			break;
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
			DestroyBlock(entity.GetComponent<NetworkView>().viewID);
		}
	}
	[RPC] void DestroyBlock(NetworkViewID blockId) {
		Network.Destroy(blockId);
	}
	[RPC] void FireCode425(Vector3 origin, Vector3 target) {
		GameObject projectile = (GameObject)Network.Instantiate(prefabCode425, origin, Quaternion.identity, 0);
		projectile.GetComponent<Code425>().setTarget(target);
	}
}
