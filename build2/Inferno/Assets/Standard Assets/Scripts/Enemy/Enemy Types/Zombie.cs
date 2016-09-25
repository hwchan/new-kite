using UnityEngine;
using System.Collections;

public class Zombie : EnemyType {
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// Handle texture flip depending on facing
		if( plane != null && (plane.localScale.x > 0 && es.GetFacingVector().x < 0) || (plane.localScale.x < 0 && es.GetFacingVector().x > 0) ){	
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
		}
	}
	
}
