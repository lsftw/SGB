using UnityEngine;
using System.Collections;
using System.Linq;

public class Code425 : Projectile {
	public float maxSpeed = 10;
	public float explosionRadius = 7f;
	public override float GetMaxSpd() {
		return maxSpeed;
	}
	public override float GetExplodeRadius() {
		return explosionRadius;
	}
}
