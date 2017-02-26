using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {
	
	private float moveSpeed = 2f;	
	public float MoveSpeed{
		get{return moveSpeed;}
		set{moveSpeed = value;}
	}
	private float attackRange = 1f;
	public float AttackRange{
		get{return attackRange;}
		set{attackRange = value;}
	}
	
	public bool guardMode = false;
	private float moveSpeedMultiplier = 1f;
	private int attackPause = 1;
	private bool slowActive = false;
	private GameObject objPlayer;
	private PlayerScript ps;
	private Vector3 inputMovement;
	private Vector3 finalMovement;
	private Vector3 updatedVector;
	public AI passiveAI;
	public AI aggroAI;
	public AI currentAI;
	public bool gizmoToggle = false;
	public float searchRange = 3f;
	public float aggroSearchRange;
	public float passiveSearchRange;
	public float srchRngeMultplr = 1.75f;
	public int isFree = 1; // 1 is true, 0 is false
	private Vector3 externalDirection;
	
	private GameObject objPowerup;
	public bool stopAI = false;
	
	// the target location the enemy is trying to get to
	private Vector3 AItarget;
	private LevelInfo mapInfo;
//	public bool genPattern = true;
	public Vector3[] patternPoints;
	private int patternIndex = 0;
	
	public string spawnPointOrigin;
	
	private Vector3 turnVec = Vector3.right;
	private int timerAI = 0;
	private Vector3 gridVec;
	private int moveTimer = 0;
//	private bool isInc = true;	// For spiralAI
	
	public bool isRotating = false;
	private float rotateDur = 0f;
	private float rotateDegrees = 0f;
	
	public Vector3 target;
	
	private Vector3 leashPt;
	
//	private ChargeyScript cs;
//	private ExplodeyScript exs;
	public bool isKO = false;
	
	void Start () {

		// Get the move-speed and attack-range from EnemyType
		EnemyType et = GetComponent<EnemyType>();
		if(et != null){
			moveSpeed = et.moveSpeed;
			attackRange = et.attackRange;
			Enemy tmp = new Enemy(gameObject, et.ToString(), true);
			EnemyList.Add(tmp);
		}
		
		objPlayer = (GameObject) GameObject.FindWithTag("Player");
		ps = objPlayer.GetComponent<PlayerScript>();
		updatedVector = new Vector3(1,0,1);
		mapInfo = MapScript.getMap();
		externalDirection = Vector3.zero;
//		if (genPattern){
//			generatePatternPoints();
//		}
		
		gridVec = transform.position;
		leashPt = transform.position;
		SetSearchRange(searchRange);
//		cs = GetComponent<ChargeyScript>();
//		exs = GetComponent<ExplodeyScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if( !isKO ){
			if( isFree == 1 && currentAI != AI.IdleAI && Vector3.Distance(ps.GetVisiblePosition(), transform.position) <= attackRange){
				TryAttack();
				currentAI = AI.AttackAI;
			}
			else{
				FindAIInput();
			}
		}
		ProcessMovement();
	}
	
	private void TurnAggroAI(){
		currentAI = aggroAI;
		searchRange = aggroSearchRange;
		CallForHelp(2);
	}
	
	/// <summary>
	/// Call for help from nearby mobs. Mobs called upon will have their search range switched to aggro switch range.
	/// </summary>
	/// <param name='callRadius'>
	/// Radius to call for help.
	/// </param>
	public void CallForHelp(float callRadius){
		Collider[] col = Physics.OverlapSphere(transform.position, callRadius, VariableScript.intHostileLayerMask);
		foreach(Collider c in col){
			EnemyScript es = c.GetComponent<EnemyScript>();
			if(es != null){	
				es.AlertMe();
			}
		}
	}
	
	/// <summary>
	/// Alerts the mob. The mob will gain aggroSearchRange search radius for 0.1 seconds.
	/// </summary>
	public void AlertMe(){
		CancelInvoke("TogglePassiveSearchRange");
		searchRange = aggroSearchRange;
		Invoke("TogglePassiveSearchRange", .25f);
	}
	
	private void TogglePassiveSearchRange(){
		if(currentAI != aggroAI){
			searchRange = passiveSearchRange;
		}
	}
	
	private void TryAttack(){
		EnemyType et = GetComponent<EnemyType>();
		if(et!=null){
			et.Attack();
			AttackPause(et.attackRate);
		}
	}
	
	void FindAIInput(){
		// Handle whether the AI is at aggroAI, passiveAI, or FrozenAI
		if(!stopAI){

			// Ignore aggro toggle logic if passive and aggro AI are the same
			if( passiveAI == aggroAI && passiveAI == currentAI){}

			// Handle toggling passive to aggro AI state
			else if( currentAI == passiveAI || currentAI == AI.IdleAI ){
				if( Vector3.Distance(ps.GetVisiblePosition(), transform.position) <= searchRange ){
					TurnAggroAI();
				}
			}

			// Handle toggling attack to aggro AI state
			else if( currentAI == AI.AttackAI ){
				if( Vector3.Distance(ps.GetVisiblePosition(), transform.position) <= aggroSearchRange ){
					TurnAggroAI();
				}
			}

			// Handle toggling aggro/attack to passive AI state
			else if( currentAI == aggroAI || currentAI == AI.IdleAI || currentAI == AI.AttackAI){
				// Handle guarding/leashing
				if ( guardMode && Vector3.Distance(transform.position, leashPt) > searchRange ){
					currentAI = AI.TargetAI;
					target = leashPt;
					searchRange = 0;
				}
				// Handle logic for aggro to passive
				else if ( Vector3.Distance(ps.GetVisiblePosition(), transform.position) > searchRange ){
					currentAI = passiveAI;
					searchRange = passiveSearchRange;
				}
			}

			// Check if at target
			if( currentAI == AI.TargetAI ){
				if( Vector3.Distance(transform.position, target) < .35f ){
					currentAI = passiveAI;
					searchRange = passiveSearchRange;
				}
			}
			
		}
		
		switch (currentAI){
			case AI.ChaseAI:
				inputMovement = ChaseAI();
				break;
			case AI.HomingAI:
				inputMovement = HomingAI();
				break;
//			case AI.CircleAI:
//				inputMovement = CircleAI(circleRadius);
//				break;
//			case AI.SpiralAI:
//				inputMovement = SpiralAI(circleRadius);
//				break;
			case AI.RandomAI:
				inputMovement = RandomAI();
				break;
			case AI.FleeAI:
				inputMovement = FleeAI();
				break;
			case AI.IdleAI:
				inputMovement = IdleAI();
				break;
			case AI.CollectyAI: 
				inputMovement = CollectyAI(); 
				break; 
			case AI.PatternAI:
				inputMovement = PatternAI();
				break;
			case AI.PatrolAI:
				inputMovement = PatrolAI ();
				break;
			case AI.TurnAI:
				inputMovement = TurnAI();
				break;
			case AI.GridAI:
				inputMovement = GridAI();
				break;
			case AI.RandomLineAI:
				inputMovement = RandomLineAI(1f);
				break;
			case AI.TargetAI:
				inputMovement = TargetAI();
				break;
			case AI.FrozenAI:
				inputMovement = IdleAI();
				break;
			
			
		}
		
	}
	
