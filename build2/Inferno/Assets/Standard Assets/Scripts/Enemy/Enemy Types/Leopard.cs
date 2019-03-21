using UnityEngine;
using System.Collections;

public class Leopard : EnemyType {
	
	private Color translucent;
	private Vector3 pounceVec;
	public bool isPouncing = false;
    public float[] pounceSetupTimes = { 1.5f };
	
	void Start(){		
		translucent = new Color(1,1,1,.25f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Handle texture flip depending on facing
		if( plane != null && (plane.localScale.x > 0 && es.GetFacingVector().x < 0) || (plane.localScale.x < 0 && es.GetFacingVector().x > 0) ){	
			plane.localScale = new Vector3(plane.localScale.x * -1, plane.localScale.y, plane.localScale.z);
		}
		if(es.currentAI == es.aggroAI && !isPouncing){
			StartCoroutine(PounceCoroutine());
		}
	}
	
	private IEnumerator PounceCoroutine() {
		isPouncing = true;
		yield return new WaitForSeconds(1.5f);
        foreach(float time in pounceSetupTimes)
        {
            yield return StartCoroutine(Pounce(time));
        }
        yield return new WaitForSeconds(.5f);
        es.Pause(.5f);
        yield return new WaitForSeconds(3f);

        if (es.currentAI == es.aggroAI)
			StartCoroutine(PounceCoroutine());
		else
			isPouncing = false;  
    }

    private IEnumerator Pounce(float setupTime)
    {
        gameObject.AddComponent<ColourTransitionScript>().SetUp(translucent, setupTime - .15f);
        //pounceVec = es.GetFacingVector().normalized;
        pounceVec = (VariableScript.scrPlayerScript1.GetVisiblePosition() - transform.position).normalized;

        

        gameObject.layer = LayerMask.NameToLayer("Projectile (Player)");    //only collides with wall, enemies
        es.SlowDown(0, setupTime + .25f);

        yield return StartCoroutine(CreatePounceGhostImg(setupTime));
        yield return new WaitForSeconds(setupTime + .15f);
        plane.GetComponent<Renderer>().material.color = Color.white;
        es.SetExternalDirection(pounceVec, .5f);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        yield return new WaitForSeconds(.5f);
    }

    private IEnumerator CreatePounceGhostImg(float setUpTime)
    {
        float speed = 5;
        Transform plane = transform.FindChild("Plane");
        Vector3 target = (VariableScript.scrPlayerScript1.GetVisiblePosition() - transform.position).normalized;
        target = transform.position + target;

        for (int i = 0; i < speed; i++)
        {
            GameObject ghostImg = GUIManager.FloatingTexture(plane.GetComponent<Renderer>().material, UtilityScript.MoveY(transform.position, 0), plane.localScale, setUpTime / (speed * 1.5f));
            ghostImg.GetComponent<Renderer>().material.color = new Color(1, 1, 1, .25f);
            ghostImg.GetComponent<TempScript>().target = target;
            ghostImg.GetComponent<TempScript>().speed = speed;
            yield return new WaitForSeconds(.05f);
        }
    }
}
