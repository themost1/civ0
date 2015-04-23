using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {
	public static float cooldown=0;
	public float spawnRadius = 4f;
	public static int currentPlayer=1;
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

			bool canSpawn = false;
			int playersTanks = 0; //number of tanks for that player

			foreach(GameObject go in vehicles)
				if(go.GetComponent<Vehicle>().player == currentPlayer)
					playersTanks++;

			if(playersTanks == 0)
				canSpawn = true;
			else
				foreach(GameObject go in vehicles)
					if(Vector3.Distance(go.transform.position,SelectUnit.clickedHex.worldPosition) < spawnRadius)
						canSpawn = true;
			
			if(canSpawn){
				if(SelectUnit.clickedHex != null && !SelectUnit.clickedHex.full){
					Vector3 pos = SelectUnit.clickedHex.worldPosition;
					pos.y = 0.4f;
					vehicle = (GameObject)Instantiate(tank, pos, new Quaternion(0,0,0,0));
					vehicle.GetComponent<Vehicle>().player = currentPlayer;
					MeshRenderer[] chillin = vehicle.GetComponentsInChildren<MeshRenderer>();
					foreach(MeshRenderer mr in chillin)
						if(currentPlayer == 1)	
							mr.material.color = Color.blue;
						else if(currentPlayer == 2)
							mr.material.color = Color.red;
					
					if(currentPlayer == 2)
						vehicle.transform.Rotate(0,180f,0);

					vehicles.Add(vehicle);

					SelectUnit.clickedHex.full = true;
				}
			}
		}
		
		

		selectVehicle ();
	}

	void selectVehicle() {
		foreach(GameObject go in vehicles)
			if(go.GetComponent<Vehicle>().player == currentPlayer)
				if(Vector3.Distance(go.transform.position,SelectUnit.clickedHex.worldPosition) < 1)
					selected = go;
	}
}