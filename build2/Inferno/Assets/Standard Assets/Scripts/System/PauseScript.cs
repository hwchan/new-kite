using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {
	
	Texture2D minimap;
	bool paused = false;
	public bool Paused{
		get{ return paused; }
	}
	float levelStartTime = 0.0f;
//	public Texture2D digitsSpriteSheet;
//	public Texture2D goTexture;
//	public Texture2D pauseImg;
	int mapScale = 0;
	
	public GUIStyle txt;
	
	private Texture2D[] legendColors;
	
	// Use this for initialization
	void Start () {
		levelStartTime = Time.realtimeSinceStartup;
		//digitsSpriteSheet = (Texture2D)GameObject.Find("Multiplier").renderer.material.GetTexture("_MainTex"); 
//		goTexture = (Texture2D)Resources.Load("countdown");
//		pauseImg = (Texture2D)Resources.Load("paused");
		
		// Setting timescale to 0 too early (in start, awake etc. is currently bugged. This is a workaround
		//Invoke ("InitialPause", Time.deltaTime);	
	}
	
	private void InitialPause(){
		Time.timeScale = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Pause"))
		{
			HandlePause();
		}
		if (Time.timeScale == 0.0f && !paused && realtimeSinceLevelStart() > 3.0f) {
			endCountdown();
		}
	}
	
//	void OnGUI(){
//		if (Time.timeScale	== 0.0f && paused){
//			Debug.Log("paused");
//			GUI.Label(new Rect(Screen.width*.5f, Screen.height*.3f, 60, 60), "PAUSED", txt);
//		}
//	}
	
	void OnGUI()
	{
		// Pause button fuctionality - calls up a text overlay. May want to move this to
		// another object?
		if (Time.timeScale	== 0.0f && paused)
		{
//			GUI.DrawTexture(new Rect((Screen.width-128*3)/2, 0, 128*3, 32*3), pauseImg);
			if(GUI.Button(new Rect((Screen.width - 2 * 110)/2 + 0,32*3,100,30), "Continue")) {
				//Application.LoadLevel("");
				HandlePause();
			}
			if(GUI.Button(new Rect((Screen.width - 2 * 110)/2 + 110,32*3,100,30), "Main Menu")) {
				Time.timeScale = 1.0f; // reset timescale, otherwise, replaying level will have game paused.
				Application.LoadLevel("Instructions");
			}
			
			// TODO: this section should be global/go in Start()
			// taken from LevelInfo.cs
			// make sure these two arrays are of equal length
//			Color[] legColors = { Color.yellow, Color.red};
//			string[] accompanyText = { "Player", "Enemy" };
//			
//			if (legendColors == null) {
//				
//				legendColors = new Texture2D[legColors.Length];
//				for (int i = 0; i < legColors.Length; i++) {
//					legendColors[i] = new Texture2D(1,1);
//					legendColors[i].SetPixel(0,0, legColors[i]);
//					legendColors[i].Apply();
//				}
//			}
			
			//int legendH = Mathf.Min(Screen.height - (32 * 3 + 30 + 10) - 20, minimap.height);
			//int legendPerH = (int)Mathf.Max(mapScale, 40);
//			int legendPerH = 40;
//			int legendH = legColors.Length * (legendPerH + 5) + 10;
			
			if (minimap != null) {
				GUI.Box(new Rect((Screen.width - minimap.width * mapScale - 200)/2, 32 * 3 + 30 + 10, minimap.width * mapScale+6, minimap.height * mapScale +6), "");
				GUI.DrawTexture(new Rect((Screen.width - minimap.width * mapScale - 200)/2+3, 32 * 3 + 30 + 10+3, minimap.width * mapScale, minimap.height * mapScale), minimap);
			}
			
//			GUI.Box(new Rect((Screen.width - minimap.width * mapScale - 200)/2 + minimap.width * mapScale + 10, 32 * 3 + 30 + 10, 200,
//				legendH), "");
//			GUI.BeginGroup(new Rect((Screen.width - minimap.width * mapScale - 200)/2 + minimap.width * mapScale + 10, 32 * 3 + 30 + 10, 200, 
//				legendH));
//			
//			for (int i = 0; i < legColors.Length; i++) {
//				GUI.DrawTexture(new Rect(5, 5 + (legendPerH + 5) * i + (legendPerH - 30) / 2, 30, 30), legendColors[i]);
//				GUI.Label(new Rect(5 + legendPerH, 15 + (legendPerH + 5) * i, 200 - 30 - 20, legendPerH), accompanyText[i]);
//			}
//			GUI.EndGroup();
			
			
		}
		
		if (Time.timeScale == 0.0f && !paused && realtimeSinceLevelStart() < 3.0f) {
			// countdown
			
		//	GUI.Label(new Rect((Screen.width)/2, (Screen.width)/2, 100, 100), ((int)realtimeSinceLevelStart() + 1).ToString());
			GUI.BeginGroup(new Rect((Screen.width - 256)/2, (Screen.height - 256 - 100)/2, 256, 256));
//			int remainingTime = 3 - (int)realtimeSinceLevelStart();
			//GUI.Label(new Rect(32*(-remainingTime), 0, 320, 64), digitsSpriteSheet);
//			GUI.DrawTexture(new Rect(256*(-remainingTime), 0, 2560, 256), digitsSpriteSheet);
			GUI.EndGroup();
			//GUI.Label(new Rect((Screen.width)/2, (Screen.width)/2, 100, 100), remainingTime.ToString());
		} else if (realtimeSinceLevelStart() < 4.0f) {
			GUI.BeginGroup(new Rect((Screen.width - 512)/2, (Screen.height - 256 - 100)/2, 512, 256));
			//GUI.Label(new Rect(0, 0, 32, 64), "GO!");
			
			
			
			// Got sick of waiting for 3 seconds/seeing "GO!" for 3 seconds
			//GUI.DrawTexture(new Rect(0, 0, 512, 256), goTexture);
			
			
			
			
			
			GUI.EndGroup();
		}
			
	}
	
	void HandlePause()
	{
		if (Time.timeScale	== 1.0f && !paused)
		{
			Time.timeScale = 0.0f;
			StartCoroutine("UpdateMiniMap");
			paused = true;
//			EnemyList.Print();
			Debug.Log("Number of enemies alive: " + EnemyList.NumAlive());
		}
		else if (paused)
		{
			Time.timeScale = 1.0f;
			paused = false;
		}
		
	}
	
	private float realtimeSinceLevelStart() {
		return Time.realtimeSinceStartup - levelStartTime;
	}
	
	private void endCountdown() {
		print(Time.realtimeSinceStartup);
		Time.timeScale = 1.0f;
	}
	
	void UpdateMiniMap() {
		Vector2 mapSize = MapScript.getMap().getSize();
		int availableWidth = Screen.width - 200 - 20 - 10; // 200 for legend
		int availableHeight = Screen.height -  (32 * 3 + 30 + 10 + 10 + 10); // for pause image + buttons
		int widthScale = availableWidth / (int)mapSize.x;
		int heightScale = availableHeight / (int)mapSize.y;
		mapScale = (int)Mathf.Min(widthScale, heightScale);
		minimap = MapScript.getMap().getScaledMapImage(1);
		minimap.filterMode = FilterMode.Point;
		
	}
	
}
