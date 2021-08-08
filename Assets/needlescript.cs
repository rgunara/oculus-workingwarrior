using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class needlescript : MonoBehaviour {
	private GameObject samurai;
	public bool needlestatus;
	Vector3 tempdir;
	float distance;
    bool play=true;
	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");

	}

	IEnumerator playneedle (){
		play = false;
		GetComponent<Animation> ().Play ("Anim_TrapNeedle_Play");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Anim_TrapNeedle_Play").length);
		needlestatus = false;
		play = true;


	}

	IEnumerator idleneedle (){
		play = false;
		GetComponent<Animation> ().Play ("Anim_TrapNeedle_Idle");
		yield return new WaitForSeconds (GetComponent<Animation> ().GetClip ("Anim_TrapNeedle_Play").length+1);
		needlestatus = true;
		play=true;


	}
	//Update is called once per frame
	void Update () {
		tempdir = samurai.transform.position;
		distance = Vector3.Magnitude (transform.position - tempdir);
		if (distance < 30) {
			if (play && needlestatus)
				StartCoroutine (playneedle ());
			else if (play && !needlestatus)
				StartCoroutine (idleneedle ());

		}
	}
}
