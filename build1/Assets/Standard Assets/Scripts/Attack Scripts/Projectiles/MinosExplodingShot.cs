using UnityEngine;
using System.Collections;

public class MinosExplodingShot : ExplodingShot {
	
	protected override void RemoveMe(){
		KnockBack(targetGroup, transform.position, aoe*.6f, 0, damage);
		KnockBack(targetGroup, transform.position, aoe, knockbackStr, aoeDmg);
		GameObject boom = (GameObject) Instantiate(VariableScript.objBullet, transform.position, Quaternion.Euler(0,180,0));
		boom.transform.localScale = new Vector3(aoe * 3, 0, aoe * 3);
		BulletScript bs = boom.GetComponent<BulletScript>();
		bs.SetExpiry(.2f);
		boom.GetComponent<Collider>().enabled = false;
		AnimationScript.AnimateSprite(boom.GetComponent<Renderer>(), 4, 1, 2, 0, 1, 1);
		// Create 4 projectiles that fire off at different directions
		for(int i=0; i<4; i++){
			Quaternion quat = Quaternion.AngleAxis(360/4 * i, Vector3.up);
			Vector3 offset = quat * Vector3.forward * .5f;	
			GameObject projectile = (GameObject) Instantiate(VariableScript.objBullet, transform.position + offset, Quaternion.identity);
			projectile.GetComponent<BulletScript>().SetProjectileParams(projectile.transform.position + offset, 1, BulletScript.PLAYERS,2,20);
		}
		Destroy(gameObject);
	}
	
	void OnCollisionEnter(Collision other){
		RemoveMe();
	}
	
}
