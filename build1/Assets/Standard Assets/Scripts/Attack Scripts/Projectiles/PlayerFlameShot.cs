using UnityEngine;
using System.Collections;

public class PlayerFlameShot : ExplodingShot {
	
	private float fireExpiry = 1;
	private float fireSize = 1;
//	private int fireDamage = 1;
	
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
		
		GameObject fireBomb = (GameObject) Instantiate(VariableScript.objFire, transform.position, Quaternion.Euler(0,180,0));
		FireScript fs = fireBomb.GetComponent<FireScript>();
		fs.expiry = fireExpiry;
		fireBomb.transform.localScale = new Vector3(fireSize, 0, fireSize);
		Destroy(gameObject);
	}
	
	void OnCollisionEnter(Collision other){
		RemoveMe();
	}
	
	public override void SetUp(Vector3 target, float charge, int damage){
		fireExpiry = charge;
		fireSize = .35f + charge * .1f;
		
		int basePrimaryDamage = Mathf.CeilToInt(3*charge + 2*damage);
		int aoeDamage = Mathf.FloorToInt(basePrimaryDamage * .5f);
		float aoe = ((.13f+.02f*charge)*3f + damage*.04f);
		float aoePush = 0;
				
		SetProjectileParams(target, basePrimaryDamage, aoeDamage, aoe, aoePush, 
			BulletScript.ENEMIES, 8, Vector3.Distance(target, transform.position));
		
		print ("Base: " + basePrimaryDamage + "\nAoe: " + aoeDamage);
	}
}
