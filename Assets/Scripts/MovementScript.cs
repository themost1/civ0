using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	// Use this for initialization
	float totalMoved;
	Vector3 direction;
	bool moving;
	static float xDist = Mathf.Sqrt(3)/2f,zDist = 1.5f;
	
	Vector3 UPLEFT = new Vector3(-xDist,0,zDist),
	UPRIGHT = new Vector3(xDist,0,zDist),
	LEFT = new Vector3(-2f*xDist,0,0),
	RIGHT = new Vector3(2f*xDist,0,0),
	DOWNLEFT = new Vector3(-xDist,0,-zDist),
	DOWNRIGHT = new Vector3(xDist,0,-zDist);

	public bool test=false;
	GameObject theTank; //The currently selected gameobject

	void Start () {
		totalMoved = 0;
		direction = new Vector3(0,0,0);
		moving = false;
	}

	//Read key press to move selected tank
	void Update () {
		if (!moving){
			theTank = Gameplay.selected;
			
			if (Input.GetKey (KeyCode.Q))
				beginMoving(UPLEFT);	//Move to the adjacent hex up and to the left
			else if (Input.GetKey (KeyCode.E))
				beginMoving(UPRIGHT);	//Move up and right
			else if(Input.GetKey(KeyCode.A))
				beginMoving(LEFT);	//Move left
			else if(Input.GetKey(KeyCode.D))
				beginMoving(RIGHT);	//Move right
			else if(Input.GetKey(KeyCode.Z))
				beginMoving(DOWNLEFT);	//Move down and left
			else if(Input.GetKey(KeyCode.C))
				beginMoving(DOWNRIGHT);	//Move down and right
			else if(Input.GetKey(KeyCode.J))			
				theTank.transform.Rotate(Vector3.down*Time.deltaTime*69);
			else if(Input.GetKey(KeyCode.L))
				theTank.transform.Rotate(Vector3.up*Time.deltaTime*69);
			else if(Input.GetKey(KeyCode.I))
				theTank.transform.Find("MainGun").Rotate(Vector3.left*Time.deltaTime*69);
			else if(Input.GetKey(KeyCode.K))
				theTank.transform.Find("MainGun").Rotate(Vector3.right*Time.deltaTime*69);
		}
		else{
			beginMoving(direction);
		}
	}

	void beginMoving(Vector3 dir) {
		bool canMove = true;
		foreach(GameObject tnak in Gameplay.vehicles)
			if(!tnak.Equals(theTank) && Vector3.Distance(tnak.transform.position,theTank.transform.position+direction) < 1f)
				canMove = false;
				
		direction = dir;
		moving = true;

		if(theTank!=null && Gameplay.powerPoints > 0){
			float dt = Time.deltaTime;	
			
			if(canMove){
				theTank.transform.Translate(direction.x*dt,0,direction.z*dt,Space.World);
				totalMoved += (Mathf.Abs(direction.x) + Mathf.Abs(direction.z))*dt;

				if(totalMoved >= ((Mathf.Abs(direction.x) + Mathf.Abs(direction.z))*(1-dt))){
					moving = false;
					Gameplay.powerPoints--;
				}
			}
		}
		else
			moving = false;

		//right and left must move total of Mathf.Sqrt(3)
		//other directions must move total of 1.5+Mathf.Sqrt(3)

		if(!moving && theTank != null){
			totalMoved = 0;
			
			SelectUnit.clickedHex.full = false;
			SelectUnit.clickedHex.clicked = false;
			SelectUnit.clickedHex.Start();
			SelectUnit.clickedHex.parentChunk.Combine();
			
			HexInfo theHex = null;
			foreach(HexChunk chunk in WorldManager.hexChunks)
				foreach(HexInfo hex in chunk.hexArray)
					if(Vector3.Distance(theTank.transform.position,hex.worldPosition)<1f)
						theHex = hex;
			
			Vector3 XactPos = theHex.worldPosition;
			XactPos.y += 0.4f;
			
			theTank.transform.position = XactPos;
												
			SelectUnit.clickedHex = theHex;
			SelectUnit.clickedHex.full = true;
			SelectUnit.clickedHex.clicked = true;
			SelectUnit.clickedHex.Start();
			SelectUnit.clickedHex.parentChunk.Combine();
		}
	}
}