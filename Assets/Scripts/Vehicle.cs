using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {
	public int player;
	public float health=100;
	public float barrelRotation=0;
	public GameObject Explosion;
	
	public float barDisplay = 0;
	public Vector2 pos = new Vector2(20,40);
	public Vector2 size = new Vector2(60,20);
	public Texture2D progressBarEmpty;
	public Texture2D progressBarFull;
	
	void Start(){
		
	}
	
	void OnGUI()
	{
		
		// draw the background:
		/*GUI.BeginGroup (new Rect (pos.x, pos.y, size.x, size.y));
		GUI.Box (new Rect (pos.x, pos.y, size.x, size.y),progressBarEmpty);
		
		// draw the filled-in part:
		GUI.BeginGroup (new Rect (0, 0, size.x * barDisplay, size.y));
		GUI.Box (new Rect (5, 15, size.x, size.y),progressBarFull);
		GUI.EndGroup ();
		
		GUI.EndGroup ();*/
		
	} 
	
	void Update()
	{
		// for this example, the bar display is linked to the current time,
		// however you would set this value based on your desired display
		// eg, the loading progress, the player's health, or whatever.
		//barDisplay = Time.time * 0.05;
		transform.Find ("HealthBar").gameObject.transform.localScale = new Vector3 (health/10, transform.Find ("HealthBar").gameObject.transform.localScale.y, transform.Find ("HealthBar").gameObject.transform.localScale.z);
	}
	public void explode(){		
		//Explosion.GetComponent<ParticleSystemMultiplier>().multiplier = 0.4f;
		Instantiate(Explosion,transform.position,new Quaternion(0,0,0,0));
	}
}