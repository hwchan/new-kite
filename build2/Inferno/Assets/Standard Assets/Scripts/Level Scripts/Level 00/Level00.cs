using UnityEngine;
using System.Collections;

public class Level00 : MonoBehaviour {

    public GameObject wolfBoss;
    public GameObject[] wolves;
    public GameObject wolfBossRoom;

    private EnemyHealthScript _wolfBossHp;
    private bool _didWolfBossTrigger = false;
    private bool _isWolfBossFightStarted = false;
    private GameObject _zzzText;

	// Use this for initialization
	void Start () {
        _wolfBossHp = wolfBoss.GetComponent<EnemyHealthScript>();
        _zzzText = GUIManager.FloatingText("Zzz...", wolfBoss.transform.position, 0);
        foreach(GameObject wolf in wolves)
        {
            wolf.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {

        //print(Vector3.Distance(wolfBoss.transform.position, wolfBossRoom.transform.position));

	    if(!_didWolfBossTrigger && _wolfBossHp.curHp <= _wolfBossHp.maxHp * .9f)
        {
            Destroy(_zzzText);
            _didWolfBossTrigger = true;
            EnemyScript es = wolfBoss.GetComponent<EnemyScript>();
            es.aggroAI = AI.TargetAI;
            es.target = wolfBossRoom.transform.position;
        }
        else if(IsPlayerAndWolfBossInPosition())
        {
            Debug.Log("FIGHT START");
            StartCoroutine(StartWolfBossFight());
        }
    }

    private bool IsPlayerAndWolfBossInPosition()
    {
        return _didWolfBossTrigger 
            && !_isWolfBossFightStarted 
            && UtilityScript.CheckIfAtTarget(wolfBoss.transform.position, wolfBossRoom.transform.position)
            //&& Vector3.Distance(wolfBoss.transform.position, wolfBossRoom.transform.position) <= .5f
            && UtilityScript.IsObjectInLOS(VariableScript.scrPlayerStats1, wolfBoss);
    }

    private IEnumerator StartWolfBossFight()
    {
        _isWolfBossFightStarted = true;
        EnemyScript es = wolfBoss.GetComponent<EnemyScript>();
        es.aggroAI = AI.HomingAI;
        es.MoveSpeed = es.MoveSpeed / 2;
        foreach (GameObject wolf in wolves)
        {
            yield return StartCoroutine(SpawnWolfCoroutine(wolf));
        }        
    }

    private IEnumerator SpawnWolfCoroutine(GameObject wolf)
    {
        wolf.SetActive(true);
        wolf.AddComponent<ColourTransitionScript>().SetUp(Color.clear, Color.white, 1f);        
        yield return new WaitForSeconds(1f);
        wolf.GetComponent<EnemyScript>().aggroAI = AI.HomingAI;
    }

    

}
