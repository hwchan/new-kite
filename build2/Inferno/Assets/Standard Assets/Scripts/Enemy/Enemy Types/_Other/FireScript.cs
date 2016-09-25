using UnityEngine;
using System.Collections;

public class FireScript : MonoBehaviour {
	
	public float expiry = 0;
	private float timeSpentAlive;
	private PlayerHealthScript phs;
	private EnemyHealthScript ehs;
	private float timer = 0;
	private float dmgInterval = .25f;
	
	private Color c;
	private bool inc = true;
	private bool dec = false;
	
	// Use this for initialization
	void Start () {
		c = new Color(1,1,1,0);
		GetComponent<Renderer>().material.color = c;
		GetComponent<Collider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		AnimationScript.AnimateSprite(GetComponent<Renderer>(), 2, 1, 0, 0, 2, 4);
		timer++;
		if(inc){
			IncreaseAlpha();
		}
		if(dec){
			DecreaseAlpha();
		}
		if(expiry > 0){
			timeSpentAlive += Time.deltaTime;
			if(timeSpentAlive > expiry){
				RemoveMe();
			}
		}
	}
	
	void OnTriggerStay(Collider col){
		if(timer > dmgInterval/Time.deltaTime){
			timer = 0;
			phs = col.GetComponent<PlayerHealthScript>();
			ehs = col.GetComponent<EnemyHealthScript>();
			if(phs != null){
				// Increase damage if player is too close
				if(Vector3.Distance(transform.position, col.transform.position) <= .35f){
					timer = dmgInterval/Time.deltaTime;
					phs.HurtHealth(2);
				}
				phs.HurtHealth(1);
				
			}
			else if(ehs != null){
				ehs.HurtHealth(3, this);
			}
		}
		
	}
	
	private void DecreaseAlpha(){
		if(c.a > 0){
			c.a -= Time.deltaTime;
			GetComponent<Renderer>().material.color = c;
		}
		// Turn off collider when alpha is .5f
		if(c.a < .5f){
			GetComponent<Collider>().enabled = false;
		}
	}
	
	private void IncreaseAlpha(){
		if(GetComponent<Renderer>().material.color.a >= 1){
			GetComponent<Collider>().enabled = true;
			inc = false;
		}
		else{
			c.a += Time.deltaTime * 1.5f;
			GetComponent<Renderer>().material.color = c;
		}
	}
	
	private void RemoveMe(){
		dec = true;
		Destroy(gameObject, 1);
	}
}
