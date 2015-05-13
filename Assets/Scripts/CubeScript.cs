using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour {
	public GameObject Explosion;
	public float health = 80;
	
	public void OnCollisionEnter(Collision col){
		if(col.gameObject.name == "Ball(Clone)")
		{
			col.gameObject.GetComponent<BallPosition>().explode();
			Destroy(col.gameObject);
			
			health -= 40f;
			if(health <= 0f){
				explode();
				foreach(HexChunk chunk in WorldManager.hexChunks)
					foreach(HexInfo hex in chunk.hexArray)
						if(hex.worldPosition.x==gameObject.transform.position.x &&
						   hex.worldPosition.z==gameObject.transform.position.z)
							hex.full = false;
				Destroy(gameObject);
			}
		}
	}
	
	public void explode(){		
		Instantiate(Explosion,transform.position,new Quaternion(0,0,0,0));
	}
}