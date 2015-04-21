using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {
	public int player;
	public bool moved;
	float health=100;
	// Use this for initialization
	void Start () {
		moved = false;
	}
}