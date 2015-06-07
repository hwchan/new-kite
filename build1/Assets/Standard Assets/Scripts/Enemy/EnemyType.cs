using UnityEngine;
using System.Collections;

public class EnemyType : MonoBehaviour {
	
	public int healthPoints = 15;
	
	public float moveSpeed = 1;
	
	public float attackRange = 1;
	public float attackRate = 3;
	
	public GameObject missileModel;	// TODO
	public float missileSpeed = 3f;
	public float missileRange = 5f;
	public int missileDamage = 1;
	
	public int experience = 1;
	public GameObject droppedItem;	// TODO
	
	protected float timerAtk = 0;
	protected Transform plane;
	protected Transform reticle;
	protected EnemyScript es;
	
	void Awake(){
		plane = transform.FindChild("Plane");
		reticle = transform.FindChild("Reticle");
		es = GetComponent<EnemyScript>();
		if(missileModel == null){
			missileModel = VariableScript.objBullet;
		}
	}
	
//	public float MissileSpeed{  
//		get{ return missileSpeed; } 
//		set{ missileSpeed = value; } 
//	}
//	public float MissileRange{  
//		get{ return missileRange; }
//		set{ 
//			missileRange = value; 
//			if(es != null){
//				es.attackRange = value;		 	
//			}
//		} 
//	}
//	public int MissileDamage{ 	
//		get{ return missileDamage; }
//		set{ missileDamage = value; } 
//	}
//	public string TargetTag{ 
//		get{ return targetTag; }
//		set{ targetTag = value; } 
//	}
//	
//	public virtual int Experience{
//		get{ return experience; }
//		set{ experience = value; }
//	}
	
	void LateUpdate(){
		timerAtk++;	
	}
	
	public virtual void Attack(){
		if(timerAtk > attackRate/Time.deltaTime){
			timerAtk = 0;
			GameObject objCreatedBullet = (GameObject) Instantiate(missileModel, transform.position, Quaternion.identity);
//			// Rotate the plane that holds the material
//			objCreatedBullet.transform.FindChild("Plane").localRotation = Quaternion.Euler(0,90,0);
			// Set bullet params
			BulletScript bs = objCreatedBullet.GetComponent<BulletScript>();
			PlayerScript ps = VariableScript.scrPlayerScript1;
			bs.SetProjectileParams(ps.GetVisiblePosition(), missileDamage, BulletScript.PLAYERS, missileSpeed, missileRange);
			Physics.IgnoreCollision(objCreatedBullet.GetComponent<Collider>(), GetComponent<Collider>());
		}
	}
	
	public virtual void OnDeath(){
		VariableScript.scrPlayerExperience1.IncreaseExp(experience);
		KillMe();
	}
	
	public virtual void KillMe(){
		
	}
}
