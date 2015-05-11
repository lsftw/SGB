using UnityEngine;
using System.Collections;
using System.Linq;

public class Pellet : MonoBehaviour {
	public float maxSpeed = 30;
	public float explosionRadius = .1f;
	private Vector3 target;
	[RPC] public void setTarget(Vector3 target) {
		this.target = target;
	}

	// Update is called once per frame
	void Update() {
		if (target != null) {
			Vector3 oldPosition = transform.position;
			float step = maxSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, target, step);
			Vector3 newPosition = transform.position;
			float distanceTravelled = Vector3.Distance(oldPosition, newPosition);
			if (distanceTravelled < .0001) {
				Kaboom();
			}
		}
	}

	void Kaboom() {
		foreach (GameObject block in GetAllInRange(gameObject, explosionRadius)) {
			if (block.tag == "Block") {
				Network.Destroy(block);
			}
		}
		Destroy(gameObject);
	}
	
	System.Collections.Generic.IEnumerable<GameObject> GetAllInRange(GameObject centerObject, float radius) {
		Vector3 center = centerObject.GetComponent<Renderer>().bounds.center;
        Collider[] colliders = Physics.OverlapSphere(center, radius);
		return colliders.Select(collider => collider.gameObject);
    }
}
