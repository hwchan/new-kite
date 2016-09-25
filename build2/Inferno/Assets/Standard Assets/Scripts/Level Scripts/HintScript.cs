using UnityEngine;
using System.Collections;

/**
 * Sets up a GUI message in the centre of the screen
 * Message will be whatever is in the messages array or the game object's name if the array is empty
 * Messages will be shown for 'duration' seconds before iterating to the next in the array
 **/

public class HintScript : MonoBehaviour {
	
	public float duration = 3;
	public int fontSize = 75;
	public bool destroyOnTrigger = false;
	public string[] messages;
	public GUIStyle hintStyle;
	private bool toggle = false;
	private int counter = 0;
	
	// Use this for initialization
	void Start () {
		hintStyle.fontSize = fontSize;
		hintStyle.normal.textColor = Color.white;
		hintStyle.alignment = TextAnchor.MiddleCenter;
		hintStyle.wordWrap = true;
		if (messages.Length == 0){
			messages = new string[1];
			messages[0] = gameObject.name;
		}
	}
	
	public void OnTriggerEnter(Collider col){
		if(col.tag == "Player" && !toggle){
			toggle = true;
			InvokeRepeating("SetGUIMessage", 0f, duration);
		}
	}

	public void TriggerMe(){
		toggle = true;
		InvokeRepeating("SetGUIMessage", 0f, duration);
	}

	private void SetGUIMessage(){
		if(counter >= messages.Length){
			CancelInvoke("SetGUIMessage");
			counter = 0;
			toggle = false;
			if(destroyOnTrigger){
				Destroy(gameObject);
			}
			return;
		}
		Rect rect = new Rect(Screen.width/4, Screen.height/4, Screen.width/2, 20);
		GUIManager.SetLabel(rect, messages[counter], hintStyle, duration, gameObject);
		counter++;
	}

}
