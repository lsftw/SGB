using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Weapon {
	HAND, // melee, single block destruction
	DRILL // melee, multiple block destruction
};
// Handles weapon selection for a single player
public class Weapons : MonoBehaviour {
	private Weapon selectedWeapon = Weapon.HAND;
	private Camera camera;

	// Used for GUI text display of selected weapon
	public string getSelectedWeapon() {
		return selectedWeapon.ToString();
	}
	// TODO use constructor instead? Camera is mandatory for raycasting
	public void setCamera(Camera camera) {
		this.camera = camera;
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
			for (int i = 0; i < 3; i++) {
				DestroyOneFromCursor(5);
			}
			break;
			default:
			Debug.Log("Unrecognized weapon: " + selectedWeapon);
			break;
		}
	}

	private void DestroyOneFromCursor(int maxDistance) {
		RaycastHit objectHit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out objectHit, maxDistance)){
			//if they left click, objectHit will have the targeted object's data
			GameObject block = objectHit.collider.gameObject;
			DestroyEntity(block);
		}
	}

	// old non-rpc version
	// void DestroyBlockAtCursor(GameObject block) {
		// block.SetActive(false);
	// }

	void DestroyEntity(GameObject entity) {
		// only allow destroying blocks
		if (entity.tag == "Block") { // YOU CAN USE == FOR STRING EQUALITY C# MVP
			DestroyBlock(entity.GetComponent<NetworkView>().viewID);
		}
	}
	[RPC] void DestroyBlock(NetworkViewID blockId) {
		//block.SetActive(false);
		Network.Destroy(blockId);
		// if (GetComponent<NetworkView>().isMine)
			// GetComponent<NetworkView>().RPC("DestroyBlockAtCursor", RPCMode.OthersBuffered, blockId);
	}
}
