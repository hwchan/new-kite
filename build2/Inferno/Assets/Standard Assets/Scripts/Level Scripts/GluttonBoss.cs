using UnityEngine;
using System.Collections;

public class GluttonBoss : MonoBehaviour {
	
	private bool TriggerToggle = false;
	public float hunger = 0;
	public float speed;
	public GameObject boss;
	public GameObject door;
	private DoorScript ds;
	private EnemyScript es;
	private EnemyHealthScript ehs;
	private PlayerScript ps;
	
	public Collider[] col;
	
	// Use this for initialization
	void Start () {
		ds = door.GetComponent<DoorScript>();
		es = boss.GetComponent<EnemyScript>();
		ehs = boss.GetComponent<EnemyHealthScript>();
		ps = VariableScript.scrPlayerScript1;
		speed = es.MoveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if(ehs.curHp <= 0){
			Destroy(boss.gameObject);
			CancelInvoke();
		}
	}
	
	void OnTriggerEnter(Collider col){
		if(ds.isOpen && !TriggerToggle && col.GetComponent<PlayerScript>() != null){
			TriggerToggle = true;
			ps.transform.position = transform.position;
			es.SetSearchRange(100);
			InvokeRepeating("HandleEat", 0, .1f);
			InvokeRepeating("IncreaseHunger", 3, 3);
			InvokeRepeating("CheckHunger", 1, 1);
		}
	}
	
	private void CheckHunger(){
		es.MoveSpeed = speed + hunger*.25f;
	}
	
	private void IncreaseHunger(){
		hunger++;
	}
	
	private void HandleEat(){
		// Only eat when hunger > 0
		if(hunger > 0){
			col = Physics.OverlapSphere(boss.transform.position, 1, 1 << 17);	// PickedUp layer
			foreach(Collider c in col){
				Slime s = c.GetComponent<Slime>();
				if(s != null){
					Eat(s.gameObject);
				}
			}
		}
	}
	
	private void Eat(GameObject o){
		Slime s = o.GetComponent<Slime>();
		Vector3 V = o.transform.localScale - new Vector3(.1f, 0, .1f);
		s.SetSize(V);
		hunger -= .1f;
		es.Pause(.2f);
	}
	
}
