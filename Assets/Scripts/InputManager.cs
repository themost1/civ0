using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if(Input.GetKey("2"))
			Camera.main.GetComponent<SpawnCube>().spawnCube();
	}
}