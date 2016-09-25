using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	private Camera enemyCamera;
	private Camera losCamera;
	private GameObject player1;
	private bool player1CameraLock = true;
	private float cameraShakeMagnitude = 0;
	
	// Use this for initialization
	void Start () {
		enemyCamera = transform.FindChild("EnemyCamera").GetComponent<Camera>();
		losCamera = transform.FindChild("LoS Camera").GetComponent<Camera>();
		player1 = VariableScript.objPlayer1;
		SetCameraSize(GetComponent<Camera>().orthographicSize);
	}

	void LateUpdate(){
		HandlePlayer1Camera();
	}
	
	public void SetCameraSize(float size){
		GetComponent<Camera>().orthographicSize = size;
		enemyCamera.orthographicSize = size;
		
		// To get the field of view, get the arctan of size * .1f (half of the angle)
		float fov = Mathf.Atan(size * .1f);
		// Convert the result into degrees
		fov = fov * Mathf.Rad2Deg;
		// Set the field of view
		losCamera.fieldOfView = fov * 2;
	}

	private void HandlePlayer1Camera(){
		if(player1CameraLock){
			transform.position = new Vector3(player1.transform.position.x, transform.position.y, player1.transform.position.z);
		}
		if(cameraShakeMagnitude > 0){
			float x_change = UnityEngine.Random.Range(cameraShakeMagnitude*-1, cameraShakeMagnitude);
			float z_change = UnityEngine.Random.Range(cameraShakeMagnitude*-1, cameraShakeMagnitude);
			transform.position = new Vector3 (player1.transform.position.x + x_change, transform.position.y, player1.transform.position.z + z_change);
		}
	}

	public void LockCamera(bool b){
		player1CameraLock = b;
	}

	public void ShakeCamera(float length, float magnitude){
		CancelInvoke("ResetCameraShakeMagnitude");
		Invoke("ResetCameraShakeMagnitude", length);
		cameraShakeMagnitude = magnitude;
	}

	public void ResetCameraShakeMagnitude(){
		cameraShakeMagnitude = 0;
	}

	public void ResetCamera(){
		CancelInvoke("ResetCameraShakeMagnitude");
		cameraShakeMagnitude = 0;
		player1CameraLock = true;
		transform.position = new Vector3(player1.transform.position.x, transform.position.y, player1.transform.position.z);
	}
}
