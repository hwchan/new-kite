using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {
	
	private GameObject[][] projectilePrefabsArray;
	[SerializeField]
	private GameObject[] projectilePrefabs0;
	[SerializeField]
	private GameObject[] projectilePrefabs1;
	[SerializeField]
	private GameObject[] projectilePrefabs2;
	[SerializeField]
	private GameObject[] projectilePrefabs3;
	
	private PlayerScript ps;
	private PlayerAbilityManager pam;
	private PlayerStats pstat;
	
//	public int damage = 1;
//	public float attackRange = 3.25f;
	public GameObject target;
	
//	private float timePerCharge = .15f;
	[SerializeField]
	private float chargeTimeModifier = 0;
	
//	public float TimePerCharge{
//		get{return timePerCharge;}
//		set{timePerCharge = value;}
//	}
	public float ChargeTimeModifier{
		get{return chargeTimeModifier;}
		set{chargeTimeModifier = value;}
	}
	
	private float chargeTimer = 0;
	private int chargeLevel = 0;
	private GameObject objProjectile;
	private short abilityInputLock = -1;	// Stops multiple ability input from player
	[SerializeField]
	private AbilityScript castingAbility;
	
	private void Start () {
		ps = GetComponent<PlayerScript>();
		pam = GetComponent<PlayerAbilityManager>();
		pstat = GetComponent<PlayerStats>();
		projectilePrefabsArray = new GameObject[][]{ projectilePrefabs0, projectilePrefabs1, projectilePrefabs2, projectilePrefabs3 };
	}
	
	public void CopyProjectilePrefabs(GameObject[] newArray, int index){
		projectilePrefabsArray[index] = new GameObject[newArray.Length];
		for(int i=0; i<newArray.Length; i++){
			projectilePrefabsArray[index][i] = newArray[i];
		}
	}
	
	void Update () {	
		if(castingAbility != null && castingAbility.targetMethod == TargetMethod.AutoTarget){
			AutoTarget();
		}
		else{
			SetReticle(false);
			target = null;
		}
		HandleFire();
		HandleButtonUp();
	}
	
	// Maps the input to the correct projectile prefab array for charging up attack
	private void HandleFire(){
		for(int i=0; i<projectilePrefabsArray.Length; i++){
			if(Input.GetButton( "Ability" + (i+1) ) && (abilityInputLock == -1 || abilityInputLock == i) && pam.GetAbilityScript(i) != null){
				abilityInputLock = (short)i;	// Lock ability input
				castingAbility = pam.GetAbilityScript(i);
				ChargeTimeModifier = castingAbility.additionalChargeTime;
				HandleButtonDown(projectilePrefabsArray[i], i);
			}
		}
	}
	
	// Handles the charge logic
	private void HandleButtonDown(GameObject[] array, int index){
		if(Time.time >= chargeTimer){
			chargeTimer = Time.time + pstat.chargeTime + ChargeTimeModifier;
				
			// Charges up the selected ability
			if(chargeLevel < array.Length && array[chargeLevel] != null){
				Destroy(objProjectile);
				objProjectile = (GameObject) Instantiate(array[chargeLevel], 
						new Vector3(transform.position.x,.5f,transform.position.z + .759f), Quaternion.identity);
				objProjectile.transform.parent = transform;
				ExplodingShot exs = objProjectile.GetComponent<ExplodingShot>();
				if(exs != null){
					exs.Initialize(pam.GetAbilityObject(index), chargeLevel);
					exs.enabled = false;
				}
				chargeLevel++;
			}
		}
	}
	
	// Handles logic in firing projectile
	private void HandleButtonUp(){

		if( Input.GetButtonUp("Ability1") || Input.GetButtonUp("Ability2") || 
			Input.GetButtonUp("Ability3") || Input.GetButtonUp("Ability4") ){
			
			// Unlocks ability input
			abilityInputLock = (short)-1;
			
			// Sets up and fires an exploding shot
			if(objProjectile != null){
				
				// Set up projectile
				objProjectile.transform.parent = null;
				ExplodingShot exs = objProjectile.GetComponent<ExplodingShot>();
				objProjectile.transform.position = transform.position;
				
				// Set up projectile for launching
				BulletScript bs = objProjectile.GetComponent<BulletScript>();
				if(bs != null){
					bs.enabled = true;
				}
				objProjectile.GetComponent<Collider>().enabled = true;
				Physics.IgnoreCollision(objProjectile.GetComponent<Collider>(), GetComponent<Collider>());
				
				// Set up special properties of ability
				if(exs != null){
					exs.SetUp(GetTargetLocation(), chargeLevel, pstat.damage);
				}
				
				// Reset projectiles back to default
				objProjectile = null;
				ChargeTimeModifier = 0;
				
				// Make casting a new attack slower than charging an existing attack
				chargeTimer = Time.time + 2.5f*(pstat.chargeTime + ChargeTimeModifier);
				
			}
			chargeLevel = 0;
		}
	}
	
	//http://www.unifycommunity.com/wiki/index.php?title=LookAtMouse
	private Vector3 GetCursorPosition(){
		// Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position);
    
        // Generate a ray from the cursor position
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        // then find the point along that ray that meets that distance.  This will be the point
        // to look at.
		
        float hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast (ray, out hitdist)) 
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
			return targetPoint;
        }	
		return Vector3.zero;
	}
	
	
	// Sets the Vector3 location of the "target" if valid
	private Vector3 GetTargetLocation(){
		if(target != null){
			return target.transform.position;
		}
		else{ 
			if(ps.GetMovement() == Vector3.zero){
				return transform.position + ps.GetFacingVec()*pstat.attackRange;
			}
			return transform.position + ps.GetMovement()*pstat.attackRange;;
		}
	}
	
	
	// Sets "target" (closest valid enemy) and shows its reticle 
	private void AutoTarget(){
		
		SetReticle(false);
		
		// Find the closest valid enemy target (proximity, line of sight, enemy)
		GameObject[] targetArray = UtilityScript.GetObjectsInLOS(transform.position, pstat.attackRange, VariableScript.intHostileLayerMask, GetComponent<Collider>()).ToArray();
		float shortestDist = 9999f;
		target = null;
		
		for(int i=0; i<targetArray.Length; i++){
			// Find the closest one
			float dist = Vector3.Distance(targetArray[i].transform.position, transform.position);
			if(targetArray[i].tag == "Enemy" && dist < shortestDist){
				shortestDist = dist;
				target = targetArray[i].gameObject;
			}
		}
		SetReticle(true);
	}
	
	private void SetReticle(bool b){
		if(target != null){
			EnemyHealthScript ehs = target.GetComponent<EnemyHealthScript>();
			if(ehs != null){
				if(target != null){
					ehs.ShowReticle(b);
				}
			}
		}
	}
	
