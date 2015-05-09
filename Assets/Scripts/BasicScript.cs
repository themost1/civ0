﻿using UnityEngine;
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
		if(col.gameObject.name == "Ball(Clone)")
		{
			col.gameObject.GetComponent<BallPosition>().explode();
			Destroy(col.gameObject);
			
			GetComponent<Vehicle>().health -= 40f;
			if(gameObject.GetComponent<Vehicle>().health <= 0f){
				gameObject.GetComponent<Vehicle>().explode();
				Gameplay.vehicles.Remove(gameObject);
				Destroy(gameObject);
			}
		}
	}
}
