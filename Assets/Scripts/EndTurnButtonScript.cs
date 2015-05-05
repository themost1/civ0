using UnityEngine;
using System.Collections;

public class EndTurnButtonScript : MonoBehaviour {

	public void endTurn(){
		if(Gameplay.currentPlayer == 1){
			Gameplay.currentPlayer = 2;
			Gameplay.powerPoints = 5;
		}
		else{
			Gameplay.currentPlayer = 1;
			Gameplay.powerPoints = 5;
		}
	}
}