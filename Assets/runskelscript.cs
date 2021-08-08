using UnityEngine;
using System.Collections;

public class runskelscript : MonoBehaviour {
	GameObject samchar;
	CharacterController enemycontroller;
	Vector3 enemyDirection, tempdir;
	float distance, speed;
	bool attacking, isdead, is_hit;
	int health;
	public AudioClip sound1, sound2;
	int combocounter;
	int fallbackcounter;

	GameObject texthealth;

	// Use this for initialization
	void Start () {
		samchar= GameObject.Find ("MYSamurai");
		texthealth = GameObject.Find ("EnemyHealth");
		enemycontroller=GetComponent<CharacterController> ();
		GetComponent<Animation> ().Play ("idle");
		attacking = false;
		health = 90;
		isdead = false;
		speed = 0f;
		is_hit = false;
		fallbackcounter = 0;

		combocounter = 2;


		tempdir = samchar.transform.position;
		distance = Vector3.Magnitude (transform.position - tempdir);


	}


	void OnTriggerEnter(Collider coll){
		

	}


	void OnTriggerExit(Collider coll){
		


	}


	void OnTriggerStay(Collider coll){
		
		if (coll.gameObject.name == "Cylinder001" && samchar.GetComponent<samuraiscript>().isattacking()) {
			is_hit = true;
		
		
			health = health - 2;
				//GetComponent<AudioSource> ().PlayOneShot (sound1);

		}
	if (health<1 && !isdead){
			
			samchar.GetComponent<samuraiscript> ().setscore (10);
			isdead = true;

		}


			
			

			
	}


	IEnumerator waitattacking (){
		string animhit;
		if (combocounter<2){
			animhit = "punch";
			combocounter++;
		}
		else{
			animhit = "punch";
			combocounter=0;
		}
		GetComponent<Animation> ().Play (animhit);
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip(animhit).length/2);

		Vector3 tempdirect = samchar.transform.position;
		float tempdistance = Vector3.Magnitude (transform.position - tempdirect);
		//if attack was in striking distance take health
		if (tempdistance < 2) {
			samchar.GetComponent<samuraiscript> ().sethealth (3);
			samchar.GetComponent<samuraiscript> ().fallposition ();
		}

		yield return new WaitForSeconds (GetComponent<Animation>().GetClip(animhit).length/2);


	
		attacking = false;
	}


	IEnumerator waitdead(){
		GetComponent<Animation> ().Play ("death");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("death").length);
		Destroy (transform.gameObject);
		
	}

	// Update is called once per frame
	void Update () {

		tempdir = samchar.transform.position;
		distance = Vector3.Magnitude (transform.position - tempdir);
		
		if (distance > 50 && !isdead) {
			speed = 0f;
			GetComponent<Animation> ().Play ("idle");
		} else if (distance >1.7f && distance<50 && !isdead &&!is_hit) {
			
			speed = 0.1f;
			GetComponent<Animation> ().Play ("run");

			tempdir.y = transform.position.y;
			transform.LookAt (tempdir);
			enemyDirection = transform.TransformDirection (Vector3.forward);
			enemycontroller.Move (enemyDirection * speed);

		} else if (distance <=1.7f && attacking == false && !isdead && !is_hit) {
			
			speed = 0.4f;
			attacking = true;

			tempdir.y = transform.position.y;
			transform.LookAt (tempdir);
			enemyDirection = transform.TransformDirection (Vector3.forward);
			enemycontroller.Move (enemyDirection * speed);

			StartCoroutine (waitattacking ()); 
		} 
	
		if (distance<20)
			texthealth.GetComponent<enemyhealthscript>().getdata (transform.name, health);

		/*
		if (!isdead) {
			tempdir.y = transform.position.y;
			transform.LookAt (tempdir);
			enemyDirection = transform.TransformDirection (Vector3.forward);
			enemycontroller.Move (enemyDirection * speed);
		}

        */

		if (isdead) {
			StartCoroutine (waitdead ());

		}

		if (is_hit) {
			enemyDirection=transform.TransformDirection (Vector3.back);
			enemycontroller.Move (enemyDirection * (0.06f));
			is_hit = false;
		}



		if (enemycontroller.isGrounded==false) {
			enemyDirection=transform.TransformDirection(Vector3.down);
			enemycontroller.Move(enemyDirection*0.2f);
		}







	}
}
