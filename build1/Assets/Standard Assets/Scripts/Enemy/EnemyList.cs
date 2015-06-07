using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyList {
	
	static List<Enemy> objList;
	public static int _totalEnemies;
	
	static EnemyList(){
		objList = new List<Enemy>();
	}
	
	public static void Add(Enemy obj){
		objList.Add(obj);
	}
	
	public static void Kill(GameObject obj){
		foreach(Enemy o in objList){
			if(obj == o.gameObject){
				o.isAlive = false;
			}
		}
	}
	
	public static int NumAlive(){
		int i = 0;
		foreach(Enemy o in objList){
			if(o.isAlive){
				i++;
			}
		}
		return i;
	}
	
	public static void Print(){
		Debug.Log("Enemies in game:");
		foreach(Enemy o in objList){
			Debug.Log(o.gameObject + " : " + o.scriptType + " : " + o.isAlive);
		}
	}
	
	public static List<Enemy> GetEnemies(){
		return objList;
	}

}
