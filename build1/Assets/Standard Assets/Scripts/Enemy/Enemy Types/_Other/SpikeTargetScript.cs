using UnityEngine;
using System.Collections;

public class SpikeTargetScript : ExpiryScript {

	public int targets = 1 << 9;	// player
	public int damage = 0;
	public float timeUntilEffect = -1;
	
	public float timeCreated = 0;
	
	void Start () {
		timeCreated = Time.time;
		if(timeUntilEffect >= 0){
			Invoke("DamageEffect",timeUntilEffect);
		}
	}
	
	void Update(){
		AnimationScript.AnimateSprite(timeCreated,GetComponent<Renderer>(),8,1,0,0,8,6);
	}
	
	private void DamageEffect(){
		if(damage > 0){
			Collider[] temp = Physics.OverlapSphere(transform.position,transform.localScale.x,targets);
			foreach(Collider c in temp){
				HealthScript hs = c.GetComponent<HealthScript>();
				if(hs!=null){
					hs.HurtHealth(damage);
				}
			}
		}
	}
	
}
