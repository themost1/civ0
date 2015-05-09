using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {
	public int player;
	public float health=100;
	public float barrelRotation=0;
	public GameObject Explosion;
	
	// Use this for initialization
	void Start () {
		//Explosion.GetComponent<ParticleSystemMultiplier>().multiplier = 0.4f;
	}
	
	public void explode(){		
		Instantiate(Explosion,transform.position,new Quaternion(0,0,0,0));
	}
}