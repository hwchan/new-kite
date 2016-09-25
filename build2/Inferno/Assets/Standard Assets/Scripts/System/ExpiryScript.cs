using UnityEngine;
using System.Collections;

public class ExpiryScript : MonoBehaviour {
	
	[SerializeField]
	protected float expiry = 0;
	private int c,r,rfs,cfs,tf,fps = 0;

	void Awake () {
		SetExpiry(expiry);
	}
	void LateUpdate(){
		if(fps > 0){
			AnimationScript.AnimateSprite(GetComponent<Renderer>(),c,r,rfs,cfs,tf,fps);
		}
	}
	
	public void SetExpiry(float f){
		if(f > 0){
			Destroy(gameObject, f);
		}
	}

	public void StartAnimation(int colSize, int rowSize, int rowFrameStart, int colFrameStart, int totalFrames, int fpsec ){
		c=colSize;
		r=rowSize;
		rfs=rowFrameStart;
		cfs=colFrameStart;
		tf=totalFrames;
		fps=fpsec;
	}
	
}
