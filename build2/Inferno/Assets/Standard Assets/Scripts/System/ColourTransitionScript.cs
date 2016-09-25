using UnityEngine;
using System.Collections;

// Attach this on an object with a mesh renderer to smoothly fade in or fade out/transition colour
public class ColourTransitionScript : MonoBehaviour {
	
	public Color colour1;
	public Color colour2;
	public float fadeRate = 1;
	
	private Color modifiedColour;
	private float creationTime;
	private Renderer thisRenderer;
	
	void Start () {
		creationTime = Time.time;
		GetRenderer();
		TransitionColour();	// In case FixedUpdate takes too long to happen
	}
	
	void FixedUpdate () {
		TransitionColour();
	}
	
	private void TransitionColour(){
		modifiedColour = Color.Lerp(colour1, colour2, (Time.time-creationTime)/fadeRate);
		thisRenderer.material.color = modifiedColour;
		if(modifiedColour == colour2){
			Destroy(this);
		}
	}
	
	private void GetRenderer(){
		if(GetComponent<Renderer>() == null){
			thisRenderer = transform.Find("Plane").GetComponent<Renderer>();
		}else{
			thisRenderer = GetComponent<Renderer>();
		}
	}
	
	/// <summary>
	/// "Constructor" of this component. Transitions from current colour to 'newColour' smoothly.
	/// </summary>
	/// <param name='newColour'>
	/// New colour to transition to.
	/// </param>
	public void SetUp(Color newColour){
		GetRenderer();
		colour1 = thisRenderer.material.color;
		colour2 = newColour;
	}
	
	/// <summary>
	/// "Constructor" of this component. Transitions from current colour to 'newColour' smoothly.
	/// </summary>
	/// <param name='newColour'>
	/// New colour to transition to.
	/// </param>
	/// <param name='transitionSpeed'>
	/// Colour transition speed. Default is '1', 0 to 1: faster, 1+: slower.
	/// </param>
	public void SetUp(Color newColour, float transitionSpeed){
		GetRenderer();
		colour1 = thisRenderer.material.color;
		colour2 = newColour;
		fadeRate = transitionSpeed;
	}
	
	/// <summary>
	/// "Constructor" of this component. Transitions from 'oldColour' to 'newColour' smoothly.
	/// </summary>
	/// <param name='oldColour'>
	/// Old colour to transition from.
	/// </param>
	/// <param name='newColour'>
	/// New colour to transition to.
	/// </param>
	/// <param name='transitionSpeed'>
	/// Colour transition speed. Default is '1', 0 to 1: faster, 1+: slower.
	/// </param>
	public void SetUp(Color oldColour, Color newColour, float transitionSpeed){
		GetRenderer();
		colour1 = oldColour;
		colour2 = newColour;
		fadeRate = transitionSpeed;
	}
}