//	private void generatePatternPoints() {
//		int minPts = 2;
//		int maxPts = 4;
//		float minDistance = 4.0f;
//		patternPoints = new Vector3[(int)Random.Range(0, maxPts - minPts + 1) + minPts];
//		for(int i = 0; i < patternPoints.Length; i++) {
//			// generate each point
//			patternPoints[i] = mapInfo.randomStaticallyUnoccupiedWorldPoint();
//			bool farEnoughPoint = true;
//			for (int j = 0; j < i; j++) {
//				if (Vector3.Distance(patternPoints[i], patternPoints[j]) < minDistance) {
//					farEnoughPoint = false;
//				}
//			}
//
//			if (farEnoughPoint == false) {
//				// nonstandard convention for for-loops
//				// basically, go back a "step" if too close to other generated points
//				i--;
//			}
//		}
//	}
	
	void ProcessMovement(){
		
		if(isFree == 1){
			inputMovement.y = 0;
		}
		
		CharacterController controller = GetComponent<CharacterController>();
		inputMovement = transform.TransformDirection(inputMovement);
		
		// Handle external forces
		externalDirection[1] = 0; 
		if(externalDirection == Vector3.zero){
			finalMovement = inputMovement.normalized * Time.deltaTime * moveSpeed * -1 * isFree * attackPause * moveSpeedMultiplier;
			controller.Move(finalMovement);
		}
		else{
			controller.Move(externalDirection * Time.deltaTime * 4);
		}
		
		// Lock the object to y = 0
		transform.position = new Vector3(transform.position.x,0,transform.position.z);
		
		// Handle rotation when thrown
		if(isRotating && gameObject.layer != 17){	// 17 is the layer for picked up enemies
			transform.Rotate(0,rotateDegrees*(1/rotateDur)*Time.deltaTime,0);
		}
		else{
			transform.eulerAngles = new Vector3(0,180,0);	//keeps the object from rotating TODO: solve the UV mapping so things aren't all rotated
		}
	}
	
	public Vector3 GetFacingVector(){
		if(currentAI != AI.AttackAI){
			return finalMovement;
		}
		else{
			return ps.GetVisiblePosition() - transform.position;
		}
	}
	
	public Vector3 GetMovementVector(){
		return finalMovement;
	}
	
	
	
	
	/** 
	 * AI 
	 **/
	
	
	
	
	
	/* Chases the player using A* */
	Vector3 HomingAI(){
		AItarget = AStar.Find(transform.position, ps.GetVisiblePosition(), mapInfo);
		return AItarget - transform.position;
	}
	
	/* Chases the player without A* */
	Vector3 ChaseAI(){
		return ps.GetVisiblePosition() - transform.position;
	}
	
	/* Follows a pattern in a circle with radius 'r' in unknown units */
