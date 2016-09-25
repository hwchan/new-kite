using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	// Movement
	public float moveSpeed = 3;
	private float moveSpeedTmp;
	private Vector3 inputMovement;
	private Vector3 externalForce;
	private Vector3 externalDirection;
	private float moveSpeedMultiplier = 1;
	private Vector3 facingVec;
	private Vector3 trackedLocation;
	
	// Buffs/Debuffs
	private bool stealthActive = false;
	
	CharacterController controller;
	GameObject objCamera;
	
	public const int NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3;
	public const int MOVEMENT = 0, ATTACKING = 1;
	
	void Start(){
		controller = GetComponent<CharacterController>();
		objCamera = GameObject.FindWithTag("MainCamera");
	}
    void FixedUpdate (){
		objCamera.transform.position = new Vector3(transform.position.x, objCamera.transform.position.y, transform.position.z);
		moveSpeedTmp = 0f;
		if(Input.GetMouseButton(0)){
	        MoveToCursor();
			HandleDirection(inputMovement, MOVEMENT);
		}
		HandleMovement();
    }
	
	//http://www.unifycommunity.com/wiki/index.php?title=LookAtMouse
	private void MoveToCursor(){
		// Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position);
    
        // Generate a ray from the cursor position
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    
        // Determine the point where the cursor ray intersects the plane.
        // This will be the point that the object must look towards to be looking at the mouse.
        // Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
        // then find the point along that ray that meets that distance.  This will be the point
        // to look at.
		
        float hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast (ray, out hitdist)) 
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
			
			// Set the direction towards the point clicked
			inputMovement = (targetPoint - transform.position).normalized;
			
			// Now move it
			moveSpeedTmp = moveSpeed;
        }		
	}
	
	private void HandleMovement(){
		// Forcibly move the player if externalDirection is not null. 
		// Otherwise move with an external force acting against it (can be null).
		if(externalDirection == Vector3.zero){
			controller.Move((externalForce + inputMovement) * Time.deltaTime * moveSpeedTmp * moveSpeedMultiplier); 
		}
		else{
			controller.Move(externalDirection * Time.deltaTime * 4);
		}
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);	// Lock to y = 0
	}
	
	/// <summary>
	/// Forces the object to move in a specified direction.
	/// </summary>
	/// <param name='direction'>
	/// The direction to move.
	/// </param>
	/// <param name='duration'>
	/// The duration of the external direction effect.
	/// </param>
	public void setExternalDirection(Vector3 direction, float duration) {	// TODO: Set it so it takes in distance/speed
		CancelInvoke("clearExternalDirection"); // cancel any external direction waiting to be cleared
 		externalDirection = direction;
		Invoke("clearExternalDirection", duration);
	}
	
	private void clearExternalDirection() {
		externalDirection = Vector3.zero;
	}
	
	/// <summary>
	/// Sets an external force to act on the player's movement.
	/// </summary>
	/// <param name='force'>
	/// The direction of the force.
	/// </param>
	public void SetExternalForce(Vector3 force){
		externalForce = force;
	}
	
	/// <summary>
	/// Alters the player's movement speed multiplier.
	/// </summary>
	public void SlowDown(float multiplier, float duration){	// TODO: make it so if there multiple slows, take the highest
		CancelInvoke("SlowDownOff");
		SlowDown(multiplier);
		Invoke("SlowDownOff", duration);
	}
	
	private void SlowDown(float multiplier){
		moveSpeedMultiplier = multiplier;
	}
	
	private void SlowDownOff(){
		moveSpeedMultiplier = 1f;
	}
	
	/// <summary>
	/// Handles the facing direction and its corresponding animations.
	/// </summary>
	/// <param name='input'>
	/// The Vector3 direction to face.
	/// </param>
	/// <param name='anim'>
	/// Choose which animations to play. (MOVEMENT, ATTACKING)
	/// </param>
	public void HandleDirection(Vector3 input, int anim){
		if (input.x	> 0 && Mathf.Abs(input.x) >= Mathf.Abs(input.z) ){	// E
			facingVec = new Vector3(1,0,0);
			SetAnimation(EAST, anim);
		}
		if (input.x	< 0 && Mathf.Abs(input.x) >= Mathf.Abs(input.z)){	// W
			facingVec = new Vector3(-1,0,0);
			SetAnimation(WEST, anim);
		}
		if (input.z	> 0 && Mathf.Abs(input.x) <= Mathf.Abs(input.z)){	// N
			facingVec = new Vector3(0,0,1);
			SetAnimation(NORTH, anim);
		}
		if (input.z	< 0 && Mathf.Abs(input.x) <= Mathf.Abs(input.z)){	// S
			facingVec = new Vector3(0,0,-1);
			SetAnimation(SOUTH, anim);
		}
	}
	
	/// <summary>
	/// Sets the animation to play depending on the facing direction.
	/// </summary>
	/// <param name='direction'>
	/// The facing direction.
	/// </param>
	/// <param name='anim'>
	/// The animation to play.
	/// </param>
	private void SetAnimation(int direction, int anim){
		switch(direction){
			
			case NORTH:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 1, 2, 4);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 1, 1, 4);
						break;
				}
				break;
			
			case EAST:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 2, 2, 4);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 2, 1, 4);
						break;
				}
				break;
			
			
			case SOUTH:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 0, 2, 4);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 0, 1, 4);
						break;
				}
				break;
			
			case WEST:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 3, 2, 4);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 3, 1, 4);;
						break;
				}
				break;
			
		}
	}
	
	/// <summary>
	/// Gets the last visible position of the player. (Can be inaccurate)
	/// </summary>
	/// <returns>
	/// The last tracked position.
	/// </returns>
	public Vector3 GetVisiblePosition() {
		if (stealthActive) {
			return trackedLocation;
		} 
		else {
			return new Vector3(transform.position.x, 0, transform.position.z);
		}
	}
	
	/// <summary>
	/// Sets the player's visible position.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void SetVisiblePosition(Vector3 position){
		trackedLocation = position;
	}
	
	/// <summary>
	/// Gets the (normalized) facing vector.
	/// </summary>
	/// <returns>
	/// The facing vector.
	/// </returns>
	public Vector3 GetFacingVec(){
		return facingVec; 	
	}
	
	/// <summary>
	/// Gets the current temporary movement speed (from 0 to moveSpeed).
	/// </summary>
	/// <returns>
	/// The temporary movement speed.
	/// </returns>
	public float GetMovement(){
		return moveSpeedTmp;
	}
	
}
