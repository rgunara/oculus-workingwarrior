using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class healthscript : MonoBehaviour {
	private GameObject samurai;

	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");

	}

	string healthbar(int v){
		string bar = "";
		for (int i = 1; i <= Mathf.CeilToInt(v/6); i++) {
			bar=bar+"I";

		}
		return bar;
	}



	//Update is called once per frame
	void Update () {
		//GetComponent<Text> ().text = ""+samurai.GetComponent<samuraiscript>().gethealth().ToString()+"%";
		//GetComponent<Text> ().text = healthbar(samurai.GetComponent<samuraiscript>().gethealth());
	}
}
