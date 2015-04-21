using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	// Use this for initialization
	float totalMoved;
	string direction;
	bool moving;
	public bool test=false;
	GameObject theTank; //The currently selected gameobject

	void Start () {
		totalMoved = 0;
		direction = "";
		moving = false;
		//OnGUI ();
	}

	//Read key press to move selected tank
	void Update () {
		theTank = Gameplay.selected;
		if (!moving)
			if (Input.GetKey (KeyCode.Q))
				beginMoving("ul");	//Move to the adjacent hex up and to the left
			else if (Input.GetKey (KeyCode.E))
				beginMoving("ur");	//Move up and right
			else if(Input.GetKey(KeyCode.A))
				beginMoving("l");	//Move left
			else if(Input.GetKey(KeyCode.D))
				beginMoving("r");	//Move right
			else if(Input.GetKey(KeyCode.Z))
				beginMoving("dl");	//Move down and left
			else if(Input.GetKey(KeyCode.C))
				beginMoving("dr");	//Move down and right
			else if (Input.GetKey (KeyCode.J))
				transform.Rotate(Vector3.down*Time.deltaTime*100);
			else if (Input.GetKey (KeyCode.L))
				transform.Rotate(Vector3.up*Time.deltaTime*100);
		else
			beginMoving(direction);
	}

	void beginMoving(string dir) {
		direction = dir;
		moving = true;
		if(theTank!=null)
			if(dir.Equals("ul")){
				theTank.transform.Translate(-Mathf.Sqrt(3)/2f,0,1.5f);
			}
			else if(dir.Equals("ur")){
				theTank.transform.Translate(Mathf.Sqrt(3)/2f,0,1.5f);
			}
			else if(dir.Equals("l")){
				theTank.transform.Translate(-Mathf.Sqrt(3),0,0);
			}
			else if(dir.Equals("r")){
				theTank.transform.Translate(Mathf.Sqrt(3),0,0);
			}
			else if(dir.Equals("dl")){
				theTank.transform.Translate(-Mathf.Sqrt(3)/2f,0,-1.5f);
			}
			else if(dir.Equals("dr")){
				theTank.transform.Translate(Mathf.Sqrt(3)/2f,0,-1.5f);
			}

		//right and left must move total of Mathf.Sqrt(3)
		//other directions must move total of 1.5+Mathf.Sqrt(3)
		
		moving = false;

		if (!moving) {
			if (Gameplay.currentPlayer==1)
				Gameplay.currentPlayer=2;
			else
				Gameplay.currentPlayer=1;
		}
	}
}