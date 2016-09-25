using UnityEngine;
using System.Collections;

public class Succubus : EnemyType {
	
	public float aoeRadius = .35f;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public override void Attack(){
		if(timerAtk > attackRate/Time.deltaTime){
			timerAtk = 0;
			GameObject objCreatedBullet = (GameObject) Instantiate(VariableScript.objExplodingShot, transform.position, Quaternion.identity);
			ExplodingShot exs = objCreatedBullet.GetComponent<ExplodingShot>();
			PlayerScript ps = VariableScript.scrPlayerScript1;
			exs.SetProjectileParams(ps.GetVisiblePosition(), 0, missileDamage, aoeRadius, 0, ExplodingShot.PLAYERS, missileSpeed, Vector3.Distance(ps.GetVisiblePosition(), transform.position));
			exs.AddSlowEffect(.25f, 1);
			Physics.IgnoreCollision(objCreatedBullet.GetComponent<Collider>(), GetComponent<Collider>());
			objCreatedBullet.layer = 28;
		}
	}
	
}
