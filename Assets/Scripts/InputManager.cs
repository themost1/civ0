using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(Input.GetKey("2"))
			Camera.main.GetComponent<SpawnCube>().spawnCube();
		if(Input.GetKey (KeyCode.R)){
			restart();
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
	
	void restart(){
		Gameplay.powerPoints=10;
		Gameplay.currentPlayer=1;
		Gameplay.gameOver=false;
		Gameplay.selected=null;
		Gameplay.turns=1;
		Gameplay.vehicles=new ArrayList();
		Gameplay.tankCost=0;
		Gameplay.wallCost=0;
		WorldManager.hexChunks=null;
		SelectUnit.clickedHex=null;
	}	
}