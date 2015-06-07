using UnityEngine;
using System.Collections;

public class PlayerQuake : ExplodingShot {

	public Material crackedEarth;

	void LateUpdate(){
		
	}
	
	protected override void RemoveMe(){
		KnockBack(targetGroup, transform.position, aoe*.6f, 0, damage);
		KnockBack(targetGroup, transform.position, aoe, knockbackStr, aoeDmg);
//		boom.transform.localScale = new Vector3(aoe * 3, 0, aoe * 3);
		Vector3 scale = new Vector3(aoe * 1, 0, aoe * 1);
		GUIManager.FloatingTexture(crackedEarth,transform.position,scale,.2f);
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
		float aoe = ((.13f+.02f*charge)*3f + damage*.04f)*3f;
		float aoePush = ((charge-1)*(charge-1)*.2f*(1+.2f*damage))*.75f;
				
		SetProjectileParams(target, basePrimaryDamage, aoeDamage, aoe, aoePush, 
			BulletScript.ENEMIES, 8, 0);
		
		print ("Base: " + basePrimaryDamage + "\nAoe: " + aoeDamage);
	}
}
