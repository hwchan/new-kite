using UnityEngine;
using System.Collections;

public class ChestScript : MonoBehaviour {
	
	public int expGiven = 25;
	public bool playerTrigger = true;
	public bool enemyTrigger = false;
	public bool neutralTrigger = false;
	
	void OnTriggerEnter(Collider col){
		if( playerTrigger && col.tag == "Player" ||
			enemyTrigger && col.tag == "Enemy" ||
			neutralTrigger && col.tag == "Neutral"){
			
			GUIManager.FloatingText("+" + expGiven + "XP", UtilityScript.MoveY(transform.position,2), 1.5f);
			PlayerStats pe = col.GetComponent<PlayerStats>();
			if(pe != null){
				pe.IncreaseExp(expGiven);
			}
			Destroy(gameObject);
		}
	}
}
