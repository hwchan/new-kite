using UnityEngine;
using System.Collections;

public class Leopard : EnemyType {
	
	private Color translucent;
	private Vector3 pounceVec;
	public bool isPouncing = false;
	
	void Start(){
		
		translucent = new Color(1,1,1,.25f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Handle texture flip depending on facing
		if( plane != null && (plane.localScale.x > 0 && es.GetFacingVector().x < 0) || (plane.localScale.x < 0 && es.GetFacingVector().x > 0) ){	
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
		}
		if(es.currentAI == es.aggroAI && !isPouncing){
			StartCoroutine("Pounce");
		}
	}
	
	private IEnumerator Pounce(){
		isPouncing = true;
		yield return new WaitForSeconds(5);
		gameObject.AddComponent<ColourTransitionScript>().SetUp(translucent, 1.25f);
		pounceVec = es.GetFacingVector().normalized;
		es.Pause(1.5f);
		yield return new WaitForSeconds(1.5f);
		plane.GetComponent<Renderer>().material.color = Color.white;
		es.SetExternalDirection(pounceVec, .5f);
		if(es.currentAI == es.aggroAI){
			StartCoroutine("Pounce");
		}else{
			isPouncing = false;
		}
	}
}
