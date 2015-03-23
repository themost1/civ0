using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BorderTextureData : ScriptableObject {

	public int textureWidth;
	public int textureHeight;

	public string idName;
	public List<Rect> rects;
	public List<string> names;

	public Rect GetRectFromBorderId( int borderId ){

		return rects[ names.IndexOf( borderId.ToString() ) ];

	}

}
