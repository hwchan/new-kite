using UnityEngine;
using System.Collections;

public class ExplodingShot : BulletScript {
	
	private bool explosion = false;
	
	void LateUpdate(){
		if(!explosion){
			AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 1, 0, 0, 2, 8);
		}
	}
	
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, aoe);
	}
	
	/// <summary>
	/// Sets the parameters for the projectile.
	/// </summary>
	/// <param name='projectileTarget'>
	/// The position of the target.
	/// </param>
	/// <param name='primaryDamage'>
	/// Damage done to the primary target.
	/// </param>
	/// <param name='aoeDamage'>
	/// Damage done to secondary 'splashed' targets.
	/// </param>
	/// <param name='areaOfEffect'>
	/// The radius of the area of effect to apply damage and knockback.
	/// </param>
	/// <param name='knockbackStrength'>
	/// The strength of knockback.
	/// </param>
	/// <param name='projectileTargetGroup'>
	/// The target group to determine collision physics. ENEMIES, PLAYERS.
	/// </param>
	/// <param name='projectileSpeed'>
	/// The projectile speed.
	/// </param>
	/// <param name='projectileDistance'>
	/// The distance the projectile reaches before expiring.
	/// </param>
	public void SetProjectileParams(Vector3 projectileTarget, int primaryDamage, int aoeDamage, float areaOfEffect, 
									float knockbackStrength, int projectileTargetGroup, 
									float projectileSpeed, float projectileDistance){
		SetProjectileParams(projectileTarget, primaryDamage, projectileTargetGroup, projectileSpeed, projectileDistance);
		aoeDmg = aoeDamage;
		aoe = areaOfEffect;
		knockbackStr = knockbackStrength;
	}

	// NOTE: Damage done on collided target = damage + aoeDmg
	protected void KnockBack(int targetGroup, Vector3 pos, float radius, float magnitude, int damage){
		if(targetGroup == ENEMIES){
			Collider[] col = Physics.OverlapSphere(pos, radius, VariableScript.intEnemyLayerMask);
			foreach(Collider c in col){
				
				EnemyScript es = c.GetComponent<EnemyScript>();
				EnemyHealthScript ehs = c.GetComponent<EnemyHealthScript>();
				BuildingHealthScript bhs = c.GetComponent<BuildingHealthScript>();	// TODO: combine healthscripts
				if(es != null){
					es.SlowDown(1 - slowStr, slowDur);
					Vector3 knockback = (es.transform.position - pos).normalized * magnitude;
					es.SetExternalDirection(knockback, .15f);
					ehs.HurtHealth(damage, this);
				}
				else if(bhs != null){
					bhs.HurtHealth(damage, this);
				}
			}
		}
		else if(targetGroup == PLAYERS){
			Collider[] col = Physics.OverlapSphere(pos, radius, 1 << 9);
			foreach(Collider c in col){
				
				PlayerScript ps = c.GetComponent<PlayerScript>();
				PlayerHealthScript phs = c.GetComponent<PlayerHealthScript>();
				if(ps != null){
					ps.SlowDown(1 - slowStr, slowDur);
					Vector3 knockback = (ps.transform.position - pos).normalized * magnitude;
					ps.SetExternalDirection(knockback, .15f);
					phs.HurtHealth(damage);
				}
			}
		}
	}
	
	protected override void RemoveMe(){
		KnockBack(targetGroup, transform.position, aoe, knockbackStr, aoeDmg);
		GameObject boom = (GameObject) Instantiate(VariableScript.objBullet, transform.position, Quaternion.Euler(0,180,0));
		boom.transform.localScale = new Vector3(aoe * 3, 0, aoe * 3);
		BulletScript bs = boom.GetComponent<BulletScript>();
		bs.SetExpiry(.2f);
		boom.GetComponent<Collider>().enabled = false;
		AnimationScript.AnimateSprite(boom.GetComponent<Renderer>(), 4, 1, 2, 0, 1, 1);
		Destroy(gameObject);
	}
	
	// Initializes at instantiation time
	public virtual void Initialize(GameObject abilityPrefab, int chargeLevel){}
	
	// Sets up the projectile for firing
	public virtual void SetUp(Vector3 target, float charge, int damage){}
	
}
