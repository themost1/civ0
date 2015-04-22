using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {
	public static float cooldown=0, currentPlayer=1, turn;
	public GameObject tank;
	public static GameObject selected;
	public int vNum;
	public ArrayList p1vehicles = new ArrayList();
	public ArrayList p2vehicles = new ArrayList();

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
				SelectUnit.clickedHex.full = true;
			}

			if(vehicle != null){
				if(currentPlayer == 1){
					MeshRenderer[] chillin = vehicle.GetComponentsInChildren<MeshRenderer>();
					foreach(MeshRenderer mr in chillin)
						mr.material.color = Color.blue;
					p1vehicles.Add (vehicle);
					currentPlayer = 2;
				}
				else if(currentPlayer == 2){
					MeshRenderer[] chillin = vehicle.GetComponentsInChildren<MeshRenderer>();
					foreach(MeshRenderer mr in chillin)
						mr.material.color = Color.red;
					p2vehicles.Add (vehicle);
					currentPlayer = 1;
				}
			}
			turn++;
		}

		selectVehicle ();
//		foreach (GameObject go in vehicles) {
//			if(go != null && SelectUnit.clickedHex != null)
//				if (Vector3.Distance(go.transform.position, SelectUnit.clickedHex.worldPosition)<=1)
//					selected=go;
//			go.GetComponent<Vehicle>().moved=false;
//		}
//		if(selected != null)
//			if (Vector3.Distance (selected.transform.position, SelectUnit.clickedHex.worldPosition) > 1)
//				selected = null;
	}

	void selectVehicle() {
//		if (selected.GetComponent<Vehicle> ().moved && cooldown == 0)
//			cooldown = 0.1f;
//		else if (selected.GetComponent<Vehicle> ().moved && cooldown>0.3) {
//			selected=(GameObject)vehicles[vNum];
//			cooldown=0f;
//			vNum++;
//		}
//		GameObject finalVehicle = (GameObject)vehicles [vehicles.Count - 1];
//		if (vNum == vehicles.Count && finalVehicle.GetComponent<Vehicle>().moved) {
//			GameObject ve = (GameObject)vehicles[vehicles.Count-1];
//			print (ve.GetComponent<Vehicle>().moved);
//			foreach(GameObject v in vehicles)
//				v.GetComponent<Vehicle>().moved=false;
//			vNum=0;
//			selected=null;
//			cooldown=0.1f;
//		}
		if(currentPlayer == 1)
			foreach(GameObject go in p1vehicles)
				if(Vector3.Distance(go.transform.position,SelectUnit.clickedHex.worldPosition) < 1)
					selected = go;
		else if(currentPlayer == 2)
			foreach(GameObject go2 in p2vehicles)
				if(Vector3.Distance(go.transform.position,SelectUnit.clickedHex.worldPosition) < 1)
					selected = go2;
	}
}