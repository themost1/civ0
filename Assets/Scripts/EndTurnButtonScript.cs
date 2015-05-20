using UnityEngine;
using System.Collections;

public class EndTurnButtonScript : MonoBehaviour {
	public static int p1pp=0, p2pp=0;
	static bool firstTurn=true;

	public static void endTurn(){
		if(Gameplay.currentPlayer == 1){
			p1pp=Gameplay.powerPoints;
			Gameplay.currentPlayer = 2;
			if(firstTurn){
				Gameplay.powerPoints=10;
				firstTurn=false;
			}
			else
				Gameplay.powerPoints = p2pp+7;
		}
		else{
			p2pp=Gameplay.powerPoints;
			Gameplay.currentPlayer = 1;
			Gameplay.powerPoints = p1pp+7;
			Gameplay.turns++;
			Gameplay.calcCosts();
		}
	}
	public void endTurn2(){
		endTurn ();
	}
}