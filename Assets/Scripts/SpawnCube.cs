using UnityEngine;
using System.Collections;

public class SpawnCube : MonoBehaviour {
	public GameObject Cube;

	public void spawnCube(){
		if(Gameplay.powerPoints>1 && !SelectUnit.clickedHex.full){
			Vector3 vec = SelectUnit.clickedHex.worldPosition;
			vec.y+=0.5f;
			GameObject theCube = (GameObject)Instantiate(Cube,vec,new Quaternion(0,0,0,0));
				
			SelectUnit.clickedHex.full=true;
			Gameplay.powerPoints-=2;
		}
	}
}