//	Vector3 CircleAI(float r){
//		
//		//Instantiate(obj, transform.position, Quaternion.identity);
//		
//		updatedVector = Quaternion.Euler(0,(1/r)*100*Time.deltaTime,0) * updatedVector;
//		return updatedVector;
//	}
	
	/* Moves in a spiral pattern of increasing radius until it hits a certain size in which it will decrease the spiral radius */
//	Vector3 SpiralAI(float r){
//		if(circleRadius >= 3){
//			isInc = false;
//		}
//		else if(circleRadius <= 1){
//			isInc = true;
//		}
//		if(isInc){
//			circleRadius = circleRadius + (.1f * Time.deltaTime) ;
//		}
//		else{
//			circleRadius = circleRadius - (.1f * Time.deltaTime) ;
//		}
//		updatedVector = Quaternion.Euler(0,(1/r)*3,0) * updatedVector;
//		return updatedVector;
//	}
	
	/* Goes around randomly (2% chance of changing direction) */
	Vector3 RandomAI(){
		if (Random.value > .98f){
			updatedVector = new Vector3(Random.Range(-10.0f,10.0f),0,Random.Range(-10.0f,10.0f));
			return updatedVector;
		}
		else
			return updatedVector;
	}
	
	/* Runs away from the player */
	Vector3 FleeAI(){
		AItarget = AStar.Flee(transform.position, ps.GetVisiblePosition(), mapInfo);
		return AItarget - transform.position;
	}
	
	/* Chases after powerups */
	Vector3 CollectyAI(){ 
		objPowerup = (GameObject) GameObject.FindWithTag("Powerup");
		if (objPowerup == null) return IdleAI();
		AItarget = AStar.Find(transform.position, objPowerup.transform.position, mapInfo); 
		return AItarget - transform.position; 
	} 
	
	/* Patrols between certain pattern points using A*. Can only handle int value coordinates */
	Vector3 PatternAI() {
		if (Vector3.Distance(mapInfo.GridToLevel(mapInfo.LevelToGrid(transform.position)), (patternPoints[patternIndex])) < .1f) {
			timerAI++;
			if(timerAI > .25/Time.deltaTime){ //Move to next point after .25 second
				timerAI = 0;
				patternIndex++;
			}
			if (patternIndex >= patternPoints.Length) {
				patternIndex = 0;
			}
		}
		AItarget = AStar.Find(transform.position, patternPoints[patternIndex], mapInfo);
		return AItarget - transform.position;
	}
	
	/* Similar to PatternAI but doesn't use A* */
	Vector3 PatrolAI() {
//		print (UtilityScript.CheckIfAtTarget(transform.position, patternPoints[patternIndex]));
		if (UtilityScript.CheckIfAtTarget(transform.position, patternPoints[patternIndex])) {
			timerAI++;
			if(timerAI > .5f/Time.deltaTime){ //Move to next point after .5 second
				timerAI = 0;
				patternIndex++;
			}
			else{
				return Vector3.zero;
			}
			if (patternIndex >= patternPoints.Length) {
				patternIndex = 0;
			}
		}
		return patternPoints[patternIndex] - transform.position;
	}
	
	/* Moves in a straight line until it hits a wall. Turn right each time it hits a wall */
	Vector3 TurnAI(){
		Vector3 temp;
		temp = transform.position + turnVec;
		if(mapInfo.isOccupiedBy(temp, 1) ){	// Checks if the position in front is occupied (1 to only check for walls)
			timerAI++;
			if(timerAI > 1/Time.deltaTime){	// Change direction after 1 second
				timerAI = 0;
				turnVec = Quaternion.Euler(0,90,0) * turnVec;	// Rotate the moving vector by 90 degrees
			}
		}
		return turnVec;
	}
	
	/* Moves up, down, left or right one grid point every 1-3 seconds */
	Vector3 GridAI(){
		if(timerAI > moveTimer/Time.deltaTime){	// Set a 3-4 second timer until it can move again
			moveTimer = Random.Range(3,5);
			timerAI = 0;
			Vector3 offset = Vector3.right;
			offset = Quaternion.Euler(0,90*Random.Range(0,4),0) * offset;	// Randomly rotate the offset by 90x degrees
			
			int loopCt = 0;	// Counter so the loop breaks eventually if there's no where to go
			while(mapInfo.isOccupiedBy(transform.position + offset, 2)){	// Check if location is blocked; if it is, find another one
				offset = Quaternion.Euler(0,90*Random.Range(0,4),0) * offset;
				if(loopCt > 50){
					break;
				}
				loopCt++;
			}
			gridVec = transform.position + offset;
			gridVec[0] = Mathf.RoundToInt(gridVec[0]);	// Snap the point to the nearest int coordinates
			gridVec[2] = Mathf.RoundToInt(gridVec[2]);
		}
		timerAI++;
		if(Vector3.Distance(gridVec, transform.position) > .025f){	// If it's close enough to the target point then stop (reduces stuttering)
			return gridVec - transform.position;	
		}
		else{
			return Vector3.zero;
		}
	}
	
	/* Moves in a random direction for 1 second then turns a random 90 degree direction */
	Vector3 RandomLineAI(float t){
		if(timerAI > t/Time.deltaTime){
			timerAI = 0;
			turnVec = Quaternion.Euler(0,90*Random.Range(0,4),0) * turnVec;
		}
		timerAI++;
		return turnVec;
	}
	
	/* Moves towards the target using pathing. Defaults to Vector3.zero if target is unset */
	Vector3 TargetAI(){
		if(Vector3.Distance(target,transform.position) < 1.0f){
			return target - transform.position;
		}
		else{
			AItarget = AStar.Find(transform.position, target, mapInfo);
			return AItarget - transform.position;
		}
	}
	
	/* Do nothing */
	Vector3 IdleAI(){
		return new Vector3(0,0,0);
	}
	
	
	
	
	/**
	 * Other methods
	 **/
	
	
	
	
	
	public Vector3 GetInputMovement(){
		return inputMovement;
	}
	
	/// <summary>
	/// Rotates the entity.
	/// </summary>
	/// <param name='dur'>
	/// Duration of rotation.
	/// </param>
	/// <param name='degrees'>
	/// Degrees rotated.
	/// </param>
	public void RotateMe(float dur, float degrees){
		isRotating = true;
		rotateDur = dur;
		rotateDegrees = degrees;
		Invoke("StopRotating", dur);
	}
	
	public void StopRotating(){
		isRotating = false;
	}
	
	/// <summary> 
	/// Alters movement speed by a multiplier 
	/// </summary>
	public void SlowDown(float multiplier, float duration){
		if(!slowActive){
			slowActive = true;
			moveSpeedMultiplier = multiplier;
			Invoke("SlowDownOff", duration);
		}
	}
	
	private void SlowDownOff(){
		slowActive = false;
		moveSpeedMultiplier = 1f;
		CancelInvoke("SlowDownOff");
	}	
	
	public void AttackPause(float duration){
		if(attackPause == 1){
			attackPause = 0;
			Invoke("AttackPauseOff", duration);
		}
	}
	
	private void AttackPauseOff(){
		attackPause = 1;
		CancelInvoke("AttackPauseOff");
	}
	
	/// <summary>
	/// Pause/stun the entity. Damage and EnemyType scripts are disabled when paused
	/// </summary>
	public void Pause() {
		CancelInvoke("Unpause"); // cancel any existing unpause-in-wait
		isFree = 0;
	}
	
	/// <summary>
	/// Pause/stun the entity. Damage and EnemyType scripts are disabled when paused
	/// </summary>
	public void Pause(float len) {	
		Pause();
		Invoke("Unpause", len); // unpause after len seconds
	}
	
	/// <summary>
	/// Unpause/stun the entity. Damage and EnemyType scripts are reenabled when unpaused
	/// </summary>
	public void Unpause() {	
		isFree = 1;
		transform.Find("Plane").GetComponent<Renderer>().material.color = Color.white;
	}
	
	/// <summary>
	/// Sets the external direction of the entity. Entity is not stunned when pushed
	/// </summary>
	public void SetExternalDirection(Vector3 v, float duration) {
		CancelInvoke("clearExternalDirection"); // cancel any ext dir waiting to be cleared
 		//Pause(3); 
		stopAI = true;
		//currentAI = AI.IdleAI;
		inputMovement = Vector3.zero;
 		externalDirection = v;
		Invoke("ClearExternalDirection", duration);
	}
	
	private void ClearExternalDirection() {
		externalDirection = Vector3.zero;
		stopAI = false;
	}
	
