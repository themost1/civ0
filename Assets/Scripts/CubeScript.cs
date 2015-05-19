using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour {
	public GameObject Explosion;
	public float health = 80, initHeight=1.2f;
	
	public void OnCollisionEnter(Collision col){
		if(col.gameObject.name == "Ball(Clone)")
		{
			col.gameObject.GetComponent<BallPosition>().explode();
			Destroy(col.gameObject);
			
			health -= 40f;
			GunShot.makeExplosion=true;
			transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y*1/2,transform.localScale.z);
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