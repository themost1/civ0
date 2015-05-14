using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public float horizontalSpeed, verticalSpeed, zoomSpeed;
	public bool moveToGame=false;
	void Start () {
//		horizontalSpeed = 4f;
//		verticalSpeed = 4f;
//		zoomSpeed=4f;
		this.transform.position=new Vector3(-300f,10, -6.5f);
	}
	// Update is called once per frame
	void Update () {

		if (moveToGame) {
			Camera.main.transform.position+=new Vector3 ((8f-(-300f))*Time.deltaTime, (4f-10f)*Time.deltaTime, (-4f-(-6.5f))*Time.deltaTime);
			if (Camera.main.transform.position.x>8)
				moveToGame=false;
		}
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
			if (transform.position.y>2)
				transform.Translate (v*verticalSpeed*Time.deltaTime);
			
		}
		else if (Input.GetKey (KeyCode.Slash)) {
			//Vector3 v = Vector3.back;
			Vector3 v = new Vector3(0,1/Mathf.Sqrt (3),-1);
			transform.Translate (v*zoomSpeed*Time.deltaTime);
		}
		else if (Input.GetKey (KeyCode.LeftControl)) {
			//Vector3 v = Vector3.back;
			Vector3 v = new Vector3(0,1/Mathf.Sqrt (3),-1);
			transform.Rotate (Vector3.up*10*Mathf.Sqrt (3)*Time.deltaTime);
			transform.Rotate (Vector3.forward*1/Mathf.Sqrt (3)*Time.deltaTime);
		}
	}
}