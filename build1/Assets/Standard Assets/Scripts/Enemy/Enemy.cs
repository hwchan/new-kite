using UnityEngine;
using System.Collections;

public class Enemy {
	
	public GameObject gameObject;
	public string scriptType;
	public bool isAlive;
	
	public Enemy( GameObject obj, string name, bool alive ){
		gameObject = obj;
		scriptType = name;
		isAlive = alive;
	}

}
