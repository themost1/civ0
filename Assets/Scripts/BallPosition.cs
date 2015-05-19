using UnityEngine;
using System.Collections;

public class BallPosition : MonoBehaviour {
	public GameObject Explosion;
	//Delete the ball when it goes off the map
	void Update () {
		if(gameObject.transform.position.y < 0)
			Destroy(gameObject);
		else if(gameObject.transform.position.y <= 0.2f){
			explode();
			Destroy(gameObject);
		}
	}
	
	public void explode(){
		Instantiate(Explosion,gameObject.transform.position,new Quaternion(0,0,0,0));
	}
}