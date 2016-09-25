using UnityEngine;
using System.Collections.Generic;

public static class UtilityScript {

	public static Object[] RandomizeArray(Object[] arr){
		for(int i = 0; i < arr.Length; i++){
			// Swap arr[i], arr[random]
			int j = Random.Range(i, arr.Length);
			Object temp = arr[i];
			arr[i] = arr[j];
			arr[j] = temp;
		}
		return arr;
	}
	
	public static List<GameObject> GetPlayersInRange(Vector3 origin, float range){
		List<GameObject> playerGroup = new List<GameObject>(VariableScript.players);
		List<GameObject> toDelete = new List<GameObject>();
		// Add objects to delete into a new list
		foreach(GameObject o in playerGroup){
			if(Vector3.Distance(o.transform.position, origin) > range){
				toDelete.Add(o);
			}
		}
		// Delete objects as listed in temp list from returned list
		foreach(GameObject o in toDelete){
			playerGroup.Remove(o);
		}
		return playerGroup;
	}
	
	/// <summary>
	/// Gets the closest player in range.
	/// </summary>
	/// <returns>
	/// The closest player in range.
	/// </returns>
	/// <param name='origin'>
	/// Origin position.
	/// </param>
	/// <param name='range'>
	/// Radius from origin to look.
	/// </param>
	public static GameObject GetClosestPlayerInRange(Vector3 origin, float range){
		float closestDistance = float.MaxValue;
		float testDistance = 0;
		GameObject closestObj = null;
		
		foreach(GameObject player in VariableScript.players){
			testDistance = Vector3.Distance(player.transform.position, origin);
			if(testDistance <= range && testDistance <= closestDistance){
				closestObj = player;
				closestDistance = testDistance;
			}
		}
		
		return closestObj;
	}
	
	public static GameObject GetRandomPlayerInRange(Vector3 origin, float range){
		List<GameObject> inRangePlayers = new List<GameObject>();
		foreach(GameObject player in VariableScript.players){
			if(Vector3.Distance(player.transform.position, origin) <= range){
				inRangePlayers.Add(player);
			}
		}
		if(inRangePlayers.Count == 0){
			return null;
		}
		short i = (short)Random.Range(0, inRangePlayers.Count);
		return inRangePlayers[i];
		
	}
	
	/// <summary>
	/// Gets all objects in line of sight. Only objects in the "Wall" layer blocks line of sight. Ignores calling object.
	/// </summary>
	/// <returns>
	/// The objects in line of sight.
	/// </returns>
	/// <param name='origin'>
	/// The starting position.
	/// </param>
	/// <param name='range'>
	/// The range to look for objects.
	/// </param>
	/// <param name='mask'>
	/// The physics layers to look for the objects. Accepts a layer mask.
	/// </param>
	/// <param name='ignore'>
	/// The collider to ignore.
	/// </param>
	public static List<GameObject> GetObjectsInLOS(Vector3 origin, float range, int mask, Collider ignore){
		List<GameObject> collided = new List<GameObject>();
		Collider[] col = Physics.OverlapSphere(origin, range, mask);
		
		int hit = 1 << 27;	// Wall layer blocks LoS
		foreach(Collider o in col){
			if(o != ignore && !Physics.Raycast(origin, o.transform.position - origin, Vector3.Distance(o.transform.position, origin), hit)){
				collided.Add(o.gameObject);
			}
		}
		return collided;
	}
	
	/// <summary>
	/// Checks if an object is at target location. Use this instead of Vector3.Distance checks for better performance.
	/// Doesn't check y-coordinate.
	/// </summary>
	/// <returns>
	/// Returns true if the object is at target location.
	/// </returns>
	/// <param name='position'>
	/// Position of object.
	/// </param>
	/// <param name='target'>
	/// Target location.
	/// </param>
	public static bool CheckIfAtTarget(Vector3 position, Vector3 target){
		if( (Mathf.Abs(position.x - target.x) < .1f) && (Mathf.Abs(position.z - target.z) < .1f) )
			return true;
		else
			return false;
	}
	
	/// <summary>
	/// Changes the y-coordinate of a Vector3.
	/// </summary>
	/// <returns>
	/// The Vector3 with the editted y-coordinate.
	/// </returns>
	/// <param name='originalPosition'>
	/// Original position.
	/// </param>
	/// <param name='newY'>
	/// New y.
	/// </param>
	public static Vector3 MoveY(Vector3 originalPosition, float newY){
		return new Vector3(originalPosition.x, newY, originalPosition.z);
	}
	
}
