using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class enemyhealthscript : MonoBehaviour {
	private GameObject samurai;
	string data;
	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");
		data = "";
	}

	string healthbar(int v){
		string bar = "";
		for (int i = 1; i <= Mathf.RoundToInt(v/4); i++) {
			bar=bar+"I";

		}
		return bar;
	}


	public void getdata(string name, int value){
		if (value < 0)
			value = 0;
		
		//data = data + name + ": " + value.ToString () +" \n" ;
		data=data+ healthbar(value) +"\n\n";
	}
	// Update is called once per frame
	void Update () {
		GetComponent<Text> ().text = data;
		data = "";

	}
}
