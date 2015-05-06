using UnityEngine;
using System.Collections;

public class StartMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	void OnMouseDown(){
		Camera.main.GetComponent<CameraMovement> ().moveToGame=true;
	}   
}
