using UnityEngine;
using System.Collections;

public class TargetGroundShot : BulletScript {

	public override void SetProjectileParams(	Vector3 projectileTarget, int projectileDamage, int projectileTargetGroup, 
							float projectileSpeed, float projectileDistance){
		moveVec = projectileTarget - transform.position;
		damage = projectileDamage;
		moveSpeed = projectileSpeed;
		timeUntilDeath = projectileDistance/projectileSpeed;
		targetGroup = projectileTargetGroup;
	}
	
	
}
