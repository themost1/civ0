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
		if (Input.GetKey(KeyCode.Space) && !fired && Gameplay.selected.Equals(transform.root.gameObject) && !transform.root.GetComponent<Vehicle>().moved) {
			fired=true;
			GameObject cannonball;
			cannonball = (GameObject)Instantiate(ball, transform.position, transform.rotation);
			Vector3 dir = new Vector3(0,0,speed);
			cannonball.GetComponent<Rigidbody>().velocity = transform.TransformDirection(dir);
			transform.root.GetComponent<Vehicle>().moved=true;
		}
		if (Input.GetKey(KeyCode.F) && fired) {
			fired=false;
		}
	}
}
