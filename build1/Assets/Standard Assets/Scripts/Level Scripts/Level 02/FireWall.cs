using UnityEngine;
using System.Collections;

public class FireWall : MonoBehaviour {
	
	public GameObject door;
	public GameObject[] targets;
	public GameObject[] fire;
	private bool triggerToggle = false;
	private DoorScript ds;
	
	// Use this for initialization
	void Start () {
		ds = door.GetComponent<DoorScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(triggerToggle){
			CheckIfEmpty(targets);
		}
	}
	
	void OnTriggerEnter(Collider col){
		if(!triggerToggle && col.GetComponent<PlayerScript>() != null){
			foreach(GameObject o in VariableScript.players){
				o.GetComponent<PlayerHealthScript>().respawnLocation = gameObject;
			}
			triggerToggle = true;
			if(ds!=null){ ds.TriggerMe(); }
			foreach(GameObject i in targets){
				if(i!=null){
					EnemyScript es = i.GetComponent<EnemyScript>();
					if(es!=null){
						es.searchRange = 6.5f;
					}
				}
			}
		}
	}
	
	private void CheckIfEmpty(GameObject[] arr){
		int count = 0;
		foreach(GameObject i in arr){
			if(i == null){
				count++;
			}
		}
		if(count >= arr.Length){
			foreach(GameObject i in fire){
				Destroy(i);
			}
			if(ds!=null){ ds.TriggerMe(); }
			Destroy (gameObject);
		}
	}
}
