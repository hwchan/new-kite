using UnityEngine;
using System.Collections;

public class VariableResourcesScript : MonoBehaviour {
	
	public GameObject[] players;
	public GameObject objPlayer1;
	//public GameObject objPlayer2;
	
	public GameObject objBullet;
	public GameObject objExplodingShot;
	public GameObject objArrow;
	public GameObject objFire;
	public GameObject objExplosion;
	
	public GameObject objTemp;
	
	public GameObject objText;

	public Texture texAttribute;
	
	public Material matLineOfSight;
	public Material matLineOfSightTransparent;
	public Material matSkull;
	
	void Awake(){
		players = GameObject.FindGameObjectsWithTag("Player");
		if(players.Length > 0){
			objPlayer1 = players[0];
			//objPlayer2 = players[1];
		}
	}
}
