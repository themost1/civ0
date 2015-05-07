using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {
	public int player;
	public bool moved;
	public float health=100;
	public float barrelRotation=0;
	// Use this for initialization
	void Start () {
		moved = false;
	}
}