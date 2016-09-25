using UnityEngine;
using System.Collections;

// TODO: use ScriptableObjects
public abstract class AbilityScript : MonoBehaviour {
	
	public TargetMethod targetMethod;
	public float additionalChargeTime = 0;
	
	public GameObject[] projectilePrefabs;
	protected PlayerAbilityManager pam;
	protected PlayerShoot psh;
	protected PlayerStats pstat;
	private Texture buttonTexture;
	public Texture ButtonTexture{
		get{return buttonTexture;}
	}
	private float ready = 0;	// So players don't automatically pick up the item when dropping
	
	void Awake(){
		buttonTexture = GetComponent<Renderer>().material.mainTexture;
	}
	
	void OnEnable(){
		ready = Time.time + .1f;
	}
	
	void OnTriggerEnter(Collider col){
		if(col.tag == VariableScript.playersTag){
			pam = col.GetComponent<PlayerAbilityManager>();
			psh = col.GetComponent<PlayerShoot>();
			pstat = col.GetComponent<PlayerStats>();
			if(Time.time >= ready && pam != null){
				int index = pam.AddAbility(gameObject);	// Adds the ability and returns the index of the slot added into (else returns -1)
				if(index != -1){
					gameObject.SetActive(false);
					SetUp(index);
				}
			}
		}
	}
	
	/// <summary>
	/// Sets up the ability on trigger enter. (eg. copy projectile prefabs)
	/// </summary>
	/// <param name='index'>
	/// Index of the slot the ability was added in.
	/// </param>
	protected virtual void SetUp(int index){}
	
	/// <summary>
	/// Clean up for ability removal. (eg. removing passive item buffs)
	/// </summary>
	/// <param name='index'>
	/// Index of the slot the ability was added in.
	/// </param>
	public virtual void SetDown(int index){}
	
}
