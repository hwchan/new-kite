using UnityEngine;
using System.Collections.Generic;

public class Glutton : EnemyType {
	
	private Collider[] col;
	public int food = 1;
	public float auraRange = 2.1f;
	public bool gizmoToggle = false;
	public bool defaultSettings = true;
	private EnemyHealthScript ehs;
	private float timer = 0;
	private float interval = .5f;
	private bool stopped = false;
	private float eatAnimTime = 2f;
	
	private Transform child;
	
	// Use this for initialization
	void Start () {
		ehs = GetComponent<EnemyHealthScript>();
		child = transform.Find("Plane");
		Vector3 temp = transform.position;
		temp.y = -5;
//		GameObject aura = (GameObject) Instantiate(VariableScript.objSmellyAura, temp, Quaternion.identity);
//		aura.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(!stopped && timer > interval/Time.deltaTime){
			timer = 0;
			HandleAura();
			HandleEat();
		}
		timer++;
		if(child != null){
			AnimationScript.AnimateSprite(child.GetComponent<Renderer>(), 4, 8, 1, 4, 2, 6);
		}
	}
	
	private void HandleAura(){
		int mask = 1 << 9; // Player layer
		mask = mask | 1 << 24; // Neutral layer
		col = Physics.OverlapSphere(transform.position, auraRange, mask);
		foreach(Collider c in col){
			PlayerScript ps = c.GetComponent<PlayerScript>();
			EnemyScript oes = c.GetComponent<EnemyScript>();
			if(ps != null){
				ps.SlowDown(.6f, .75f);
			}
			else if(oes != null){
				oes.SlowDown(.6f, .75f);
			}
		}
	}
	
	private void HandleEat(){
		int mask = 1 << 8; // Enemy layer
		mask = mask | 1 << 24; // Neutral layer
		List<GameObject> eatObjs = UtilityScript.GetObjectsInLOS(transform.position, 1.2f, mask, GetComponent<Collider>());
		if(eatObjs.Count > 0){
			EnemyHealthScript oehs = eatObjs[0].GetComponent<EnemyHealthScript>();
			Glutton g = eatObjs[0].GetComponent<Glutton>();
			if(oehs != null && g == null){
				oehs.SetHealth(0);
				oehs.HurtHealth(1, this);
				es.Pause(eatAnimTime);
				FoodIncrease();
			}
		}
	}
	
	private void FoodIncrease(){
		if(food < 10){
			if(food % 2 == 0){
				ehs.maxHp++;
				ehs.HealHealth(1);
			}
			food++;
			Transform tex = transform.Find("Plane");
			if(tex != null){
				tex.transform.localScale += new Vector3(.03f,0,.03f);
			}
			tex = transform.Find("Exclamation Prefab");
			if(tex != null){
				tex.transform.localPosition -= new Vector3(.01f,0,.01f);
			}
		}
	}
	
	public override void KillMe(){
//		GameObject slime = (GameObject) Instantiate(VariableScript.objSlime, transform.position, Quaternion.Euler(0,180,0));
//		slime.transform.localScale += new Vector3(food*.5f, 0, food*.5f);
//		Slime s = slime.GetComponent<Slime>();
//		if(s != null){
//			s.SetSlowStrength(.3f + food * .02f);
//		}
	}
	
	void OnDrawGizmos(){
		if(gizmoToggle){
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, auraRange);
		}
	}
}
