using UnityEngine;
using System.Collections;

public class SpawnerScript : MonoBehaviour {
	
	public GameObject[] spawnArray;
	private int num = 0;
	private bool toggle = false;
	public float detectionRange = 10; // Distance to detect player for triggering spawning
	public int mobLimit = 50; // Limit of alive mobs in the whole level 
	public float spawnTimer = 15; // Time it takes to start spawning
	
	void Start () {
		InvokeRepeating("TrySpawn", 1, 1);
	}

	void Update(){
		AnimationScript.AnimateSprite(GetComponent<Renderer>(),4,1,0,0,4,6);
	}
	
	private void TrySpawn(){
		int temp = UtilityScript.GetPlayersInRange(transform.position, detectionRange).Count;
		if(temp > num){ // Make num only able to increase
			num = temp;
		}
		if(num > 0 && !toggle){
			InvokeRepeating("Spawn1", 1, spawnTimer);
			toggle = true;
		}
	}
	
	// This is here because I want InvokeRepeating and WaitForSeconds
	private void Spawn1(){
		StartCoroutine(Spawn2());
	}
	
	private IEnumerator Spawn2(){
		if(num > 0){
			int i = 0;
			while(i < num){
				if(EnemyList.NumAlive() < mobLimit){
					int x = Random.Range(0,spawnArray.Length);	// even % to spawn one or the other
					GameObject temp = (GameObject) Instantiate(spawnArray[x], transform.position, Quaternion.identity);
					EnemyScript es = temp.GetComponent<EnemyScript>();
					if(es != null){
						es.passiveAI = es.aggroAI;
						es.currentAI = es.aggroAI;
					}
					// Add fancy effect
					GameObject skullEffect = (GameObject) GUIManager.FloatingTexture(VariableScript.matSkull, UtilityScript.MoveY(temp.transform.position,2), Vector3.one, 1.5f);
					skullEffect.AddComponent<TransformTransitionScript>().SetUp(Vector3.one, Vector3.one*2, 1.5f);
					skullEffect.AddComponent<ColourTransitionScript>().SetUp(new Color(.35f,.35f,.35f,1), Color.clear, 1.5f);
					yield return new WaitForSeconds(1);
					i++;
				}
				else{
					break;
				}
			}
		}
	}
}
