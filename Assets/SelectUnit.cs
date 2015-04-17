using UnityEngine;
using System.Collections;

public class SelectUnit : MonoBehaviour {
	public static HexInfo clickedHex;
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
			if (hit) 
			{
				float minDistance = 1000f;
				HexInfo theHex = null;
				foreach(HexChunk chunk in WorldManager.hexChunks){
					foreach(HexInfo hex in chunk.hexArray){
						float dist = Vector3.Distance(hitInfo.point,hex.worldPosition);
						if(dist < minDistance){
							minDistance = dist;
							theHex = hex;
						}
					}
				}
				if(theHex == null)
					Debug.Log("Hex is null");
				theHex.clicked = true;
				theHex.Start();
				//Debug.Log("Mesh reset");
				theHex.parentChunk.Combine();
				//Debug.Log("Chunk recombined");
				if (clickedHex!=null && clickedHex!=theHex) {
					clickedHex.clicked=false;
					clickedHex.Start ();
					//Debug.Log("Mesh reset");
					clickedHex.parentChunk.Combine();
				}
				clickedHex=theHex;
				
			} else {
				Debug.Log("No hit");
			}
		} 
	}
}