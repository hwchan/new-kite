using UnityEngine;
using System.Collections;

public class LimboBoss : EnemyType {
	
	public float attackMissileSpeed = 1.25f;
	public GameObject spikeTarget;
	public GameObject[] summons;
	public Vector3[] summonLocations;
	public GameObject levelEnd;
	
	void Start () {
		missileSpeed = attackMissileSpeed;
		StartCoroutine("BossSequence");
	}
	
	void FixedUpdate () {
		if(plane != null){
			AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,3,0,2,2,2);
		}
	}

	public override void KillMe(){
		levelEnd.GetComponent<HintScript>().TriggerMe();

	}

	IEnumerator BossSequence(){
		CancelInvoke();
		InvokeRepeating("ShootCrossAttack", 1, 2f);
		yield return new WaitForSeconds(12);
		CancelInvoke("ShootCrossAttack");
		InvokeRepeating("StartSpikeTarget", 2, 2);
		yield return new WaitForSeconds(12);
		CancelInvoke("StartSpikeTarget");
		InvokeRepeating("ShootCrossAttack", 1, 2.5f);
		InvokeRepeating("StartSummons", 3, 45);
		yield return new WaitForSeconds(12);
		InvokeRepeating("StartSpikeTarget", 2, 2);
//		yield return new WaitForSeconds(25f);
//		StartCoroutine("BossSequence");
	}
	
	private void StartSpikeTarget(){
		GameObject spikedPlayer = UtilityScript.GetRandomPlayerInRange(transform.position, 100);
		PlayerScript ps = spikedPlayer.GetComponent<PlayerScript>();
		// Randomly choose whether to spike in front, on top, or around the player if he's moving
		Vector3 targetPos = spikedPlayer.transform.position;
		int x = 0;
		if(ps.GetMovement() == Vector3.zero){
			x = 0;
		}else{
			x = Random.Range(0,3);
		}
		switch(x){
			case 0:
				break;
			case 1:
				targetPos = spikedPlayer.transform.position + ps.GetMovement()*2.5f;
				break;
			case 2:
				Vector3 offset = Random.insideUnitSphere*2.5f;
				targetPos = targetPos+offset;
				break;
		}
		Instantiate(spikeTarget, targetPos, Quaternion.Euler(0,180,0));
	}
	
	private void StartSummons(){
		for(int i=0; i<summonLocations.Length; i++){
			int x = Random.Range(0,summons.Length);
			GameObject obj = (GameObject) Instantiate(summons[x],summonLocations[i],Quaternion.Euler(0,180,0));
			obj.AddComponent<ColourTransitionScript>().SetUp(Color.clear, Color.white, 2);
			EnemyScript es = obj.GetComponent<EnemyScript>();
			if(es!=null){
				es.Pause(3);
				es.passiveAI = es.aggroAI;
				es.currentAI = es.aggroAI;
			}
			// Add fancy effect
			GameObject skullEffect = (GameObject) GUIManager.FloatingTexture(VariableScript.matSkull, UtilityScript.MoveY(obj.transform.position,2), Vector3.one, 1.5f);
			skullEffect.AddComponent<TransformTransitionScript>().SetUp(Vector3.one, Vector3.one*2, 1.5f);
			skullEffect.AddComponent<ColourTransitionScript>().SetUp(new Color(.35f,.35f,.35f,1), Color.clear, 1.5f);
		}
	}
	
	private void ShootCrossAttack(){
		GameObject objBullet = (GameObject) Instantiate(VariableScript.objExplodingShot, transform.position, Quaternion.identity);
		Destroy(objBullet.GetComponent<ExplodingShot>());
		Vector3 targetPlayerPosition = UtilityScript.GetRandomPlayerInRange(transform.position, 200).transform.position;
		objBullet.AddComponent<MinosExplodingShot>().SetProjectileParams(targetPlayerPosition,
			0,1,.5f,1,BulletScript.PLAYERS,1.5f,Vector3.Distance(transform.position,targetPlayerPosition));
		
	}
	
}
