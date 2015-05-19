using UnityEngine;
using System.Collections;

public class GunShot : MonoBehaviour {
	public static bool makeSound = false, makeExplosion = false;
	public AudioClip clip, clip2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (makeSound) {
			makeSound=false;
			GetComponent<AudioSource>().PlayOneShot(clip);
		}
		if (makeExplosion) {
			makeExplosion=false;
			GetComponent<AudioSource>().PlayOneShot(clip2);
		}
	}
}
