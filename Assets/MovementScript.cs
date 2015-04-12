using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	// Use this for initialization
	float direction, totalMoved;
	bool moving;

	void Start () {
		totalMoved = 0;
		direction = 0;
		moving = false;
		//OnGUI ();
	}
	// Update is called once per frame
	void Update () {
		if (!moving) {
			if (Input.GetKey ("1")) {
				//transform.Translate(Vector3.right*Mathf.Sqrt(3));
				beginMoving (1);
			} else if (Input.GetKey ("2")) {
				//transform.Translate(Vector3.forward*(float)1.5);
				beginMoving (2);
			} else if (Input.GetKey (KeyCode.J)) {
				transform.Rotate(Vector3.down*Time.deltaTime*100);
			} else if (Input.GetKey (KeyCode.L)) {
				transform.Rotate(Vector3.up*Time.deltaTime*100);
			}
		} else {
			beginMoving (direction);
		}
	}
	void beginMoving(float dir) {
		direction = dir;
		moving = true;
		if (dir == 1) {
			transform.Translate (Time.deltaTime*Vector3.right*Mathf.Sqrt(3),Space.World);
			totalMoved+=Time.deltaTime*Mathf.Sqrt(3);
		}
		else if (dir == 2) {
			transform.Translate (Time.deltaTime*Vector3.forward*(float)1.5,Space.World);
			totalMoved+=Time.deltaTime*(float)1.5;
		}
		//right and left must move total of Mathf.Sqrt(3)
		//other directions must move total of 1.5+Mathf.Sqrt(3)
		if (dir == 1) {
			if (totalMoved>Mathf.Sqrt(3))
				moving=false;
		}
		else if (dir == 2) {
			if (totalMoved>1.5)
				moving=false;
		}
		if (!moving)
			totalMoved = 0;
	}
	void OnGUI(){
		GUI.Label(new Rect(transform.position.x+10, 10, 100, 20),"Hello World");
	}
}
