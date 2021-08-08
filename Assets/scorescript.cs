using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class scorescript : MonoBehaviour {
	private GameObject samurai;
	string bldleft="";
	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");
	}
	
	// Update is called once per frame
	void Update () {
		bldleft = samurai.GetComponent<samuraiscript> ().getbld();
		//GetComponent<Text> ().text = "PT: "+samurai.GetComponent<samuraiscript>().getscore() + "  BD: "+bldleft + "  TY: "+ samurai.GetComponent<samuraiscript> ().getbldtype();

		GetComponent<Text> ().text = samurai.GetComponent<samuraiscript> ().getscore ();
	}
}
