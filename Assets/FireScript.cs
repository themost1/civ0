using UnityEngine;
using System.Collections;

public class FireScript : MonoBehaviour {
	public GameObject ball;
	public float speed;
	bool fired=false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space) && !fired) {
			fired=true;
			GameObject cannonball;
			cannonball = (GameObject)Instantiate(ball, GameObject.Find("GunTip").GetComponent<Rigidbody>().position, transform.rotation);
			Vector3 dir = new Vector3(0,0,speed);
			cannonball.GetComponent<Rigidbody>().velocity = transform.TransformDirection(dir);
		}
		if (Input.GetKey(KeyCode.F) && fired) {
			fired=false;
		}
	}
}
