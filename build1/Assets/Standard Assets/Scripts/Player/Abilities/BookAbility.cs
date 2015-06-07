using UnityEngine;
using System.Collections;

public class BookAbility : AbilityScript {
	
	public string[] text;
	public GameObject textPrefab;
	
	protected override void SetUp(int index){
		projectilePrefabs = new GameObject[text.Length];
		for(int i=0; i<projectilePrefabs.Length; i++){
			projectilePrefabs[i] = textPrefab;
		}
		psh.CopyProjectilePrefabs(projectilePrefabs, index);
	}

}
