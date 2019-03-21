using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {
	
	public int numSwitches = 1;
	public bool isOpen = false;
	
	// Use this for initialization
	void Start () {
		HandleLoS();
		HandleOpen(isOpen);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void TriggerMe(){
		numSwitches--;
		if(numSwitches <= 0){
			isOpen = !isOpen;
			HandleOpen(isOpen);
		}
	}
	
	private void HandleOpen(bool open){
		if(!open){
			isOpen = false;
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider>().enabled = true;
			gameObject.tag = "Wall";
			transform.FindChild(gameObject.name + "(Clone)").GetComponent<Renderer>().enabled = true;
			//update mapInfo
			MapScript.getMap().addStaticObject(transform.position, transform.GetComponent<Collider>().bounds.size);
		}
		else if(open){
			isOpen = true;
			//update mapInfo
			MapScript.getMap().removeStaticObject(transform.position, transform.GetComponent<Collider>().bounds.size);
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
			gameObject.tag = "Untagged";
			transform.FindChild(gameObject.name + "(Clone)").GetComponent<Renderer>().enabled = false;
		}
	}
	
	private void HandleLoS(){
		GameObject newWall = (GameObject) Instantiate(gameObject);
		Destroy(newWall.GetComponent<DoorScript>());
		newWall.transform.parent = transform;
		newWall.GetComponent<Renderer>().material = VariableScript.matLineOfSightSolid;
		newWall.layer = 21;
		newWall.tag = "Untagged";
				
		// Change the y scale
		Vector3 tempScale = newWall.transform.localScale;
		tempScale.y = 10;
		newWall.transform.localScale = tempScale;
				
		// Move by y coordinate
		Vector3 tempPos = newWall.transform.position;
		tempPos.y = -10f;
		newWall.transform.position = tempPos;
	}
}