//	public void ShowReticle(bool b){
//		Transform reticle = transform.Find("Reticle");
//		if(reticle != null){
//			reticle.renderer.enabled = b;
//		}
//	}
	
	public void RemoveMe(){
//		MonoBehaviour[] temp = GetComponents<MonoBehaviour>();
//		foreach(MonoBehaviour x in temp){
//			x.enabled = false;
//		}
////		foreach(Transform child in transform){
////			MeshRenderer childRender = child.GetComponent<MeshRenderer>();
////			if(childRender != null)
////				childRender.enabled = false;
////		}
//		GetComponent<CharacterController>().enabled = false;
		CancelInvoke();
		moveSpeed = 0f;
		currentAI = AI.IdleAI;
		aggroAI = currentAI;
		passiveAI = currentAI;
//		EnemyType et = GetComponent<EnemyType>();
//		if(et != null){
//			et.OnDeath();
//		}
//		gameObject.AddComponent<ColourTransitionScript>().SetUp(Color.clear, .5f);
//		Destroy(gameObject, .5f);
//		EnemyList.Kill(gameObject);
	}
	
	public void SetSearchRange(float range){
		searchRange = range;
		passiveSearchRange = range;
		aggroSearchRange = range * srchRngeMultplr;
	}
	
//	public void KOMe(){
////		isKO = true;
//		isFree = 0;
//		moveSpeedMultiplier = 0;
////		transform.Find("Exclamation Prefab").renderer.enabled = false;
//	}
	
	static AI GetRandomAI<AI>(){
	    System.Array A = System.Enum.GetValues(typeof(AI));
	    AI V = (AI)A.GetValue(UnityEngine.Random.Range(0,A.Length));
	    return V;
	}
	
	void OnDestroy() {
		//print(InitializationScript.destroyedEnemy);
	}
	
	void OnDrawGizmos(){
		if(gizmoToggle){
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, searchRange);
		}
	}
}
