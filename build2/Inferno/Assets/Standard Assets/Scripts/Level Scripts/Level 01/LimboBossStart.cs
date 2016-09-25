using UnityEngine;
using System.Collections;

public class LimboBossStart : MonoBehaviour {
	
	public GameObject[] objectsToLookFor;
	public GameObject boss;
	public GameObject door;
	private bool triggered = false;
	
	void FixedUpdate () {
		if(CheckBossRequirements() && CheckIfPlayerIsClose() && !triggered){
			triggered = true;
			StartCoroutine("StartBossFight");
		}
	}
	
	// True when every element is null OR is outside of bounds
	private bool CheckBossRequirements(){
		for(int i=0; i<objectsToLookFor.Length; i++){
			if(objectsToLookFor[i] != null && GetComponent<Collider>().bounds.Contains(objectsToLookFor[i].transform.position)){
				return false;
			}
		}
		return true;
	}

	private bool CheckIfPlayerIsClose(){
		if(UtilityScript.GetPlayersInRange(boss.transform.position, 8).Count > 0){
			return true;
		}
		return false;
	}
	
	private IEnumerator StartBossFight(){
		// Show boss
		boss.SetActive(true);
		// Disable boss's control script
		EnemyType et = boss.GetComponent<EnemyType>();
		if(et!=null){et.enabled = false;}
		// Close door
		door.GetComponent<DoorScript>().TriggerMe();
		// For each player, move camera and disable control script
		foreach(GameObject player in VariableScript.players){
			GameObject objCamera = GameObject.FindWithTag("MainCamera");
			PlayerScript ps = player.GetComponent<PlayerScript>();
			ps.playerCameraScript.LockCamera(false);
			ps.enabled = false;
			objCamera.transform.position = new Vector3 (boss.transform.position.x, objCamera.transform.position.y, boss.transform.position.z);
		}
		// Play opening animations etc.
		yield return new WaitForSeconds(2);
		// For each player, reset camera and re-enable control script
		foreach(GameObject player in VariableScript.players){
			PlayerScript ps = player.GetComponent<PlayerScript>();
			ps.playerCameraScript.LockCamera(true);
			ps.enabled = true;
		}
		// Enable boss's control script
		if(et!=null){et.enabled = true;}
		// Destroy this script
		Destroy(this);
	}
}
