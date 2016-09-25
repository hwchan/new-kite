using UnityEngine;
using System.Collections;

public class PlayerHealthScript : HealthScript {
	
	public int health = 1;
	public int MAX_HP = 1;
	public int lives = 3;
	private int invuln = 0;		// current duration of invulnerability
	private int invuln_len = 1;	// length of on-hit invulnerability
	public int charges = 0;	// number of PUShieldy invulnerability charges
	public GameObject respawnLocation;
	
	private bool isInDeathAnimation = false;
	
	// Use this for initialization
	void Start () {		
//		checkUpgrades();
		health = MAX_HP;
	}
	
	// Update is called once per frame
	void Update () {
		if (invuln > 0) {
			// Pippet flashes
			if ((int)((Time.timeSinceLevelLoad - (int)Time.timeSinceLevelLoad) * 8) % 2 == 0) {
				GetComponent<Renderer>().material.color = Color.clear;
			} else {
				GetComponent<Renderer>().material.color = Color.white;
			}
		}
	}
	
//	private void checkUpgrades() {
//		//int healthPerUpgrade = 1;
//		// separate but stackable (any way to loop from health1 to health6?)
//		if (SaveFile.isPurchased(UPGRADE.HEALTH1)) {
//			MAX_HP++;
//		}
//		if (SaveFile.isPurchased(UPGRADE.HEALTH2)) {
//			MAX_HP++;
//		}
//		if (SaveFile.isPurchased(UPGRADE.HEALTH3)) {
//			MAX_HP++;
//		}
//		if (SaveFile.isPurchased(UPGRADE.HEALTH4)) {
//			MAX_HP++;
//		}
//		if (SaveFile.isPurchased(UPGRADE.HEALTH5)) {
//			MAX_HP++;
//		}
//		if (SaveFile.isPurchased(UPGRADE.HEALTH6)) {
//			MAX_HP++;
//		}
//	}
	
	public override void HurtHealth(int amount){
		if( amount > 0 && invuln <= 0 && !isInDeathAnimation){
			if(!CheckChargesLeft()){
				health -= amount;
			}
			if( health <= 0 ){
				HandleDeath();
			}
			else{
				invuln = invuln_len;
				InvokeRepeating("HandleInvuln", .5f, .5f);
			}
		}
	}
	
	// Check if there any invulnerability charges left. If there are none left, remove the picked up enemy and return false, otherwise return true
	private bool CheckChargesLeft(){
		if(charges > 0){
			charges--;
			if(charges <= 0){
				for(int i=0; i<transform.childCount; i++){
					EnemyScript es = transform.GetChild(i).GetComponent<EnemyScript>();
					if(es != null){
						es.RemoveMe();
					}
				}
			}
			return true;
		}
		else
			return false;
	}
	
	public bool IsInvuln(){
		if(invuln > 0)
			return true;
		else
			return false;
	}
	
	public void HandleInvuln(){
		invuln--;
		if( invuln <= 0 ){
			CancelInvoke("HandleInvuln");
			GetComponent<Renderer>().material.color = Color.white;
		}
	}
	
	public override void HealHealth(int amount){
		if ((health+amount) <= MAX_HP){
			health += amount;
		}
		else if(health+amount > MAX_HP){
			health = MAX_HP;
		}
	}
	
	public override void SetHealth(int amount){
		if(amount >= MAX_HP){
			health = MAX_HP;
		}
		else{
			health = amount;
		}
		if( health <= 0 ){
			health = 0;
		}
	}
	
	public void SetMaxHealth(int amount){
		int loss = MAX_HP - health;
		if(amount > MAX_HP){
			health = amount - loss;
		}else{
			SetHealth(amount - loss);
		}
		MAX_HP = amount;
	}
	
	protected override void HandleDeath(){
//		SaveFile.setLastLevel(Application.loadedLevelName);
//		SaveFile.saveGameOverData(ps.getLevelEndData(false));
		StartCoroutine("DeathAnimation");
	}
	
	IEnumerator DeathAnimation(){
		isInDeathAnimation = true;
//		PlayerScript ps = GetComponent<PlayerScript>();
//		ps.enabled = false;
		foreach(MonoBehaviour mb in GetComponents<MonoBehaviour>()){
			mb.enabled = false;
		}
		if(lives < 1){
			GUIManager.SetLabel(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, 20),"No more lives!",3);
			gameObject.AddComponent<ColourTransitionScript>().SetUp(Color.clear,2);
			yield return new WaitForSeconds(3);
			GUIManager.SetLabel(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, 20),"GAME OVER!",60);
			yield return new WaitForSeconds(3);
//			Application.LoadLevel (0);
		}else{
			GUIManager.SetLabel(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, 20),"LIFE - 1",3);
			gameObject.AddComponent<ColourTransitionScript>().SetUp(Color.clear,2);
			yield return new WaitForSeconds(3);
			transform.position = respawnLocation.transform.position;
			lives--;
			health = MAX_HP;
			invuln = 3;
			InvokeRepeating ("HandleInvuln", 1, 1);
//			ps.enabled = true;
			foreach(MonoBehaviour mb in GetComponents<MonoBehaviour>()){
				mb.enabled = true;
			}
			isInDeathAnimation = false;
			GUIManager.FloatingText("-LIFE",new Vector3(transform.position.x,2,transform.position.z+.5f),2).transform.parent = transform;
		}
	}
	
}
