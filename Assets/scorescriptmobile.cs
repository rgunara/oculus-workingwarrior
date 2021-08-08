using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class scorescriptmobile : MonoBehaviour {
	private GameObject samurai;
	string bldleft="";
	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");
	}
	
	// Update is called once per frame
	void Update () {
		bldleft = samurai.GetComponent<samuraiscript> ().getbld();
		GetComponent<Text> ().text = "Score: "+samurai.GetComponent<samuraiscript>().getscore() + "      Bld: "+bldleft;
	}
}
