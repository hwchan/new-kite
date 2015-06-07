using UnityEngine;
 
public class VariableScript : MonoBehaviour
{
	public static int intEnemyLayerMask;
	public static int intPlayerUnitsLayerMask;
	public static int intHostileLayerMask;
	
	public static GameObject[] players;
	
	public static string playersTag = "Player";
	
	public static GameObject objPlayer1;
	public static PlayerScript scrPlayerScript1;
	public static PlayerStats scrPlayerExperience1;
	public static PlayerHealthScript scrPlayerHealthScript1;
	public static PlayerShoot scrPlayerShoot1;
	public static bool thrown1 = false;
	//public static GameObject objPlayer2;
	
	public static GameObject objBullet;
	public static GameObject objExplodingShot;
	public static GameObject objArrow;
	public static GameObject objFire;
	public static GameObject objExplosion;
	
	public static GameObject objTemp;
	
	public static GameObject objText;

	public static Texture texAttribute;
	
	public static Material matLineOfSightSolid;
	public static Material matLineOfSightTransparent;
	public static Material matSkull;
	
	void Awake(){
		VariableResourcesScript vrs = GetComponent<VariableResourcesScript>();
		
		players = GameObject.FindGameObjectsWithTag("Player");
		
		objPlayer1 = vrs.objPlayer1;
		scrPlayerScript1 = objPlayer1.GetComponent<PlayerScript>();
		scrPlayerExperience1 = objPlayer1.GetComponent<PlayerStats>();
		scrPlayerHealthScript1 = objPlayer1.GetComponent<PlayerHealthScript>();
		scrPlayerShoot1 = objPlayer1.GetComponent<PlayerShoot>();
		//objPlayer2 = vrs.objPlayer2;
		
		playersTag = objPlayer1.tag;
		// 8: Enemy, 15: SpawnPoint, 20: Flying, 24: Neutral, 25: Immune
		intEnemyLayerMask = 1 << 8 | 1 << 15 | 1 << 20 | 1 << 24 | 1 << 25;
		intHostileLayerMask = 1 << 8 | 1 << 20 | 1 << 25;
		intPlayerUnitsLayerMask = 1 << 9;
		
		objBullet = vrs.objBullet;
		objExplodingShot = vrs.objExplodingShot;
		objArrow = vrs.objArrow;
		objFire = vrs.objFire;
		objExplosion = vrs.objExplosion;
		
		objTemp = vrs.objTemp;
		
		objText = vrs.objText;

		texAttribute = vrs.texAttribute;

		matLineOfSightSolid = vrs.matLineOfSight;
		matLineOfSightTransparent = vrs.matLineOfSightTransparent;
		matSkull = vrs.matSkull;
	}
	

	
}