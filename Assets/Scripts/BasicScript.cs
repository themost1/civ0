using UnityEngine;
using System.Collections;

public class BasicScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter (Collision col)
	{
		Debug.Log ("Hit");
		if(col.gameObject.name == "Ball(Clone)")
		{
			Debug.Log("Hit by ball");
			col.gameObject.GetComponent<BallPosition>().explode();
			Destroy(col.gameObject);
			Gameplay.vehicles.Remove(gameObject);
			Destroy(gameObject);
		}
	}
}
