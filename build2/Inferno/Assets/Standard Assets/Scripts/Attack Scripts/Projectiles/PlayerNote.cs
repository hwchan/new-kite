using UnityEngine;
using System.Collections;

public class PlayerNote : ExplodingShot {
	
	private string text;
	
	protected override void RemoveMe(){
		Destroy(gameObject);
	}
	
	public override void Initialize(GameObject abilityPrefab, int chargeLevel){
		GUIManager.FloatingText(abilityPrefab.GetComponent<BookAbility>().text[chargeLevel], transform.position, 0).transform.parent = transform;
	}
	
	public override void SetUp(Vector3 target, float charge, int damage){
		RemoveMe();
	}
}
