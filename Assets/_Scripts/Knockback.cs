using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour {

	//simple knockback - knocks back further based on force
	[RPC] void KnockbackPlayer(NetworkViewID playerID, int force){
		Rigidbody r = NetworkView.Find (playerID).gameObject.GetComponent<Rigidbody> ();
		r.AddForce (force * Vector3.back);
	}

}
