using UnityEngine;
using System.Collections;

public class TemporaryTitleScreen : MonoBehaviour {
	
	public Texture splashScreen;
	public string buttonText = "Play!";
	public int levelLoaded = 0;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), splashScreen);
		ShowNextButton();
	}
	
	private void ShowNextButton(){
		if(GUI.Button(new Rect(Screen.width - 150, Screen.height - 100, 100, 60), buttonText)){
			Application.LoadLevel(levelLoaded);
		}
	}
}
