using UnityEngine;
using System.Collections;

public class Immolation: EnemyType {

	public float immolationRadius = .55f;
	
	private float timer = .5f;
	private float dmgInterval = .5f;
	
	// Use this for initialization
	void Start () {
		timer = dmgInterval;
	}
	
	// Update is called once per frame
	void Update () {
		timer++;
		ImmolationAura();
	}
	
	private void ImmolationAura(){
		Collider[] col = Physics.OverlapSphere(transform.position, immolationRadius, VariableScript.intPlayerUnitsLayerMask);

		if(timer > dmgInterval/Time.deltaTime){
			timer = 0;
			foreach(Collider o in col){
				PlayerHealthScript phs = o.GetComponent<PlayerHealthScript>();
				if(phs != null){
					phs.HurtHealth(1);
				}
			}
		}
	}
	
	public override void KillMe(){
		GameObject temp = (GameObject) Instantiate(VariableScript.objFire, transform.position, Quaternion.Euler(0,180,0));
		FireScript fs = temp.GetComponent<FireScript>();
		if(fs != null){
			fs.expiry = 5;
		}
	}
	
}
