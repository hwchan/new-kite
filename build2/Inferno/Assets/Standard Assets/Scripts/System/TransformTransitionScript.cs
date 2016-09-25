using UnityEngine;
using System.Collections;

// Attach this to a gameobject to smoothly change size (Lerp)
public class TransformTransitionScript : MonoBehaviour {
	
	public Vector3 scale1;
	public Vector3 scale2;
	public float scaleRate = 1;
	
	private Vector3 modifiedTransformScale;
	private float creationTime;
	private Transform thisTransform;
	
	void Start () {
		creationTime = Time.time;
		// Get the correct transform from the gameobject with a meshrenderer
		// NOTE: If it's on a seperate plane, its collider won't scale with the plane
		if(transform.Find("Plane") != null){
			thisTransform = transform.Find("Plane");
		}else{
			thisTransform = transform;
		}
		TransitionTransform();	// In case FixedUpdate takes too long to happen
	}
	
	void FixedUpdate () {
		TransitionTransform();
	}
	
	private void TransitionTransform(){
		modifiedTransformScale = Vector3.Lerp(scale1,scale2,(Time.time-creationTime)/scaleRate);
		thisTransform.localScale = modifiedTransformScale;
		if(thisTransform.localScale == scale2){
			Destroy(this);
		}
	}
	
	/// <summary>
	/// "Constructor" of this component. Transitions from current scale to 'newScale' smoothly.
	/// </summary>
	/// <param name='newScale'>
	/// New scale to transition to.
	/// </param>
	public void SetUp(Vector3 newScale){
		scale1 = thisTransform.localScale;
		scale2 = newScale;
	}
	
	/// <summary>
	/// "Constructor" of this component. Transitions from 'oldScale' to 'newScale' smoothly.
	/// </summary>
	/// <param name='oldScale'>
	/// Old scale to transition from.
	/// </param>
	/// <param name='newScale'>
	/// New scale to transition to.
	/// </param>
	/// <param name='transitionSpeed'>
	/// Scale transition speed. Default is '1', 0 to 1: faster, 1+: slower.
	/// </param>
	public void SetUp(Vector3 oldScale, Vector3 newScale, float transitionSpeed){
		scale1 = oldScale;
		scale2 = newScale;
		scaleRate = transitionSpeed;
	}
}
