using UnityEngine;
using System.Collections;
using System.Linq;

public class Pellet : Projectile {
	public float maxSpeed = 30;
	public float explosionRadius = .1f;
	public override float GetMaxSpd() {
		return maxSpeed;
	}
	public override float GetExplodeRadius() {
		return explosionRadius;
	}
}
