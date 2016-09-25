using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	// Movement
//	public float moveSpeed = 3;
	[SerializeField]
	private Vector3 inputMovement;
	private Vector3 externalForce;
	private Vector3 externalDirection;
	private float moveSpeedMultiplier = 1;
	private float moveSpeedWhileCharge = 1;		// Movement speed while charging an attack
	[SerializeField]
	private Vector3 facingVec;
	private Vector3 trackedLocation;
	
	public float MoveSpeedWhileCharge{
		get{return moveSpeedWhileCharge;}
		set{moveSpeedWhileCharge = value;}
	}
	
	// Buffs/Debuffs
	private bool stealthActive = false;
	
	private CharacterController controller;
	public CameraScript playerCameraScript;
//	private bool lockCamera = true;
//	public GameObject shootTarget;
	
	private PlayerStats pstat;
	
	public const int NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3;
	public const int MOVEMENT = 0, ATTACKING = 1;
	
	void Start(){
        controller = GetComponent<CharacterController>();
		playerCameraScript = GameObject.FindWithTag("MainCamera").GetComponent<CameraScript>();
		facingVec = Vector3.left;
		pstat = GetComponent<PlayerStats>();
	}
    void Update (){
		HandleKeyboard();
		HandleMovement();	
    }
	
//	void LateUpdate(){
//		HandleCamera(lockCamera);
//	}
	
	private void HandleKeyboard(){
		inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		HandleDirection(inputMovement, MOVEMENT);
	}
	
//	private void HandleCamera(bool b, float shakeMagnitude){
//		if(b){
//			objCamera.transform.position = new Vector3(transform.position.x, objCamera.transform.position.y, transform.position.z);
//		}
//		if(shakeMagnitude != 0){
//			float x_change = UnityEngine.Random.Range(-0.05f, 0.05f);
//			float z_change = UnityEngine.Random.Range(-0.05f, 0.05f);
//			objCamera.transform.position = new Vector3 (transform.position.x + x_change,4,transform.position.z + z_change);
//		}
//	}
//	
//	public void LockCamera(bool b){
//		lockCamera = b;
//	}
//
//	public void ShakeCamera(float len, float magnitude){
//		if(len <= 0 || magnitude <= 0){
//			return;
//		}
//		LockCamera(false);
//	}

	//http://www.unifycommunity.com/wiki/index.php?title=LookAtMouse
	private Vector3 GetCursorPosition(){
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
			return targetPoint;
        }	
		return Vector3.zero;
	}
	
	private void HandleMovement(){
		// Forcibly move the player if externalDirection is not null. 
		// Otherwise move with an external force acting against it (can be null).
		if(externalDirection == Vector3.zero){
			controller.Move((externalForce + inputMovement) * pstat.moveSpeed * 
					Time.deltaTime * moveSpeedMultiplier * moveSpeedWhileCharge); 
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
	public void SetExternalDirection(Vector3 direction, float duration) {	// TODO: Set it so it takes in distance/speed
		CancelInvoke("clearExternalDirection"); // cancel any external direction waiting to be cleared
 		externalDirection = direction;
		Invoke("ClearExternalDirection", duration);
	}
	
	private void ClearExternalDirection() {
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
			// Flip the texture if changing direction. Flip if scale is positive (unflipped)
			if(transform.localScale.x > 0){
				transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
			}
			facingVec = new Vector3(1,0,0);
			SetAnimation(EAST, anim);
		}
		else if (input.x < 0 && Mathf.Abs(input.x) >= Mathf.Abs(input.z)){	// W
			// Flip the texture if changing direction. Flip if scale is negative (flipped)
			if(transform.localScale.x < 0){
				transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
			}
			facingVec = new Vector3(-1,0,0);
			SetAnimation(WEST, anim);
		}
		else if (input.z	> 0 && Mathf.Abs(input.x) <= Mathf.Abs(input.z)){	// N
			facingVec = new Vector3(0,0,1);
			SetAnimation(NORTH, anim);
		}
		else if (input.z	< 0 && Mathf.Abs(input.x) <= Mathf.Abs(input.z)){	// S
			facingVec = new Vector3(0,0,-1);
			SetAnimation(SOUTH, anim);
		}
	}
	
//	private void HandleDirection(Vector3 input, int anim){
//		// East
//		if(input.x > 0){
//			facingVec = new Vector3(1,0,0);
//			SetAnimation(EAST, anim);
//		}
//		// West
//		else if(input.x < 0){
//			facingVec = new Vector3(-1,0,0);
//			SetAnimation(WEST, anim);
//		}
//	}
	
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
						AnimationScript.AnimateSprite(GetComponent<Renderer>(),2,1,0,0,2,2);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 1, 1, 4);
						break;
				}
				break;
			
			case EAST:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(),2,1,0,0,2,2);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 2, 1, 4);
						break;
				}
				break;
			
			
			case SOUTH:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(),2,1,0,0,2,2);
						break;
					case ATTACKING:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(), 4, 4, 0, 0, 1, 4);
						break;
				}
				break;
			
			case WEST:
				switch(anim){
					case MOVEMENT:
						AnimationScript.AnimateSprite(GetComponent<Renderer>(),2,1,0,0,2,2);
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
	/// Gets the current movement direction.
	/// </summary>
	/// <returns>
	/// The current movement direction.
	/// </returns>
	public Vector3 GetMovement(){
		return new Vector3(Mathf.Ceil(inputMovement.x),0,Mathf.Ceil(inputMovement.z));
	}
	
}
