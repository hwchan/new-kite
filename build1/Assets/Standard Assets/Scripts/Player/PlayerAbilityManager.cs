using UnityEngine;
using System.Collections.Generic;

public class PlayerAbilityManager : MonoBehaviour {
	
	[SerializeField]
	public List<GameObject> abilityList;	// TODO: Use ScriptableObject
	public int abilityLimit = 4;
	
	public Texture attackTexture;
	public Texture healthTexture;
	public Texture speedTexture;
	public Texture sightTexture;

	/// <summary>
	/// Try to add the ability. If successful, return the index of the slot the ability was added in. If not, return -1.
	/// </summary>
	/// <param name='o'>
	/// Ability GameObject.
	/// </param>
	public int AddAbility(GameObject o){
		if(CheckAbilityLimit()){
			// Find first null and replace at that index
			int i = 0;
			while(i < abilityList.Count && abilityList[i] != null){
				i++;
			}
			abilityList[i] = o;
			return i;
		}else{
			return -1;
		}
	}
	
	public bool CheckAbilityLimit(){
		int count = 0;
		foreach(GameObject o in abilityList){
			if(o != null){
				count++;
			}
		}
		return (count < abilityLimit);
	}
	
//	public string GetAbilityName(int index){
//		if(index < abilityList.Count){
//			if(abilityList[index] != null){
//				return abilityList[index].GetComponent<AbilityScript>().AbilityName;
//			}
//		}
//		return "Empty";
//	}
	
	public AbilityScript GetAbilityScript(int index){
		if(index < abilityList.Count){
			if(abilityList[index] != null){
				return abilityList[index].GetComponent<AbilityScript>();
			}
		}
		return null;
	}
	
	public GameObject GetAbilityObject(int index){
		if(index < abilityList.Count){
			if(abilityList[index] != null){
				return abilityList[index];
			}
		}
		return null;
	}
	
	public Texture GetAbilityTexture(int index){
		if(index < abilityList.Count){
			if(abilityList[index] != null){
				return abilityList[index].GetComponent<AbilityScript>().ButtonTexture;
			}
		}
		return null;
	}
	
	public void DropAbilityAsItem(int index){
		if(index < abilityList.Count){
			if(abilityList[index] != null){
				abilityList[index].GetComponent<AbilityScript>().SetDown(index);
				abilityList[index].SetActive(true);
				abilityList[index].transform.position = transform.position;
				abilityList[index] = null;
			}
		}
	}
	
	public void RemoveAbility(AbilityScript abs){
		for(int i=0; i<abilityList.Count; i++){
			if(abilityList[i] != null){
				if(abilityList[i].GetComponent<AbilityScript>() != null){
					if(abilityList[i].GetComponent<AbilityScript>() == abs){
						abilityList[i] = null;
						break;
					}
				}
			}
		}
	}
	
}
