using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Bridgescript : MonoBehaviour {
	private GameObject samurai, panel;
	Vector3 charDirection, spawnto;
	Vector3 originalp;
	Quaternion originalr;
	CharacterController mycontroller;
	Vector3 enemyDirection, tempdir;
	int count, count2;
	bool dest=false;
	public bool created=false;


	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");
		spawnto = transform.position;
		mycontroller=GetComponent<CharacterController> ();
		count = 0;
		count2 = 0;
		panel = GameObject.Find ("BridgePanel");


		//	transform.rotation=samurai.transform.rotation;
		//	transform.position = samurai.transform.position + samurai.transform.TransformDirection (Vector3.up) * 2f;


		//+ samurai.transform.TransformDirection (Vector3.forward) * 0.5f + samurai.transform.TransformDirection (Vector3.up) * 1f;

	}

	public void setoriginal(Vector3 p, Quaternion q){
		originalp = p;
		originalr = q;


	}

	void OnTriggerEnter(Collider coll){
		
		//	if (coll.gameObject.tag != "grass") 
		//		Destroy (transform.gameObject);


	}


	void OnTriggerStay(Collider coll){
		
	//	if (coll.gameObject.tag != "grass") 
	//		Destroy (transform.gameObject);


	}





	//Update is called once per frame
	void Update () {
		

	     
	
			if (mycontroller.isGrounded == false && count < 68 && !dest) {
				enemyDirection = transform.TransformDirection (Vector3.down);
				mycontroller.Move (enemyDirection * 0.03f);
				count++;
			    if (count == 68) {
				   samurai.GetComponent<samuraiscript> ().setdata (originalp, originalr, "mybridge");
				   created = true;
			    }
			} 

		if (mycontroller.isGrounded) {
			dest = true;
			panel.GetComponent<Image> ().enabled = true;
		}

			if (dest && count2 < 400) {
				enemyDirection = transform.TransformDirection (Vector3.up);
				mycontroller.Move (enemyDirection * 0.03f);
				count2++;
			}

			if (count2 == 400) {
			    panel.GetComponent<Image> ().enabled = false;
				Debug.Log ("bridge destroyed");
				Destroy (transform.gameObject);

			}

			



	}
}
