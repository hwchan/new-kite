using UnityEngine;
using System.Collections;

public class LifeUpScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
		PlayerHealthScript phs = col.GetComponent<PlayerHealthScript>();
		if(phs != null){
			phs.HealHealth(phs.MAX_HP);
			phs.lives++;
			Destroy(gameObject);
		}
	}
}
