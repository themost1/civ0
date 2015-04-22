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
		if (Input.GetKey ("6")){
			GameObject vehicle = null;

			if(SelectUnit.clickedHex != null && !SelectUnit.clickedHex.full){
				Vector3 pos = SelectUnit.clickedHex.worldPosition;
				pos.y = 0.4f;
				vehicle = (GameObject)Instantiate(tank, pos, new Quaternion(0,0,0,0));
				currentPlayer=2;
				SelectUnit.clickedHex.full = true;
			}

			if(vehicle != null){
				MeshRenderer[] chillin = vehicle.GetComponentsInChildren<MeshRenderer>();
				foreach(MeshRenderer mr in chillin)
					mr.material.color = Color.blue;

				vehicles.Add (vehicle);
			}
			turn++;
		} 
		else if (Input.GetKey ("5") && currentPlayer==2) {
			GameObject vehicle = null;

			if(SelectUnit.clickedHex != null && !SelectUnit.clickedHex.full){
				Vector3 pos = SelectUnit.clickedHex.worldPosition;
				pos.y = 0.4f;
				vehicle = (GameObject)Instantiate(tank, pos, new Quaternion(0,0,0,0));
				vehicle.transform.Rotate(Vector3.right);
				currentPlayer=1;
				SelectUnit.clickedHex.full = true;
			}

			if(vehicle != null){
				MeshRenderer[] chillin = vehicle.GetComponentsInChildren<MeshRenderer>();
				foreach(MeshRenderer mr in chillin)
					mr.material.color = Color.red;
				vehicles.Add (vehicle);
			}
		}

		//selectVehicle ();
		foreach (GameObject go in vehicles) {
			if(go != null && SelectUnit.clickedHex != null)
				if (Vector3.Distance(go.transform.position, SelectUnit.clickedHex.worldPosition)<=1)
					selected=go;
			go.GetComponent<Vehicle>().moved=false;
		}
		if(selected != null)
			if (Vector3.Distance (selected.transform.position, SelectUnit.clickedHex.worldPosition) > 1)
				selected = null;
	}
	void selectVehicle() {
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
