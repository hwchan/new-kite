using UnityEngine;
using System.Collections;

// TODO: Handle whether last damage source was from fire using observer pattern/delegates/events

public class LustBoss : MonoBehaviour {
	
	private bool TriggerToggle = false;
	public GameObject cinematicMob;
	public GameObject cinematicFire;
	public GameObject boss;
	public Object[] spawnPts;
	private int numSpawn = 0;
	private Vector3 firePos;
	public GameObject spawn;
	public GameObject door;
	public GameObject door2;
	public GameObject bossPos;
	public GameObject victoryPos;
	public GameObject vortex;
	private PlayerScript ps;
	private bool isPulling = false;
	private EnemyHealthScript ehs;
	private int currentHP;
	
	[SerializeField]
	private int fireCount = 0;
	private int fireMax = 5;
	private GameObject fireTarget;
	
	private float defaultAttackSpeed;
	
	private float tpCD = 1;
	private float tpTimer = 0;
	private bool canTP = true;
	public Material tpMat;	// TODO: the material should be stored somewhere else
	
	// Use this for initialization
	void Start () {
		ps = VariableScript.scrPlayerScript1;
		ehs = boss.GetComponent<EnemyHealthScript>();
		currentHP = ehs.curHp;
		defaultAttackSpeed = boss.GetComponent<Succubus>().attackRate;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(isPulling){
			AnimationScript.AnimateSprite(vortex.GetComponent<Renderer>(),4,1,2,0,2,8);
			ps.SetExternalForce((bossPos.transform.position - ps.transform.position).normalized * .55f);
		}
		if(ehs.curHp <= 0){
			Destroy(boss.gameObject);
			ps.SetExternalForce(Vector3.zero);
			StopAllCoroutines();
			ps.transform.position = victoryPos.transform.position;
			ps.GetComponent<Renderer>().material.color = new Color(1f,1,1,1);
			foreach(GameObject o in VariableScript.players){
				o.GetComponent<PlayerHealthScript>().respawnLocation = victoryPos;
			}
			DoorScript ds = door2.GetComponent<DoorScript>();
			if(ds!=null){ ds.TriggerMe(); }
			Destroy(gameObject);
		}
		// Check if boss hp is lowered
		else if(canTP && tpTimer > tpCD/Time.deltaTime && currentHP > ehs.curHp){
			// Compare the last damage source (script) is of type FireScript
			if(ehs.lastDamageSource != null){
				if(ehs.lastDamageSource.GetType().Equals(typeof(FireScript))){
					// Teleport away
					tpTimer = 0;
					TeleportAway();
				}
			}
		}
		currentHP = ehs.curHp;
		tpTimer++;
		
	}
	
	// Teleport away to 1 of 5 locations. Boss is paused for a short duration afterwards.
	private void TeleportAway(){
		GUIManager.FloatingTexture(tpMat,new Vector3(boss.transform.position.x, -.1f, boss.transform.position.z),Vector3.one,.45f);
		int i = Random.Range(0, spawnPts.Length);
		GameObject temp = (GameObject) spawnPts[i]; 
		boss.transform.position = temp.transform.position;
		boss.GetComponent<EnemyScript>().Pause(.75f);
	}
	
	void OnTriggerEnter(Collider col){
		if(!TriggerToggle && col.GetComponent<PlayerScript>() != null){	
			
			foreach(GameObject o in VariableScript.players){
				o.GetComponent<PlayerHealthScript>().respawnLocation = gameObject;
			}
			GameObject objCamera = GameObject.FindWithTag("MainCamera");
			ps.playerCameraScript.LockCamera(false);
			objCamera.transform.position = new Vector3 (bossPos.transform.position.x, objCamera.transform.position.y, bossPos.transform.position.z);
			ps.enabled = false;
			ps.GetComponent<PlayerShoot>().enabled = false;
			Invoke("LockCamera", 5);
			
			TryFireIndicator(cinematicMob);
			cinematicMob.GetComponent<EnemyHealthScript>().curHp = 1;
			Invoke("Cinematic0", 2f);
//			Invoke("Cinematic1", 3.5f);
			
			TriggerToggle = true;
			StartCoroutine("BossSequence");
			DoorScript ds = door.GetComponent<DoorScript>();
			if(ds!=null){ ds.TriggerMe(); }
		}
	}
	
	IEnumerator BossSequence(){	
		yield return new WaitForSeconds(16);
		TryFireIndicator(ps.gameObject);
		yield return new WaitForSeconds(12);
		TryFireIndicator(ps.gameObject);
		yield return new WaitForSeconds(7);
		TryTeleport();
		yield return new WaitForSeconds(7);
		TryFireIndicator(ps.gameObject);
		yield return new WaitForSeconds(12);
		TryPull();
		StartCoroutine("BossSequence");
	}
	
	private void LockCamera(){
		ps.playerCameraScript.LockCamera(true);
		EnemyScript es = boss.GetComponent<EnemyScript>();
		es.enabled = true;
		ps.enabled = true;
		ps.GetComponent<PlayerShoot>().enabled = true;
	}
	
	private void Cinematic0(){
		cinematicMob.GetComponent<EnemyScript>().enabled = true;
	}
	
//	private void Cinematic1(){
//		FireScript fs = cinematicFire.GetComponent<FireScript>();
//		MeshRenderer mr = cinematicFire.GetComponent<MeshRenderer>();
//		
//		fs.enabled = true;
//		mr.enabled = true;
//		fs.expiry = 2;
//	}
	
