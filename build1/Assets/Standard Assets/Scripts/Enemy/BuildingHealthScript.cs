using UnityEngine;
using System.Collections;

public class BuildingHealthScript : HealthScript {
	
	public int curHp = 0;
	public int maxHp = 2;
	private bool hurt = false;
	public MonoBehaviour lastDamageSource;
	private Transform planeRendererObj;
	
	// Use this for initialization
	void Start () {
		curHp = maxHp;
		planeRendererObj = transform.Find("Plane");
		if(planeRendererObj == null){
			planeRendererObj = transform;
		}
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
	
	public override void HurtHealth(int amount){
		curHp -= amount;
		if( curHp <= 0 ){
			curHp = 0;
			HandleDeath();
		}
		HurtFlash(.5f);
	}
	
	public void HurtHealth(int amount, MonoBehaviour callingScript){
		HurtHealth(amount);
		lastDamageSource = callingScript;
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
			HandleDeath();
		}
	}
	
	protected override void HandleDeath(){
		MonoBehaviour[] temp = GetComponents<MonoBehaviour>();
		foreach(MonoBehaviour x in temp){
			x.enabled = false;
		}
		gameObject.AddComponent<ColourTransitionScript>().SetUp(Color.clear, .5f);
		Destroy(gameObject, .5f);
	}
	
}

