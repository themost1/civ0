using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {
	public static float cooldown=0;
	public float spawnRadius = 4f;
	public static int currentPlayer=1;
	public GameObject tank;
	public static GameObject selected;
	public int vNum;
	public static ArrayList vehicles = new ArrayList();
	public static int powerPoints = 10;
	public static int turns = 1;
	public static int tankCost, wallCost;

	// Use this for initialization
	void Start () {
		cooldown = 0.1f;
		calcCosts();
	}
	
	void OnGUI(){
		Texture2D dankBG = new Texture2D(1,1);
		dankBG.SetPixel(1,1,new Color(0,0,0));
		dankBG.wrapMode = TextureWrapMode.Repeat;
		dankBG.Apply();
		GUIStyle dankStyle = new GUIStyle();
		GUIStyleState dankSS = new GUIStyleState();
		dankSS.background = dankBG;
		dankStyle.normal = dankSS;
		dankStyle.alignment = TextAnchor.MiddleLeft;
		dankStyle.normal.textColor=new Color(255,255,255);
		dankStyle.fontSize=14;
		
//		GUI.Label(new Rect(0,0,100,30),"  Powerpoints: "+powerPoints,dankStyle);
//		GUI.Label(new Rect(100,0,150,30),"  Current player: "+currentPlayer,dankStyle);
//		GUI.Label(new Rect(250,0,80,30),"  Turns: "+turns,dankStyle);
//		GUI.Label(new Rect(330,0,80,30),"  Tank Cost: "+tankCost,dankStyle);
//		GUI.Label(new Rect(410,0,100,30),"  Wall Cost: "+wallCost,dankStyle);
	
		GUI.Label(new Rect(0,0,520,30),"  Powerpoints: "+powerPoints+"    Current player: "+currentPlayer
		          +"    Turns: "+turns+ "    Tank Cost: "+tankCost+"    Wall Cost: "+wallCost,dankStyle);
	}
	
	// Update is called once per frame
	void Update () {
		if (cooldown>0)
			cooldown += Time.deltaTime;
			
		if (Input.GetKey ("1") && powerPoints >= tankCost){
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
					powerPoints -= tankCost;
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
					GameObject obj = vehicle.transform.Find ("HealthBar").gameObject;
					obj.GetComponent<Renderer>().material.color = Color.green;
					
					if(currentPlayer == 2)
						vehicle.transform.Rotate(0,180f,0);

					vehicles.Add(vehicle);

					SelectUnit.clickedHex.full = true;
				}
			}
		}

		selectVehicle ();
	}
	
	public static void calcCosts(){
		tankCost = (int)(10f/(1f + Mathf.Exp(-0.25f*(turns-4f))));
		wallCost = (int)(6f/(1f + Mathf.Exp(-0.25f*(turns-5f))));
	}

	void selectVehicle() {
		selected = null;
		foreach(GameObject go in vehicles)
			if(go.GetComponent<Vehicle>().player == currentPlayer)
				if(Vector3.Distance(go.transform.position,SelectUnit.clickedHex.worldPosition) < 1)
					selected = go;
	}
}