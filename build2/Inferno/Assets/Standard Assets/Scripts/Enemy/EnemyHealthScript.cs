using UnityEngine;
using System.Collections;

public class EnemyHealthScript : HealthScript {
	
	public int curHp = 0;
	public int maxHp = 2;
	public float callForHelpRange = 2.75f;
//	public float KODur = 2f;
//	public float healRate = 5f;	// How long until it regenerates 1 hp
	public EnemyScript es;
//	public bool isKO = false;
	private bool hurt = false;
	private Transform planeRendererObj;
	public MonoBehaviour lastDamageSource;
	
	private float invuln = 0;		// current duration of invulnerability
	private float invuln_len = .1f;	// length of on-hit invulnerability
	
	// Use this for initialization
	void Start () {
		planeRendererObj = transform.Find("Plane");
		EnemyType et = GetComponent<EnemyType>();
		if(et!=null){maxHp = et.healthPoints;}
		curHp = maxHp;
		//es = GetComponent<EnemyScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hurt){
			if ((int)((Time.timeSinceLevelLoad - (int)Time.timeSinceLevelLoad) * 8) % 2 == 0) {
				planeRendererObj.GetComponent<Renderer>().material.color = new Color(1f,1f,1f,.35f);
			} else {
				planeRendererObj.GetComponent<Renderer>().material.color = Color.white;
			}
		}
	}
	
	private void HurtFlash(float duration){
		hurt = true;
		CancelInvoke("HurtFlashOff");
		Invoke ("HurtFlashOff", duration);
	}
	
	private void HurtFlashOff(){
		hurt = false;
		planeRendererObj.GetComponent<Renderer>().material.color = Color.white;
	}

	// TODO: Rework the logic here
	public override void HurtHealth(int amount){
		if(curHp <= 0){
			return;
		}

		if(amount > 0 && Time.time > invuln){
            GUIManager.FloatingText(amount.ToString(), transform.position, .75f);
            curHp -= amount;
			invuln = invuln_len + Time.time;
		}
		es.CallForHelp(callForHelpRange);
		if( curHp <= 0 ){
			HandleDeath();
		}else{
			HurtFlash(.5f);
		}
		
	}
	
	public void HurtHealth(int amount, MonoBehaviour callingScript){
		HurtHealth(amount);
		lastDamageSource = callingScript;
	}
	
	public void HurtHealth(int amount, float pauseLen, MonoBehaviour callingScript){
		HurtHealth(amount, callingScript);
		es.Pause(pauseLen);
	}
	
	public override void HealHealth(int amount){
		if(curHp + amount <= maxHp){
			curHp += amount;
		}
	}
	
	public override void SetHealth(int amount){
		if(amount >= maxHp){
			curHp = maxHp;
		}
		else{
			curHp = amount;
		}
		if( curHp <= 0 ){
			curHp = 0;
		}
	}
	
	public void HealHealth(){
		if(curHp + 1 <= maxHp){
			curHp++;
		}
		else{
			CancelInvoke("HealHealth");
		}
	}
	
	public void ShowReticle(bool b){
		Transform reticle = transform.Find("Reticle");
		
		float x = (float) curHp/maxHp;
		
		int y = 0;
		if(x < .25f){
			y = 3;
		}
		else if(x < .5f){
			y = 2;
		}
		else if(x < .75f){
			y = 1;
		}
		else{
			y = 0;
		}
		if(reticle != null){
			AnimationScript.AnimateSprite(reticle.GetComponent<Renderer>(),4,1,y,0,1,1);
			reticle.GetComponent<Renderer>().enabled = b;
		}
	}
	
	protected override void HandleDeath(){
        es.GetComponent<Collider>().enabled = false;
		es.RemoveMe();
		MonoBehaviour[] temp = GetComponents<MonoBehaviour>();
		foreach(MonoBehaviour x in temp){
			x.enabled = false;
		}
		EnemyType et = GetComponent<EnemyType>();
		if(et != null){
			et.OnDeath();
		}
		gameObject.layer = LayerMask.NameToLayer("Neutral");
		gameObject.AddComponent<ColourTransitionScript>().SetUp(Color.clear, .5f);
		Destroy(gameObject, .5f);
		EnemyList.Kill(gameObject);
		
	}
//	private void HandleKO(){
//		CancelInvoke("ToggleFalseKO");
//		CancelInvoke("HealHealth");
////		isKO = true;
////		es.KOMe();
//		EnemyType[] temp = GetComponents<EnemyType>();
//		foreach(EnemyType x in temp){
//			x.StopMe();
//			x.enabled = false;
//		}
//		//es.Pause(KODur);
//		//InvokeRepeating("HealHealth", KODur, healRate);
//		//Invoke("ToggleFalseKO", KODur);
//	}
	
//	private void ToggleFalseKO(){
////		isKO = false;
//	}
	
	
}
