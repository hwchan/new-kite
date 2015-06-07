using UnityEngine;
using System.Collections;

public class Guard : EnemyType {

	private EnemyHealthScript ehs;
	private int baseHp;
	private float baseMs;
	public bool defensiveStance = false;
	// Use this for initialization
	void Start () {	
		ehs = GetComponent<EnemyHealthScript>();
		baseMs = es.MoveSpeed;
		baseHp = ehs.maxHp;
		CheckStance();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Handle texture flip depending on facing
		if(plane != null && plane.localScale.x > 0 && es.GetFacingVector().x < 0){	// Face LEFT
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
			plane.localPosition = new Vector3(.09f,0,plane.localPosition.z);
		}
		else if(plane != null && plane.localScale.x < 0 && es.GetFacingVector().x > 0){	// Face RIGHT
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
			plane.localPosition = new Vector3(-.135f,0,plane.localPosition.z);
		}
		// Handle Defensive Stance toggle
		if(!defensiveStance && ehs != null){
			if( ehs.curHp < ehs.maxHp*.5f){
				defensiveStance = true;
				CheckStance();
				GUIManager.FloatingText("+DEF",new Vector3(transform.position.x,2,transform.position.z+.5f),1.5f).transform.parent = transform;
			}
		}
		// Handle sprite animation
		if(plane != null && es.GetMovementVector() != Vector3.zero){
			if(defensiveStance){
				AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,1,2,2);
			}
			else{
				AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,0,2,2);
			}
		}
	}
	
	private void CheckStance(){
		if(defensiveStance){
			AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,1,1,2);
			es.MoveSpeed = es.MoveSpeed *.75f;
			ehs.maxHp = ehs.maxHp*4;
			ehs.SetHealth(ehs.curHp*4);
		}
		else{
			AnimationScript.AnimateSprite(plane.GetComponent<Renderer>(),2,2,0,0,1,2);
			es.MoveSpeed = baseMs;
			float percentage = (float)ehs.curHp / ehs.maxHp;
			ehs.maxHp = baseHp;
			ehs.SetHealth(Mathf.RoundToInt(baseHp*percentage));
		}
	}

}
