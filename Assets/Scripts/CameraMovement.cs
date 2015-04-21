using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public float horizontalSpeed, verticalSpeed, zoomSpeed;
	void Start () {
//		horizontalSpeed = 4f;
//		verticalSpeed = 4f;
//		zoomSpeed=4f;
	}
	// Update is called once per frame
	void Update () {


		if (Input.GetKey(KeyCode.RightArrow)) {

			transform.Translate(Vector3.right*horizontalSpeed*Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.LeftArrow)) {
			transform.Translate(Vector3.left*horizontalSpeed*Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			Vector3 v = new Vector3(0,-1,-1/Mathf.Sqrt(3));
			transform.Translate (v*verticalSpeed*Time.deltaTime);
		}
		else if (Input.GetKey (KeyCode.UpArrow)) {
			Vector3 v = new Vector3(0,1, 1/Mathf.Sqrt (3));
			transform.Translate (v*verticalSpeed*Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.Quote)) {
			//Vector3 v = Vector3.forward;
			Vector3 v = new Vector3(0,-1/Mathf.Sqrt (3),1);
			transform.Translate (v*verticalSpeed*Time.deltaTime);
			
		}
		else if (Input.GetKey (KeyCode.Slash)) {
			//Vector3 v = Vector3.back;
			Vector3 v = new Vector3(0,1/Mathf.Sqrt (3),-1);
			transform.Translate (v*zoomSpeed*Time.deltaTime);
		}
	}
}