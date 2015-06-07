using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour {
	
	public float auraRange;
	public float expiry = 0;
	private Collider[] col;
	private float timer = 0;
	private float interval = .1f;
	public float slowStr = .7f;
	
	// Use this for initialization
	void Start () {
		auraRange = transform.localScale.x / 2;
		if(expiry > 0){
			Destroy(gameObject, expiry);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(timer > interval/Time.deltaTime){
			timer = 0;
			HandleAura();
		}
		timer++;
	}
	
	private void HandleAura(){
		if(slowStr < 1){
			int mask = 1 << 9; // Player layer
			mask = mask | 1 << 24; // Neutral layer
			col = Physics.OverlapSphere(transform.position, auraRange, mask);
			foreach(Collider c in col){
				PlayerScript ps = c.GetComponent<PlayerScript>();
				EnemyScript oes = c.GetComponent<EnemyScript>();
				if(ps != null){
					ps.SlowDown(slowStr, .5f);
				}
				else if(oes != null){
					oes.SlowDown(slowStr, .5f);
				}
			}
		}
	}
	
	public void SetSlowStrength(float strength){
		slowStr = 1 - strength;
	}
	
	
	public float GetSlowStrength(){
		return slowStr;
	}
	
	public void SetSize(Vector3 V){
		transform.localScale = V;
		auraRange = V.x / 2;
		if(transform.localScale.x <= 0){
			Destroy(gameObject);	
		}
	}
}
