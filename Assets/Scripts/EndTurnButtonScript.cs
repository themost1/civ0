using UnityEngine;
using System.Collections;

public class EndTurnButtonScript : MonoBehaviour {
	public int p1pp=0;
	public int p2pp=0;

	public void endTurn(){
		if(Gameplay.currentPlayer == 1){
			p1pp=Gameplay.powerPoints;
			Gameplay.currentPlayer = 2;
			Gameplay.powerPoints = p2pp+5;
		}
		else{
			p2pp=Gameplay.powerPoints;
			Gameplay.currentPlayer = 1;
			Gameplay.powerPoints = p1pp+5;
		}
	}
}