using UnityEngine;
using System.Collections;

public class EndTurnButtonPosition : MonoBehaviour {
	void Update () {
		transform.position = new Vector3(Screen.width - 90f,25f);
	}
}