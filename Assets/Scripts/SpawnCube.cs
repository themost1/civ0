using UnityEngine;
using System.Collections;

public class SpawnCube : MonoBehaviour {
	public GameObject Cube;

	public void spawnCube(){
		if (Gameplay.powerPoints >= Gameplay.wallCost && !SelectUnit.clickedHex.full && !SelectUnit.clickedHex.brimstone) {
			Vector3 vec = SelectUnit.clickedHex.worldPosition;
			vec.y += 0.5f;
			Instantiate (Cube, vec, new Quaternion (0, 0, 0, 0));
				
			SelectUnit.clickedHex.full = true;
			Gameplay.powerPoints -= Gameplay.wallCost;
		} else if (Gameplay.powerPoints < Gameplay.wallCost)
			Gameplay.warningText="Not enough powerpoints!";
		else if (SelectUnit.clickedHex.full)
			Gameplay.warningText="Hex is full!";
		else if (SelectUnit.clickedHex.brimstone)
			Gameplay.warningText="Can't spawn on brimstone!";
	}
}