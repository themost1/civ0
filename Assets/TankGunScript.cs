using UnityEngine;
using System.Collections;

public class TankGunScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.I)) {
			transform.Rotate(Vector3.left*Time.deltaTime*100);
		} else if (Input.GetKey (KeyCode.K)) {
			transform.Rotate(Vector3.right*Time.deltaTime*100);
		}
	}
}