//	// If the player clicks on an enemy, set that as the target (if valid)
//	private void HandleTargetting(){
//		if(ps.shootTarget != null){
//			if(CheckTarget(ps.shootTarget)){
//				target = ps.shootTarget;
//			}
//			else{
//				AutoTarget();
//			}
//		}
//		else{
//			AutoTarget();
//		}
//	}
//	
//	// Finds all enemies in a radius of 'aggroradius' and targets the closest one
//	private void AutoTarget(){
//		
//		targetArray = UtilityScript.GetObjectsInLOS(transform.position, pstat.attackRange, 1 << 8 | 1 << 25, collider).ToArray();
//		shortestDist = 9999f;
//		target = null;
//		
//		for(int i=0; i<targetArray.Length; i++){
//			// Find the closest one
//			float dist = Vector3.Distance(targetArray[i].transform.position, transform.position);
//			if(targetArray[i].tag == "Enemy" && dist < shortestDist){
//				shortestDist = dist;
//				target = targetArray[i].gameObject;
//			}
//		}
//	}
//	
//	// Checks whether the GameObject is valid (enemy, proximity, LoS)
//	private bool CheckTarget(GameObject o){
//		if(o == null){
//			return false;
//		}
//		EnemyScript oes = o.GetComponent<EnemyScript>();
//		float distance = Vector3.Distance(o.transform.position, transform.position);
//		if(oes != null && distance <= pstat.attackRange){
//			// Check LoS now
//			if(!Physics.Raycast(transform.position, o.transform.position - transform.position, distance, 1 << 27)){
//				return true;
//			}
//		}
//		return false;
//	}
//	
//	// Checks if player is NOT moving and the attack cooldown is finished to start shooting
//	private void HandleShooting(){
//		firingTimer++;
//		if(ps.GetMovement() == Vector3.zero && target != null){ 
//			ps.HandleDirection(target.transform.position - transform.position, PlayerMovement.ATTACKING);
//			if(firingTimer >= firingCooldown/Time.deltaTime){	// controller.velocity.magnitude would sometimes flicker to 0 even when moving
//				HandleBullets();
//			}
//		}
//		else{
//			animationTimer = 0;
//		}
//	}
//	
//	private void HandleBullets(){
//		// Extra timer for animation
//		if(animationTimer >= animationLength/Time.deltaTime){
//			animationTimer = 0;
//			firingTimer = 0;
//			// Create and set bullet parameters 
//			GameObject objCreatedBullet = (GameObject) Instantiate(VariableScript.objHookshot, transform.position, Quaternion.identity);
//			Physics.IgnoreCollision(objCreatedBullet.collider, collider);
//			HookShotProjectileScript hkps = objCreatedBullet.GetComponent<HookShotProjectileScript>();
//			hkps.SetMoveVec(target.transform.position - transform.position);
//			hkps.SetDamage(damage);
//		}
//		animationTimer++;
//	}
	
	
}
