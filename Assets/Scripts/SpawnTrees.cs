using UnityEngine;
using System.Collections;

public class SpawnTrees : MonoBehaviour{
	public GameObject Conifer,Broadleaf;
	
	public void Start (){
//		foreach(HexChunk chunk in WorldManager.hexChunks)
//			foreach(HexInfo hex in chunk.hexArray)
//				if(hex.broadleaf||hex.conifer){
//					int lim=Random.Range(7,20);
//					for(int c=0;c<lim;c++){
//						float x=1,y=1;
//						while(Mathf.Pow(x,2f)+Mathf.Pow(y,2f)>1f){
//							x=Random.Range(-1f,1f);
//							y=Random.Range(-1f,1f);
//						}
//						Vector3 vec=new Vector3(x,0,y);
//						
//						if(hex.conifer)
//							Instantiate(Conifer,hex.worldPosition+vec,new Quaternion(0,0,0,0));
//						else
//							Instantiate(Broadleaf,hex.worldPosition+vec,new Quaternion(0,0,0,0));
//					}		
//				}
	}
}