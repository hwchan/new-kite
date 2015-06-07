using UnityEngine;
using System.Collections;

public class RespawnTrigger : MonoBehaviour {
	
	public GameObject newRespawnLocation;
	
	void OnTriggerEnter(Collider col){
		PlayerHealthScript phs = col.GetComponent<PlayerHealthScript>();
		if(phs != null){
			phs.respawnLocation = newRespawnLocation;
		}
	}
}
