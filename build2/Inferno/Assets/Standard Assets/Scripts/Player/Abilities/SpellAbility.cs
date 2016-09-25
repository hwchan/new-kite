using UnityEngine;
using System.Collections;

public class SpellAbility : AbilityScript {
	
	protected override void SetUp(int index){
		psh.CopyProjectilePrefabs(projectilePrefabs, index);
	}

}
