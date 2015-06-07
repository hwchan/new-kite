using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {
	
	public int killEverythingInLayer = 24; // Neutral layer
	public bool removeByDeath = true;
	public bool removeByFade = false;
	public bool removeInstantly = false;
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.layer == killEverythingInLayer){
			if(removeByDeath){
				HealthScript hs = col.GetComponent<HealthScript>();
				if(hs != null){
					hs.HurtHealth(int.MaxValue);
				}
			}
			else if(removeByFade){
				// Add stuff here
			}
			else if(removeInstantly){
				// Add stuff here
			}
		}
	}
}
