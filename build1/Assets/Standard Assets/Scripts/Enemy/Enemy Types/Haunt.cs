using UnityEngine;
using System.Collections;

public class Haunt : EnemyType {
	
	public bool corporeal = true;
	
	private Color transColour;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(plane != null){
			if(corporeal){
				AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,0,2,2);
			}
			else{
				AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,1,2,2);
			}
		}
		// Handle texture flip depending on facing
		if( plane != null && (plane.localScale.x > 0 && es.GetFacingVector().x < 0) || (plane.localScale.x < 0 && es.GetFacingVector().x > 0) ){	
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
		}
		HandleAlpha();
	}
	
	// The farther away the player is from the Haunt, the lower the alpha, and vice versa
	private void HandleAlpha(){
		GameObject player = UtilityScript.GetClosestPlayerInRange(transform.position, es.aggroSearchRange);
		if(player != null){
			float distance = Vector3.Distance(player.transform.position, transform.position);
			// A bit of math to determine alpha (minimum .15f)
			float alpha = (1 - (distance/2.85f)) * 2f;
			if(alpha < .15f){
				alpha = .15f;
			}
			transColour = new Color(1,1,1,alpha);
			plane.GetComponent<Renderer>().material.color = transColour;
			transColour = new Color(1,1,1,alpha*.15f);
			reticle.GetComponent<Renderer>().GetComponent<Renderer>().material.color = transColour;
		}
	}
}
