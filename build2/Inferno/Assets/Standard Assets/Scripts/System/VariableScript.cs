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
		// 8: Enemy, 15: Spawn Point, 20: Flying, 24: Neutral, 25: Immune     
		intEnemyLayerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Flying") | 1 << LayerMask.NameToLayer("Neutral") | 1 << LayerMask.NameToLayer("Immune") | 1 << LayerMask.NameToLayer("Spawn Point");
		intHostileLayerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Spawn Point") | 1 << LayerMask.NameToLayer("Immune");
		intPlayerUnitsLayerMask = 1 << LayerMask.NameToLayer("Player");
		
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