	private void TryTeleport(){
		canTP = false;
		numSpawn = 0;
		spawnPts = UtilityScript.RandomizeArray(spawnPts);
		InvokeRepeating("Teleport", 2.25f, 2.25f);
	}
	
	// Teleport away to each one of the 5 locations, firing off a salvo. Boss is paused for a short duration afterwards.
	private void Teleport(){
		EnemyType et = boss.GetComponent<EnemyType>();
		if(numSpawn < spawnPts.Length){
			GUIManager.FloatingTexture(tpMat,new Vector3(boss.transform.position.x, -.1f, boss.transform.position.z),Vector3.one,.45f);
			GameObject temp = (GameObject) spawnPts[numSpawn]; 
			boss.transform.position = temp.transform.position;
			et.missileRange = 10;
			et.attackRate = .35f;
			boss.GetComponent<EnemyScript>().Pause(.75f);
		}
		else{
			canTP = true;
			CancelInvoke("Teleport");
			et.missileRange = 3;
			et.attackRate = defaultAttackSpeed;
		}
		numSpawn++;
	}
	
//	private void TrySpawn(){
//		numSpawn = 0;
//		InvokeRepeating("SpawnMob", 1.5f, 1.5f);
//	}
//	
//	private void SpawnMob(){
//		if(numSpawn < spawnPts.Length){
//			GameObject temp = (GameObject) Instantiate(spawn, spawnPts[numSpawn].transform.position, Quaternion.identity);
//			EnemyScript es = temp.GetComponent<EnemyScript>();
//			es.Pause(3f);
//			es.searchRange = 10;
//		}
//		else{
//			CancelInvoke("SpawnMob");
//		}
//		numSpawn++;
//	}
	
	private void TryFireIndicator(GameObject target){
		fireTarget = target;
//		VariableScript.objPlayer1.renderer.material.color = new Color(1f,.15f,.15f,.35f);
		// TODO: debuff system here
		GameObject debuff = (GameObject) Instantiate(VariableScript.objTemp, 
				new Vector3(fireTarget.transform.position.x, .1f, fireTarget.transform.position.z), Quaternion.identity);
		debuff.transform.parent = fireTarget.transform;
		debuff.GetComponent<ExpiryScript>().SetExpiry(2.5f);
		
		fireCount = 0;
		InvokeRepeating ("TrySpawnFire", 2f, .2f);
	}
	
	private void TrySpawnFire(){
		fireCount++;
		if(fireCount <= fireMax){
//			VariableScript.objPlayer1.renderer.material.color = new Color(1f,1,1,1);
//			firePos = VariableScript.scrPlayerScript1.GetVisiblePosition();
//			firePos.y -= .1f;
			Invoke("SpawnFire", .5f);
		}
		else{
			CancelInvoke("TrySpawnFire");
		}
	}
	
	private void SpawnFire(){
		if(fireTarget != null){
			firePos = fireTarget.transform.position;
			firePos.y -= .1f;
			GameObject temp = (GameObject) Instantiate(VariableScript.objFire, firePos, Quaternion.Euler(0,180,0));
			FireScript fs = temp.GetComponent<FireScript>();
			fs.expiry = 15;
			temp.transform.localScale -= new Vector3(.25f, 0, .25f);
			temp.tag = "Enemy";
			if(fireTarget == cinematicMob){
				fs.expiry = 1.75f;
			}
		}
//		VariableScript.objPlayer1.renderer.material.color = new Color(1f,1,1,1);
	}
	
	private void TryPull(){
//		GUIManager.FloatingText("*teleport*", boss.transform.position, 1);
		GUIManager.FloatingTexture(tpMat,new Vector3(boss.transform.position.x, -.1f, boss.transform.position.z),Vector3.one,.45f);
		canTP = false;
		boss.transform.position = bossPos.transform.position;
		EnemyScript es = boss.GetComponent<EnemyScript>();
		es.enabled = false;
		Invoke ("StartPull", 1f);
	}
	
	private void StartPull(){
		vortex.GetComponent<Renderer>().enabled = true;
		Invoke ("Pull", 2f);
		isPulling = true;
	}
	
	private void Pull(){
		// Create a ring of 6 fires around the boss
		for(int i=0; i<6; i++){
			Quaternion quat = Quaternion.AngleAxis(360/6 * i, Vector3.up);
			Vector3 offset = quat * Vector3.forward * 1.22f;	
			Vector3 temp = bossPos.transform.position; 
			temp.y -= .1f;
			GameObject fire = (GameObject) Instantiate(VariableScript.objFire, temp + offset, Quaternion.Euler(0,180,0));
			FireScript fs = fire.GetComponent<FireScript>();
			fs.expiry = 6f;
		}
		Invoke("CancelPull", 6);
	}
	
	private void CancelPull(){
		// Fire off a ring of projectiles
		for(int i=0; i<16; i++){
			Quaternion quat = Quaternion.AngleAxis(360/16 * i, Vector3.up);
			Vector3 offset = quat * Vector3.forward * 1.1f;	
			Vector3 temp = bossPos.transform.position; 
			GameObject projectile = (GameObject) Instantiate(VariableScript.objBullet, temp + offset, quat);
			BulletScript bs = projectile.GetComponent<BulletScript>();
			bs.SetProjectileParams(quat * Vector3.forward, 0, BulletScript.PLAYERS, 3, 20);
			bs.AddSlowEffect(.75f, 5);
		}
		// Revert back values and settings
		canTP = true;
		isPulling = false;
		ps.SetExternalForce(Vector3.zero);
		EnemyScript es = boss.GetComponent<EnemyScript>();
		es.enabled = true;
		vortex.GetComponent<Renderer>().enabled = false;
	}
	
}
