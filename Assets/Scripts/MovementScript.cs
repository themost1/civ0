using UnityEngine;
using System.Collections;

public enum MovementDirection : byte
{
	RIGHT = 0,
	LEFT,
	UPLEFT,
	UPRIGHT,
	DOWNLEFT,
	DOWNRIGHT
}

public class MovementScript : MonoBehaviour {
	// Use this for initialization
	float totalMoved;
	MovementDirection direction;
	bool moving;
	float xDist = Mathf.Sqrt(3)/2f,zDist = 1.5f;

	public bool test=false;
	GameObject theTank; //The currently selected gameobject

	void Start () {
		totalMoved = 0;
		direction = MovementDirection.DOWNRIGHT;
		moving = false;
	}

	//Read key press to move selected tank
	void Update () {
		if (!moving){
			theTank = Gameplay.selected;
			
			if (Input.GetKey (KeyCode.Q))
				beginMoving(MovementDirection.UPLEFT);	//Move to the adjacent hex up and to the left
			else if (Input.GetKey (KeyCode.E))
				beginMoving(MovementDirection.UPRIGHT);	//Move up and right
			else if(Input.GetKey(KeyCode.A))
				beginMoving(MovementDirection.LEFT);	//Move left
			else if(Input.GetKey(KeyCode.D))
				beginMoving(MovementDirection.RIGHT);	//Move right
			else if(Input.GetKey(KeyCode.Z))
				beginMoving(MovementDirection.DOWNLEFT);	//Move down and left
			else if(Input.GetKey(KeyCode.C))
				beginMoving(MovementDirection.DOWNRIGHT);	//Move down and right
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

	void beginMoving(MovementDirection dir) {
		direction = dir;
		moving = true;

		if(theTank!=null){
			float dt = Time.deltaTime;
			if(dir == MovementDirection.UPLEFT){
				theTank.transform.Translate(-xDist*dt,0,zDist*dt,Space.World);
				totalMoved += xDist*dt + zDist*dt;

				if(totalMoved >= xDist + zDist)
					moving = false;
			}
			else if(dir == MovementDirection.UPRIGHT){
				theTank.transform.Translate(xDist*dt,0,zDist*dt,Space.World);
				totalMoved += xDist*dt + zDist*dt;

				if(totalMoved >= xDist + zDist)
					moving = false;
			}
			else if(dir == MovementDirection.LEFT){
				theTank.transform.Translate(-xDist*2f*dt,0,0,Space.World);
				totalMoved += xDist*2f*dt;

				if(totalMoved >= xDist*2f)
					moving = false;
			}
			else if(dir == MovementDirection.RIGHT){
				theTank.transform.Translate(xDist*2f*dt,0,0,Space.World);
				totalMoved += xDist*2f*dt;

				if(totalMoved >= xDist*2f)
					moving = false;
			}
			else if(dir == MovementDirection.DOWNLEFT){
				theTank.transform.Translate(-xDist*dt,0,-zDist*dt,Space.World);
				totalMoved += xDist*dt + zDist*dt;

				if(totalMoved >= xDist + zDist)
					moving = false;
			}
			else if(dir == MovementDirection.DOWNRIGHT){
				theTank.transform.Translate(xDist*dt,0,-zDist*dt,Space.World);
				totalMoved += xDist*dt + zDist*dt;

				if(totalMoved >= xDist + zDist)
					moving = false;
			}
		}
		else
			moving = false;

		//right and left must move total of Mathf.Sqrt(3)
		//other directions must move total of 1.5+Mathf.Sqrt(3)

		if(!moving){
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
						
			SelectUnit.clickedHex = theHex;
			SelectUnit.clickedHex.full = true;
			SelectUnit.clickedHex.clicked = true;
			SelectUnit.clickedHex.Start();
			SelectUnit.clickedHex.parentChunk.Combine();
		}
	}
}