using UnityEngine;
using System.Collections;

/// <summary>
/// Level Processing Script
/// </summary>
/// <exception cref='UnityException'>
/// Is thrown when level is missing boundary indicator objects
/// </exception>
public class MapScript : MonoBehaviour {
	
	private static bool PROCESS_DYNAMIC_INFO = true;
	
	private static LevelInfo mapInfo;
	private static bool processedFlag = false;
	
	// boundary object indicators
	private static string TL_BOUNDARY = "LevelBoundTL";
	private static string BR_BOUNDARY = "LevelBoundBR";
	private static string WALL_TAG = "Wall";
	private static string ENEMY_TAG = "Enemy";
//	private static string NET_TAG = "Trap";
	//private static string LOG_TAG = "SwingLog";
//	private static string POWERUP_TAG = "Powerup";
//	private static string TRAP_CACHE_TAG = "TrapCache";
	private static string PLAYER_TAG = "Player";
	
	public static int ENEMY_GROUP = 2;
//	public static int BASIC_NET_GROUP = 3;
//	public static int SWINGING_LOG_GROUP = 4;
//	public static int POWERUP_GROUP = 5;
//	public static int TRAP_CACHE_GROUP = 6;
	public static int PLAYER_GROUP = 7;
	

	// Use this for initialization
	void Start () {
		processedFlag = false;
		processMap();
	}
	
	void Awake() {
		processedFlag = false;
	}
	
	// Update is called once per frame
	void Update () {
		// here we may want to update the map with dynamic information at each step
		// Might be better to just InvokeRepeating if this turns out to be slow?
		processDynamicObjects();
	}
	
	/// <summary>
	/// Processes the map.
	/// </summary>
	/// <exception cref='UnityException'>
	/// Is thrown when level is missing boundary indicator objects
	/// </exception>
	public static void processMap() {
		if (processedFlag == true) return; // do nothing if already processed
		processedFlag = true;
		
		// verify that boundary object exists
		if (GameObject.Find(TL_BOUNDARY) == null ||
				GameObject.Find(BR_BOUNDARY) == null) {
			throw new UnityException("Missing 1 or more boundary indicator objects (add them): "+
					TL_BOUNDARY + "," + BR_BOUNDARY);
		}
		
		// first use the level boundary game objects
		Vector3 position;
		
		position = GameObject.Find(TL_BOUNDARY).transform.position;
		float minX = position.x;
		float maxY = position.z;
		position = GameObject.Find(BR_BOUNDARY).transform.position;
		float maxX = position.x;
		float minY = position.z;
		
		mapInfo = new LevelInfo(minX, maxX, minY, maxY);
		
		// go through each wall/obstacle
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(WALL_TAG)) {	
			mapInfo.addStaticObject(obj.transform.position, obj.GetComponent<Collider>().bounds.size);
		}
	}
	
	/// <summary>
	/// Processes the dynamic objects.
	/// </summary>
	public static void processDynamicObjects() {
		if (!PROCESS_DYNAMIC_INFO) return;
		
		processMap(); // must ensure mapInfo exists
		mapInfo.resetDynamic();
		// go through each enemy
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(ENEMY_TAG)) {	
			mapInfo.addDynamicObject(obj.transform.position, obj.GetComponent<Collider>().bounds.size, ENEMY_GROUP);
		}
		// basic net trap, log trap, powerups
//		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(NET_TAG)) {	
//			mapInfo.addDynamicObject(obj.transform.position, obj.collider.bounds.size, BASIC_NET_GROUP);
//		}
//		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(LOG_TAG)) {	
//			mapInfo.addDynamicObject(obj.transform.position, obj.collider.bounds.size, SWINGING_LOG_GROUP);
//		}
//		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(POWERUP_TAG)) {	
//			mapInfo.addDynamicObject(obj.transform.position, obj.collider.bounds.size, POWERUP_GROUP);
//		}
//		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(TRAP_CACHE_TAG)) {	
//			mapInfo.addDynamicObject(obj.transform.position, obj.collider.bounds.size, TRAP_CACHE_GROUP);
//		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(PLAYER_TAG)) {	
			mapInfo.addDynamicObject(obj.transform.position, obj.GetComponent<Collider>().bounds.size, PLAYER_GROUP);
		}
	}
	
	/// <summary>
	/// Gets the LevelInfo object generated for this level
	/// </summary>
	/// <returns>
	/// The LevelInfo object
	/// </returns>
	public static LevelInfo getMap() {
		processMap();
		processDynamicObjects();
		return mapInfo;
	}
	
	// for testing
//	public void OnGUI() {
//		GUI.Box(new Rect(5,5,200,300), mapInfo.ToASCII());
//	}

}