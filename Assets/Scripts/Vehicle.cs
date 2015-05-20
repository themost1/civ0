using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {
	public int player;
	public float health=100;
	public float barrelRotation=0;
	public GameObject Explosion;
	
	void Update(){
		transform.Find ("HealthBar").gameObject.transform.localScale = new Vector3 (health/10, 
			transform.Find ("HealthBar").gameObject.transform.localScale.y, 
			transform.Find ("HealthBar").gameObject.transform.localScale.z);
	}
	public void explode(){
		Instantiate(Explosion,transform.position,new Quaternion(0,0,0,0));
	}
}