using UnityEngine;
using System.Collections;

public class enemyscript : MonoBehaviour {
	GameObject samchar;
	CharacterController enemycontroller;
	Vector3 enemyDirection, tempdir;
	float distance, speed;
	bool attacking, isdead;
	float hit;
	public AudioClip sound1, sound2;

	// Use this for initialization
	void Start () {
		samchar= GameObject.Find ("MYSamurai");
		enemycontroller=GetComponent<CharacterController> ();
		GetComponent<Animation> ().Play ("idle_battle");
		attacking = false;
		hit = 0f;
		isdead = false;
		speed = 0f;



	}


	void OnTriggerEnter(Collider coll){
		

	}


	void OnTriggerExit(Collider coll){
		


	}


	void OnTriggerStay(Collider coll){
		
		if (coll.gameObject.name == "Cylinder001" && samchar.GetComponent<samuraiscript>().isattacking()) {
			
			enemyDirection=transform.TransformDirection (Vector3.back);
			enemycontroller.Move (enemyDirection * (speed+ 2f));
			if (attacking) {
				hit = hit + 0.2f;

			} else {
				hit++;
				//GetComponent<AudioSource> ().PlayOneShot (sound1);
			}
		}
		if (hit>10 && !isdead){
			samchar.GetComponent<samuraiscript> ().setscore (10);
			isdead = true;

		}

			

			
	}


	IEnumerator waitattacking (){
		

		samchar.GetComponent<samuraiscript>().sethealth (1);
		GetComponent<Animation> ().Play ("attack1");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("attack1").length);
		attacking = false;
	}


	IEnumerator waitdead(){
		GetComponent<Animation> ().Play ("death2");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("death2").length);
		Destroy (transform.gameObject);
		
	}

	// Update is called once per frame
	void Update () {
		tempdir = samchar.transform.position;
		distance = Vector3.Magnitude (transform.position - tempdir);
		if (distance > 50 && !isdead) {
			speed = 0f;
			GetComponent<Animation> ().Play ("idle_battle");
		} else if (distance > 2.5 && distance<50 && !isdead && !attacking) {
			speed = 0.02f;
			GetComponent<Animation> ().Play ("walk");
		} else if (distance < 2.5 && attacking == false && !isdead) {
			//GetComponent<Animation> ().Play ("attack1");
			speed = 0.06f;
			attacking = true;
			StartCoroutine (waitattacking ());
		} 

		if (!isdead) {
			tempdir.y = transform.position.y;
			transform.LookAt (tempdir);
			enemyDirection = transform.TransformDirection (Vector3.forward);



			enemycontroller.Move (enemyDirection * speed);
		}

		if (isdead) {
			StartCoroutine (waitdead ());

		}




		if (enemycontroller.isGrounded==false) {
			enemyDirection=transform.TransformDirection(Vector3.down);
			enemycontroller.Move(enemyDirection*0.2f);
		}


	}
}
