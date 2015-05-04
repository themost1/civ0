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
	public static int powerPoints = 5;

	// Use this for initialization
	void Start () {
		cooldown = 0.1f;
	}
	
	void OnGUI(){
		Texture2D dankBG = new Texture2D(1,1);
		dankBG.SetPixel(1,1,new Color(50,50,50));
		dankBG.wrapMode = TextureWrapMode.Repeat;
		dankBG.Apply();
		GUIStyle dankStyle = new GUIStyle();
		GUIStyleState dankSS = new GUIStyleState();
		dankSS.background = dankBG;
		dankStyle.normal = dankSS;
		dankStyle.alignment = TextAnchor.MiddleCenter;
	
		GUI.Label(new Rect(25,25,100,30), "Powerpoints: " + powerPoints,dankStyle);	
	}
	
	// Update is called once per frame
	void Update () {
		if (cooldown>0)
			cooldown += Time.deltaTime;
		if (Input.GetKey ("6") && powerPoints > 1){
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
					powerPoints -= 2;
					Vector3 pos = SelectUnit.clickedHex.worldPosition;
					pos.y = 0.4f;
					vehicle = (GameObject)Instantiate(tank,pos,new Quaternion(0,0,0,0));
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
		selected = null;
		foreach(GameObject go in vehicles)
			if(go.GetComponent<Vehicle>().player == currentPlayer)
				if(Vector3.Distance(go.transform.position,SelectUnit.clickedHex.worldPosition) < 1){
					selected = go;
					Debug.Log ("Tank selected");
				}
	}
}