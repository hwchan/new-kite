using UnityEngine;
using System.Collections;

public class LineOfSightScript : MonoBehaviour {
	
	public GameObject lineOfSight;
	
	// Line of Sight setup, see DoorScript for door line of sight handling
	void Awake () {
		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		foreach(GameObject wall in walls){
			if(wall.layer == 27){
				// Make a cube LoS blocker shadow
				GameObject newWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
				newWall.GetComponent<Renderer>().material = VariableScript.matLineOfSightTransparent;
				newWall.layer = 29;
				newWall.tag = "Untagged";
				Destroy(newWall.GetComponent<Collider>());
				newWall.transform.parent = lineOfSight.transform;
				
				// Copy the scale
				newWall.transform.localScale = new Vector3(wall.transform.localScale.x, 10, wall.transform.localScale.z);
				
				// Move by y coordinate
				newWall.transform.position = new Vector3(wall.transform.position.x, -10, wall.transform.position.z);
			
			
				// Make a cylinder LoS blocker shadow
				GameObject newWall2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				newWall2.GetComponent<Renderer>().material = VariableScript.matLineOfSightSolid;
				newWall2.layer = 29;
				newWall2.tag = "Untagged";
				Destroy(newWall2.GetComponent<Collider>());
				newWall2.transform.position = newWall.transform.position;
				
				// Copy the scale
				newWall2.transform.localScale = new Vector3(wall.transform.localScale.x, 5, wall.transform.localScale.z);
				newWall2.transform.parent = newWall.transform;
				
				
//				// TODO: Tree y coordinate for overlap
//				if(wall.name == "Tree Prefab"){
//					wall.transform.FindChild("Plane").position = new Vector3(wall.transform.position.x, wall.transform.FindChild("Plane").position.z*-.0001f, wall.transform.FindChild("Plane").position.z);
//				}
			}
		}
	}
	
}
