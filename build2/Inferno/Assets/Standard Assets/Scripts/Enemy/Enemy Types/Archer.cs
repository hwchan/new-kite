using UnityEngine;
using System.Collections;

public class Archer: EnemyType {
	
	private float baseRange;
	private float baseMs;
	private float baseSearch;
	public bool hawkStance = false;
	// Use this for initialization
	void Start () {
		baseRange = missileRange;
		baseMs = es.MoveSpeed;
		baseSearch = es.searchRange;
		CheckStance();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Handle texture flip depending on facing
		if( plane != null && (plane.localScale.x > 0 && es.GetFacingVector().x < 0) || (plane.localScale.x < 0 && es.GetFacingVector().x > 0) ){	
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
		}

		// Handle sprite animation
		if(plane != null && es.GetMovementVector() != Vector3.zero){
			if(hawkStance){
				AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,1,2,2);
			}
			else{
				AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,0,2,2);
			}
		}
	}
	
	private void CheckStance(){
		if(hawkStance){
			AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,1,1,2);
			es.MoveSpeed = es.MoveSpeed *.25f;
			missileRange = missileRange*2;
			es.SetSearchRange(missileRange);
		}
		else{
			AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,0,1,2);
			es.MoveSpeed = baseMs;
			missileRange = baseRange;
			es.SetSearchRange(baseSearch);
		}
	}
	
}
