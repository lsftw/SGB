using UnityEngine;
using System.Collections;
using System.Linq;

public abstract class Projectile : MonoBehaviour {
	private Vector3 target;
	[RPC] public void setTarget(Vector3 target) {
		this.target = target;
	}
	public abstract float GetMaxSpd();
	public abstract float GetExplodeRadius();
	
	// Update is called once per frame
	void Update() {
		Vector3 oldPosition = transform.position;
		float step = GetMaxSpd() * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, target, step);
		Vector3 newPosition = transform.position;
		float distanceTravelled = Vector3.Distance(oldPosition, newPosition);
		if (distanceTravelled < .0001) {
			Kaboom();
		}
	}
	
	void Kaboom() {
		foreach (GameObject block in GetAllInRange(gameObject, GetExplodeRadius())) {
			if (block.tag == "Block") {
				Network.Destroy(block);
			}
		}
		Network.Destroy(gameObject);
	}
	
	System.Collections.Generic.IEnumerable<GameObject> GetAllInRange(GameObject centerObject, float radius) {
		Vector3 center = centerObject.GetComponent<Renderer>().bounds.center;
		Collider[] colliders = Physics.OverlapSphere(center, radius);
		return colliders.Select(collider => collider.gameObject);
	}
}
