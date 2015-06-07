using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	private static float timer;
	private static float duration;
	public static GameObject firingObj;
	private static bool toggle = false;
	private static Rect rectangle;
	private static string message;
	private static Texture image;
	private static GUIStyle messageStyle;
	
	
	private PlayerScript ps;
	private PlayerHealthScript phs;
	private PlayerStats pe;
	private PlayerAbilityManager pam;
	public GUIStyle hpStyle;
	public GUIStyle attributeStyle;
	public GUIStyle statStyle;
	public GUIStyle buttonStyle;
	
	void Start(){
		ps = VariableScript.scrPlayerScript1;
		phs = VariableScript.scrPlayerHealthScript1;
		pe = VariableScript.scrPlayerExperience1;
		pam = ps.GetComponent<PlayerAbilityManager>();
		hpStyle.normal.textColor = Color.white;
		hpStyle.fontSize = 40;
		attributeStyle.normal.textColor = Color.white;
		attributeStyle.fontSize = 20;
		statStyle.normal.textColor = Color.white;
		statStyle.fontSize = 25;
	}
	
	void Update(){
		Timer();
		if (pe.attributePt > 0){
			if (Input.GetButtonDown("Skill up Power")){
				pe.UpgradeAttack(1);
				pe.attributePt--;
			}
			if (Input.GetButtonDown("Skill up Health")){
				pe.UpgradeHealth(1);
				pe.attributePt--;
			}
			if (Input.GetButtonDown("Skill up Speed")){
				pe.UpgradeSpeed(1);
				pe.attributePt--;
			}
			if (Input.GetButtonDown("Skill up Vision")){
				pe.UpgradeSight(1);
				pe.attributePt--;
			}
		}
	}
	
	void OnGUI(){
		if(toggle){
			GUI.Label( rectangle, message, messageStyle );	// Show a message on screen
			GUI.Label( rectangle, image, messageStyle );	// Show an image on screen
		}
		ShowStats();
		ShowAttributeButtons();
		ShowAbilities();
	}
	
	private void ShowStats(){
		GUI.Label( new Rect(20,30,100,100), "HP: " + phs.health + "/" + phs.MAX_HP, hpStyle);
		GUI.Label( new Rect(20,90,100,100), "LVL " + pe.Level + ": " + pe.CurrentExp + "/" + pe.ExperienceToLevel, statStyle);
		GUI.Label( new Rect(20,120,100,100), "Lives: " + phs.lives, statStyle);
	}
	
	private void ShowAbilities(){
		string[] hotkeys = {"H","J","K","L"};
		for(int i=0; i<pam.abilityLimit; i++){
			if(pam.abilityList[i] != null){
				// Show buttons and drop functionality
				if (GUI.Button(new Rect(2*Screen.width/4 - 120+60*(i), Screen.height-80,60,60), pam.GetAbilityTexture(i)) 
						|| (Camera.main.GetComponent<PauseScript>().Paused && Input.GetButton( "Ability" + (i+1) ))){
					pam.DropAbilityAsItem(i);
				}
				// Show hotkey if ability is not passive
				if(pam.GetAbilityScript(i) != null && pam.GetAbilityScript(i).GetType() == typeof(SpellAbility)){
					GUI.Label(new Rect(2*Screen.width/4 - 120+60*(i), Screen.height-80,60,60), hotkeys[i]);
				}
			}
		}
		
	}
	
	
	
	private void ShowAttributeButtons(){

		GUI.Label( new Rect(Screen.width - 200,28,40,40), pam.attackTexture);
		GUI.Label( new Rect(Screen.width - 200,68,40,40), pe.UpgradeAttackDamage.ToString(), attributeStyle);
		
		GUI.Label( new Rect(Screen.width - 155,28,40,40), pam.healthTexture);
		GUI.Label( new Rect(Screen.width - 155,68,40,40), pe.UpgradeHealthPoints.ToString(), attributeStyle);
		
		GUI.Label( new Rect(Screen.width - 110,28,40,40), pam.speedTexture);
		GUI.Label( new Rect(Screen.width - 110,68,40,40), pe.UpgradeMoveAndAttackSpeed.ToString(), attributeStyle);
		
		GUI.Label( new Rect(Screen.width - 65 ,28,40,40), pam.sightTexture);
		GUI.Label( new Rect(Screen.width - 65 ,68,40,40), pe.UpgradeSightAndRange.ToString(), attributeStyle);
		
		if (pe.attributePt > 0){

			GUI.Label( new Rect(Screen.width - 208,20,200,85), VariableScript.texAttribute);

			if (GUI.Button(new Rect(Screen.width - 200,65,33,29), "+\t")){
				pe.UpgradeAttack(1);
				pe.attributePt--;
			}
			if (GUI.Button(new Rect(Screen.width - 155,65,33,29), "+\t")){
				pe.UpgradeHealth(1);
				pe.attributePt--;
			}
			if (GUI.Button(new Rect(Screen.width - 110,65,33,29), "+\t")){
				pe.UpgradeSpeed(1);
				pe.attributePt--;
			}
			if (GUI.Button(new Rect(Screen.width - 65,65,33,29), "+\t")){
				pe.UpgradeSight(1);
				pe.attributePt--;
			}
		}
	}
	
	
	
	/// <summary>
	/// Sets a GUI.Label object
	/// </summary>
	/// <param name='dur'>
	/// Duration of label
	/// </param>
	/// <param name='obj'>
	/// Reference of firing game object
	/// </param>
	public static void SetLabel(Rect rect, string str, GUIStyle style, float dur, GameObject obj){
		rectangle = rect;
		message = str;
		messageStyle = style;
		toggle = true;
		duration = dur;
		firingObj = obj;
		timer = 0;
	}
	
	public static void SetLabel(Rect rect, string str, float dur){
		messageStyle = new GUIStyle();
		messageStyle.fontSize = 35;
		messageStyle.normal.textColor = Color.white;
		messageStyle.alignment = TextAnchor.MiddleCenter;
		messageStyle.wordWrap = true;
		messageStyle.fontStyle = FontStyle.Bold;
		rectangle = rect;
		message = str;
		toggle = true;
		duration = dur;
		timer = 0;
	}

	public static void SetTexture(Rect rect, Texture img, float dur){
		messageStyle = new GUIStyle();
		messageStyle.fontSize = 35;
		messageStyle.normal.textColor = Color.white;
		messageStyle.alignment = TextAnchor.MiddleCenter;
		messageStyle.wordWrap = true;
		messageStyle.fontStyle = FontStyle.Bold;
		rectangle = rect;
		image = img;
		toggle = true;
		duration = dur;
		timer = 0;
	}
	
	private static void Timer(){
		timer++;
		if(timer > duration/Time.deltaTime){
			timer = 0;
			toggle = false;
			firingObj = null;
		}
	}
	
	// TODO: Move this to UtilityScript or something
	public static GameObject FloatingText(string text, Vector3 position, float duration){
		GameObject textObject = (GameObject) Instantiate(VariableScript.objText, position, Quaternion.Euler(90,0,0));
		textObject.GetComponent<TextMesh>().text = text;
		textObject.GetComponent<ExpiryScript>().SetExpiry(duration);
		return textObject;
	}
	
	public static GameObject FloatingTexture(Material material, Vector3 position, Vector3 scale, float duration){
		GameObject textureObject = (GameObject) Instantiate(VariableScript.objTemp, position, Quaternion.Euler(0,180,0));
		textureObject.transform.localScale = scale;
		textureObject.GetComponent<Renderer>().material = material;
		textureObject.GetComponent<ExpiryScript>().SetExpiry(duration);
		return textureObject;
	}

	public static GameObject FloatingTexture(Material material, Vector3 position, float duration, 
	                                         int colSize, int rowSize, int rowFrameStart, int colFrameStart, int totalFrames, int fps){
		GameObject textureObject = (GameObject) Instantiate(VariableScript.objTemp, position, Quaternion.Euler(0,180,0));
		textureObject.GetComponent<Renderer>().material = material;
		textureObject.GetComponent<ExpiryScript>().SetExpiry(duration);
		textureObject.GetComponent<ExpiryScript>().StartAnimation(colSize,rowSize,rowFrameStart,colFrameStart,totalFrames,fps);
		return textureObject;
	}
	
}
