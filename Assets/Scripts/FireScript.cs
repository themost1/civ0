using UnityEngine;
using System.Collections;

public class FireScript : MonoBehaviour {
	public GameObject ball;
	public float speed;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) && Gameplay.selected!=null && Gameplay.selected.Equals(transform.root.gameObject) && Gameplay.powerPoints > 0) {
			Gameplay.powerPoints--;
			GameObject cannonball;
			cannonball = (GameObject)Instantiate(ball, transform.position, transform.rotation);
			Vector3 dir = new Vector3(0,0,speed);
			cannonball.GetComponent<Rigidbody>().velocity = transform.TransformDirection(dir);
			GunShot.makeSound=true;
		}	
	}
}