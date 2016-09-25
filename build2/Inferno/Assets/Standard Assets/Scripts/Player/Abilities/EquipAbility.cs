using UnityEngine;
using System.Collections;

public class EquipAbility : AbilityScript {

	protected override void SetUp(int index){
		pstat.UpgradeHealth(2);
	}
	
	public override void SetDown(int index){
		pstat.UpgradeHealth(-2);
	}
}
