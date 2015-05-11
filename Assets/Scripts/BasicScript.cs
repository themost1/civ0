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
		if(col.gameObject.name == "Ball(Clone)")
		{
			col.gameObject.GetComponent<BallPosition>().explode();
			Destroy(col.gameObject);
			
			foreach(HexChunk chunk in WorldManager.hexChunks)
				foreach(HexInfo hex in chunk.hexArray)
					if(hex.worldPosition == gameObject.transform.position)
						hex.full = false;
			
			GetComponent<Vehicle>().health -= 40f;
			if(gameObject.GetComponent<Vehicle>().health <= 0f){
				gameObject.GetComponent<Vehicle>().explode();
				Gameplay.vehicles.Remove(gameObject);
				Destroy(gameObject);
			}
		}
	}
}
