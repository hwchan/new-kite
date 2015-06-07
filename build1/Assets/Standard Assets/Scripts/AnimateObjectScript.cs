using UnityEngine;
using System.Collections;

public class AnimateObjectScript : MonoBehaviour {

	public int columnSize,rowSize,rowFrameStart,columnFrameStart,numberOfFrames,framesPerSecond;

	// Update is called once per frame
	void Update () {
		if(GetComponent<Renderer>() != null){
			AnimationScript.AnimateSprite(GetComponent<Renderer>(),columnSize,rowSize,rowFrameStart,columnFrameStart,numberOfFrames,framesPerSecond);
		}
	}
}
