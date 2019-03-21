using UnityEngine;
using System.Collections;

public class PlayerExplodingShot : ExplodingShot {

	void LateUpdate(){
		
	}
	
	protected override void RemoveMe(){
		// Knockback enemies
		KnockBack(targetGroup, transform.position, aoe*.6f, 0, damage);
		KnockBack(targetGroup, transform.position, aoe, knockbackStr, aoeDmg);
		// Create explosion
		GameObject boom = (GameObject) Instantiate(VariableScript.objBullet, transform.position, Quaternion.Euler(0,180,0));
		boom.transform.localScale = new Vector3(aoe * 3, 0, aoe * 3);
		BulletScript bs = boom.GetComponent<BulletScript>();
		bs.SetExpiry(.2f);
		boom.GetComponent<Collider>().enabled = false;
		AnimationScript.AnimateSprite(bs.plane.GetComponent<Renderer>(), 4, 1, 2, 0, 1, 1);
		// Shake camera
		VariableScript.scrPlayerScript1.playerCameraScript.ShakeCamera(.25f,.1f*aoe);
		Destroy(gameObject);
	}
	
	void OnCollisionEnter(Collision other){
		RemoveMe();
	}
	
	public override void SetUp(Vector3 target, float charge, int damage){
		int basePrimaryDamage = Mathf.CeilToInt(charge*charge*.5f + 2*damage);
		int aoeDamage = Mathf.FloorToInt(basePrimaryDamage * .5f);
		float aoe = ((.13f+.02f*charge)*3f + damage*.04f);
		float aoePush = ((charge-1)*(charge-1)*.05f*(1+.2f*damage));
				
		SetProjectileParams(target, basePrimaryDamage, aoeDamage, aoe, aoePush,
            ENEMIES, 8, Vector3.Distance(target, transform.position));

		print ("Base: " + basePrimaryDamage + "\nAoe: " + aoeDamage + "\nPush: " + aoePush);
	}
}
