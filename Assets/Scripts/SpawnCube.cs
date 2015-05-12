using UnityEngine;
using System.Collections;

public class SpawnCube : MonoBehaviour {
	public GameObject Cube;

	public void spawnCube(){
		Vector3 vec = SelectUnit.clickedHex.worldPosition;
		vec.y+=0.5f;
		Instantiate(Cube,vec,new Quaternion(0,0,0,0));
	}
}