using UnityEngine;
using System.Collections;

public class swordscript : MonoBehaviour {

	// Use this for initialization
	private GameObject samurai;
	void Start () {
		samurai = GameObject.Find ("MYSamurai");
	}
	
	void OnTriggerEnter(Collider coll){
		//Debug.Log ("Sword Collided with: " + coll.gameObject.name);
		if (coll.gameObject.tag == "block" && samurai.GetComponent<samuraiscript>().isattacking()) {
			//samurai.GetComponent<samuraiscript> ().setscore (10);
			//Destroy (coll.gameObject);
		}

	}

	void OnTriggerStay(Collider coll){
		if (coll.gameObject.tag == "block" && samurai.GetComponent<samuraiscript>().isattacking()) {
			//samurai.GetComponent<samuraiscript> ().setscore (10);
			//Destroy (coll.gameObject);
		}
	}

	void OnTriggerExit(Collider coll){
		//Debug.Log ("Sword exited with: " + coll.gameObject.name);
	}

	void Update () {
	
	}
}
