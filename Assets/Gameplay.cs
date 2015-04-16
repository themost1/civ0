using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {
	public static float cooldown=0, currentPlayer=1, turn;
	public GameObject tank;
	public static GameObject selected;
	public int vNum;
	public ArrayList vehicles = new ArrayList();
	// Use this for initialization
	void Start () {
		cooldown = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (cooldown>0)
			cooldown += Time.deltaTime;
		if (Input.GetKey ("6") && currentPlayer==1) {
			currentPlayer=1;
			GameObject vehicle;
			vehicle = (GameObject)Instantiate(tank, new Vector3(0,0.3f,1.5f*turn), new Quaternion(0,0,0,0));
			vehicles.Add (vehicle);
			turn++;
			currentPlayer=2;
		} else if (Input.GetKey ("5") && currentPlayer==2) {
			currentPlayer=1;
			GameObject vehicle;
			vehicle = (GameObject)Instantiate(tank, new Vector3(1.7f,0.3f,1.5f*turn), new Quaternion(0,0,0,0));
			vehicles.Add (vehicle);
		}
		selectVehicle ();
	}
	void selectVehicle() {
		//Debug.Log (vNum);
		if (selected==null && cooldown>0.3)
			selected=(GameObject)vehicles[0];
		if (selected.GetComponent<Vehicle> ().moved && cooldown == 0)
			cooldown = 0.1f;
		else if (selected.GetComponent<Vehicle> ().moved && cooldown>0.3) {
			selected=(GameObject)vehicles[vNum];
			cooldown=0f;
			vNum++;
		}
		GameObject finalVehicle = (GameObject)vehicles [vehicles.Count - 1];
		if (vNum == vehicles.Count && finalVehicle.GetComponent<Vehicle>().moved) {
			GameObject ve = (GameObject)vehicles[vehicles.Count-1];
			print (ve.GetComponent<Vehicle>().moved);
			foreach(GameObject v in vehicles)
				v.GetComponent<Vehicle>().moved=false;
			vNum=0;
			selected=null;
			cooldown=0.1f;
		}
	}
}
