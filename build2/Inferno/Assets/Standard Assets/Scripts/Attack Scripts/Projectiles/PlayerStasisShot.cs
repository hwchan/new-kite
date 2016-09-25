using UnityEngine;
using System.Collections;

public class PlayerStasisShot : ExplodingShot {
	
	void LateUpdate(){
		
	}
	
	protected override void RemoveMe(){
		KnockBack(targetGroup, transform.position, aoe*.6f, 0, damage);
		KnockBack(targetGroup, transform.position, aoe, knockbackStr, aoeDmg);
		GameObject boom = (GameObject) Instantiate(VariableScript.objBullet, transform.position, Quaternion.Euler(0,180,0));
		boom.transform.localScale = new Vector3(aoe * 3, 0, aoe * 3);
		BulletScript bs = boom.GetComponent<BulletScript>();
		bs.SetExpiry(.2f);
		boom.GetComponent<Collider>().enabled = false;
		AnimationScript.AnimateSprite(boom.GetComponent<Renderer>(), 4, 1, 2, 0, 1, 1);
		
		Destroy(gameObject);
	}
	
	void OnCollisionEnter(Collision other){
		RemoveMe();
	}
	
	public override void SetUp(Vector3 target, float charge, int damage){
		slowDur = .5f + .75f * charge;
		slowStr = 1f;
		
		int basePrimaryDamage = Mathf.CeilToInt((3*charge + 2*damage)*.5f);
		int aoeDamage = Mathf.FloorToInt(basePrimaryDamage * .5f);
		float aoe = ((.13f+.02f*charge)*3f + damage*.04f);
		float aoePush = (charge*.5f + .2f*damage)*.5f;
				
		SetProjectileParams(target, basePrimaryDamage, aoeDamage, aoe, aoePush, 
			BulletScript.ENEMIES, 12, Vector3.Distance(target, transform.position));
		
		print ("Base: " + basePrimaryDamage + "\nAoe: " + aoeDamage);
	}
}
