using UnityEngine;
using System.Collections;

public class EndTurnButtonScript : MonoBehaviour {

	public void endTurn(){
		if(Gameplay.currentPlayer == 1)
			Gameplay.currentPlayer = 2;
		else
			Gameplay.currentPlayer = 1;
	}
}