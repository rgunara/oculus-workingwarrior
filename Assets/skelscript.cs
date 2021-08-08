using UnityEngine;
using System.Collections;

public class skelscript : MonoBehaviour {
	GameObject samchar;
	CharacterController enemycontroller;
	Vector3 enemyDirection, tempdir;
	float distance, speed;
	bool attacking, isdead, isdead2, is_hit;
	int health;
	public AudioClip sound1, sound2;
	string whatsound="";
	bool waitsound;
	IEnumerator co;

	GameObject texthealth;

	// Use this for initialization
	void Start () {
		samchar= GameObject.Find ("MYSamurai");
		texthealth = GameObject.Find ("EnemyHealth");
		enemycontroller=GetComponent<CharacterController> ();
		GetComponent<Animation> ().Play ("Idle1");
		attacking = false;
		health = 80;
		isdead = false;
		isdead2 = false;
		speed = 0f;
		is_hit = false;
		waitsound = false;




	}
		


	void OnTriggerEnter(Collider coll){
		

	}


	void OnTriggerExit(Collider coll){
		


	}


	void OnTriggerStay(Collider coll){
		
		if (coll.gameObject.name == "Cylinder001" && samchar.GetComponent<samuraiscript>().isattacking()) {
			is_hit = true;
		
		    // was -4
			health = health - 8;
				//GetComponent<AudioSource> ().PlayOneShot (sound1);

		}
	if (health<1 && !isdead){
			
			samchar.GetComponent<samuraiscript> ().setscore (10);
			isdead = true;

		}


			
			

			
	}


	IEnumerator waitattacking (){
		


		GetComponent<Animation> ().Play ("Attack1h1");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Attack1h1").length/2);

		Vector3 tempdirect = samchar.transform.position;
		float tempdistance = Vector3.Magnitude (transform.position - tempdirect);
		//if attack was in striking distance take health
		if (tempdistance < 3) {
			samchar.GetComponent<samuraiscript> ().sethealth (3);
			samchar.GetComponent<samuraiscript> ().fallposition ();
		}

		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Attack1h1").length/2);




		
		attacking = false;
	}


	IEnumerator waitdead(){

	
		GetComponent<Animation> ().Play ("Hit1");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Hit1").length);
		isdead2 = true;

		//Destroy (transform.gameObject);
		transform.position=new Vector3 (0f, -500f, 0f);
	}


	IEnumerator playingsound (){
		whatsound = "growl";
		waitsound = true;


			GetComponent<AudioSource> ().PlayOneShot (sound1);
			yield return new WaitForSeconds (sound1.length*2);
		whatsound = "";
		waitsound = false;


	}

	IEnumerator playingsound2 (){
		whatsound = "swordhit";
		waitsound = true;


		GetComponent<AudioSource> ().PlayOneShot (sound2);
		yield return new WaitForSeconds (sound2.length*0.5f);
		whatsound = "";
		waitsound = false;


	}


	// Update is called once per frame
	void Update () {
		
		if (isdead2 == true && waitsound == false) {
			Destroy (transform.gameObject);
			Debug.Log ("is destroyed");
		}
        


		tempdir = samchar.transform.position;
		distance = Vector3.Magnitude (transform.position - tempdir);
		if (distance > 40 && !isdead) {
			speed = 0f;
			GetComponent<Animation> ().Play ("Idle1");
		} else if (distance > 2 && distance<40 && !isdead && !attacking && !is_hit) {
			
			speed = 0.04f;
			GetComponent<Animation> ().Play ("Walk");


		} else if (distance < 2 && attacking == false && !isdead && !is_hit) {
			


			speed = 0.00f;
			attacking = true;
			StartCoroutine (waitattacking ()); 

		




		} 

		if (distance < 20 && !isdead) {
			if (waitsound == false) {

				co = playingsound ();
				StartCoroutine (co);
			}

		}


	
		if (distance<20)
			texthealth.GetComponent<enemyhealthscript>().getdata (transform.name, health);

		if (!isdead && distance > 2) {
			tempdir.y = transform.position.y;
			transform.LookAt (tempdir);
			enemyDirection = transform.TransformDirection (Vector3.forward);
			enemycontroller.Move (enemyDirection * speed);
		}

		if (isdead) {
			Debug.Log ("isdead if");

		
			   StartCoroutine (waitdead ());


		}

		if (is_hit) {

			if (whatsound != "swordhit") {
				GetComponent<AudioSource> ().Stop ();
				StopCoroutine (co);
				waitsound = false;
			}
			if (waitsound == false) {

				co = playingsound2 ();
				StartCoroutine (co);
			}


			enemyDirection=transform.TransformDirection (Vector3.back);
			enemycontroller.Move (enemyDirection * 0.05f);
			is_hit = false;
		}



		if (enemycontroller.isGrounded==false) {
			enemyDirection=transform.TransformDirection(Vector3.down);
			enemycontroller.Move(enemyDirection*0.2f);
		}


	}
}
