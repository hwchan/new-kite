using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	protected float moveSpeed = 3f;
	protected float timeSpentAlive;
	protected float timeUntilDeath = 0;
	[SerializeField]
	protected Vector3 moveVec;
	public int damage = 1;
	
	protected int aoeDmg = 0;
	protected float aoe = 0;
	protected float knockbackStr = 0;

	public Transform plane;

	// Additional effects
	protected float slowStr = 0;
	protected float slowDur = 0;
	
	protected int targetGroup = 0;
	public const int ENEMIES = -1, PLAYERS = -2;

	void Awake () {
		plane = transform.FindChild("Plane");
	}

	// Update is called once per frame
	void Update () {
		timeSpentAlive += Time.deltaTime;
		if(timeSpentAlive > timeUntilDeath){
			RemoveMe();
		}
		moveVec.y = 0;
		transform.Translate(moveVec.normalized * moveSpeed * Time.deltaTime);
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		
	}
	
	public void SetExpiry(float expiry){
		timeUntilDeath = expiry;
	}
	
	/// <summary>
	/// Sets the parameters for the projectile.
	/// </summary>
	/// <param name='projectileTarget'>
	/// The position of the target.
	/// </param>
	/// <param name='projectileDamage'>
	/// Damage done to the primary target.
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
	public virtual void SetProjectileParams(Vector3 projectileTarget, int projectileDamage, int projectileTargetGroup, 
											float projectileSpeed, float projectileDistance){
		moveVec = projectileTarget - transform.position;
		damage = projectileDamage;
		moveSpeed = projectileSpeed;
		timeUntilDeath = projectileDistance/projectileSpeed;
		targetGroup = projectileTargetGroup;
		if(projectileTargetGroup == ENEMIES){
			gameObject.layer = LayerMask.NameToLayer("Projectile (Player)");	// Projectile (Player): only hits enemies
		}
		else if(projectileTargetGroup == PLAYERS){
			gameObject.layer = LayerMask.NameToLayer("Projectile (Enemy)");	// Projectile: only hits players
		}
//		print(moveVec);
		// Rotates the plane where the material is on
		Quaternion bulletRotate = Quaternion.LookRotation(moveVec,Vector3.up);
		bulletRotate.eulerAngles = new Vector3(bulletRotate.eulerAngles.x,bulletRotate.eulerAngles.y+45,bulletRotate.eulerAngles.z);
		if(plane!=null){
			plane.localRotation = bulletRotate;
		}
	}
	
	public void SetTarget(Vector3 projectileTarget){
		moveVec = projectileTarget - transform.position;
	}
	
	public void AddSlowEffect(float slowStrength, float slowDuration){
		slowDur = slowDuration;
		slowStr = slowStrength;
	}
	
	protected virtual void RemoveMe(){
		Destroy(gameObject);
	}

	protected virtual void RemoveMe(PlayerScript ps){
		Destroy(gameObject);
	}
	
	void OnCollisionEnter(Collision other){
		PlayerHealthScript playerHealth = other.gameObject.GetComponent<PlayerHealthScript>();
		PlayerScript ps = other.gameObject.GetComponent<PlayerScript>();
		EnemyHealthScript enemyHealth = other.gameObject.GetComponent<EnemyHealthScript>();
		EnemyScript es = other.gameObject.GetComponent<EnemyScript>();
		if(playerHealth != null && ps != null){
			playerHealth.HurtHealth(damage);
			ps.SlowDown(1 - slowStr, slowDur);
		}
		else if(enemyHealth != null && es != null){
			enemyHealth.HurtHealth(damage, .5f, this);
			es.SlowDown(1 - slowStr, slowDur);
		}
		RemoveMe(ps);
	}
	
//	// TODO: Handle damage on buildings
//	// NOTE: Damage done on collided target = damage + aoeDmg
//	private void KnockBack(int targetGroup, Vector3 pos, float radius, float magnitude, int damage){
//		if(targetGroup == ENEMIES){
//			Collider[] col = Physics.OverlapSphere(pos, radius, VariableScript.intEnemyLayerMask);
//			foreach(Collider c in col){
//				
//				EnemyScript es = c.GetComponent<EnemyScript>();
//				EnemyHealthScript ehs = c.GetComponent<EnemyHealthScript>();
//				if(es != null){
//					Vector3 knockback = (es.transform.position - pos).normalized * magnitude;
//					es.SetExternalDirection(knockback, .15f);
//					ehs.HurtHealth(damage, this);
//				}
//			}
//		}
//		else if(targetGroup == PLAYERS){
//			Collider[] col = Physics.OverlapSphere(pos, radius, 1 << 9);
//			foreach(Collider c in col){
//				
//				PlayerScript ps = c.GetComponent<PlayerScript>();
//				PlayerHealthScript phs = c.GetComponent<PlayerHealthScript>();
//				if(ps != null){
//					Vector3 knockback = (ps.transform.position - pos).normalized * magnitude;
//					ps.SetExternalDirection(knockback, .15f);
//					phs.HurtHealth(damage);
//				}
//			}
//		}
//	}
}
