using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour {
	
	public GameObject[] triggeredObjects;
	public bool DestroyWhenTriggered = true;
	public bool TriggerOnPlayer = true;
	public bool TriggerOnEnemy = false;
	public bool TriggerOnNeutral = false;
	bool triggered = false;
	
	void Update () {
		if (triggered) AnimationScript.AnimateSprite(GetComponent<Renderer>(),2,1,1,0,1,1);
		else AnimationScript.AnimateSprite(GetComponent<Renderer>(),2,1,0,0,1,1);
	}
	
	void OnTriggerEnter(Collider other){
		if(!triggered){
			switch (other.tag){
				
			case "Player":
				if(TriggerOnPlayer){
					HandleTriggering();
					triggered = true;
				}
				break;
			case "Enemy":
				if(TriggerOnEnemy){
					HandleTriggering();
					triggered = true;
				}
				break;
			case "Neutral":
				if(TriggerOnNeutral){
					HandleTriggering();
					triggered = true;
				}
				break;
				
			}
		}
	}
	
	void HandleTriggering(){
		foreach(GameObject i in triggeredObjects){
			DoorScript ds = i.GetComponent<DoorScript>();
			if(ds != null){
				ds.TriggerMe();
			}
			else{
				i.SetActive(true);
			}
		}
	}
	
	void OnTriggerExit(Collider other){
		if(!DestroyWhenTriggered){
			triggered = false;
		}
	}
	
	void RemoveMe(){
		Destroy(gameObject);
	}
}
