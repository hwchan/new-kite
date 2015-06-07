using UnityEngine;
using System.Collections;

public class Corpulent : EnemyType {
	
	public float countdown = 2;
	private Collider[] explosionObjs;
	private float r = 2f;
	public int damage = 2;
	private float distance;
	public float explRange = 2f;
	private Transform objChildPlane;
	public bool defaultSettings = true;
	
	void Start () {
		objChildPlane = transform.Find("Plane");
		InvokeRepeating("HandleTrigger", 1f, .25f);
	}
	
//	public override void StartMe(){
//		InvokeRepeating("HandleTrigger", 1f, .25f);
//	}
//	
//	public override void StopMe(){
//		CancelInvoke();
//		countdown = 2;
//		objChildPlane.renderer.material.color = Color.white;
//	}
	
	void Update () {
		AnimationScript.AnimateSprite(objChildPlane.GetComponent<Renderer>(), 4, 1, 1, 0, 2, 8);
	}
	
	void HandleTrigger(){
		int mask = 1 << 9; // Player layer
		Collider[] col = Physics.OverlapSphere(transform.position, explRange*.65f, mask);
		if(col.Length > 0){
			CancelInvoke("HandleTrigger");
			if(es != null){
				es.SlowDown(.5f, 3f);
			}
			InvokeRepeating("Countdown", 0, .25f);
		}
	}
	
	void Countdown(){
		countdown -= .25f;
		objChildPlane.GetComponent<Renderer>().material.color = new Color(1,1,1, objChildPlane.GetComponent<Renderer>().material.color.a - .1f);
		if( countdown < 0){
			CancelInvoke("Countdown");
			Explode();
		}
	}
	
	void Explode(){
		int mask = 1 << 9; // Player layer
		mask = mask | 1 << 24; // Neutral layer
		explosionObjs = Physics.OverlapSphere(transform.position, r, mask);
		foreach(Collider col in explosionObjs){
			PlayerHealthScript phs = col.GetComponent<PlayerHealthScript>();
			EnemyHealthScript ehs = col.GetComponent<EnemyHealthScript>();
			if(phs != null){
				phs.HurtHealth(damage);
			}
			else if(ehs != null){
				ehs.HurtHealth(damage, this);
			}
		}
		GameObject temp = (GameObject) Instantiate(VariableScript.objExplosion, transform.position, Quaternion.Euler(-90,0,0));	//TODO: Change into animation
		Destroy(temp, .75f);
		es.RemoveMe();
	}
}
