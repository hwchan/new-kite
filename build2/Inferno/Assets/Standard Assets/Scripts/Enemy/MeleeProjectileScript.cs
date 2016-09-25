using UnityEngine;
using System.Collections;

public class MeleeProjectileScript : BulletScript {

	public Material meleeEffect; 

	protected override void RemoveMe(PlayerScript ps){
		if(ps!=null){
			GUIManager.FloatingTexture(meleeEffect,((ps.transform.position+transform.position)*.5f),.3f,4,1,0,0,4,12);
		}
		Destroy(gameObject);
	}


}
