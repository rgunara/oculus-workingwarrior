using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
public class samuraiscriptmobile : MonoBehaviour {

	// Notes:  Original Rear camera:  -0.8899665, 1.88, -1.9 
	//         Original Main camera: -0.7, 1.5. -1


	private float runSpeed= 0.15f;
	private float mousex, mousey;
	int score, health, airtime, camcount;
	CharacterController mycontroller;
	Vector3 charDirection, camDirection, spawnto, temprot, rotatebridge;
	private Camera maincam, rearcam;
	bool stopgravity, iscolliding, attacking, inpit, is_hit, hit2air, onbridge;
    bool invertcont;
	private int icounter;
	private int triggercounter;
	Quaternion spawnrotation, camrotation;
	GameObject blockX;
	float leftx, lefty, rightx, righty, joytrigg;
	public AudioClip sound1, sound2;
	bool waitsound;
	IEnumerator co;
	string where, buildingtype;
	bool startrotationup, startrotationdown, camreset;
	float rotprev, rotcur, rotdiff, turnprev, turncur, turndiff;

	GameObject exitscreen;
	Canvas msc, buttonControl;
	 MyString mystring, mystring2;
	public Material sky1, sky2;
	int skycount=0;
	bool undoflag=true;

	void Start () {
		
		mycontroller = GetComponent<CharacterController> ();
		maincam = Camera.allCameras[0];
		rearcam = Camera.allCameras[1];
		maincam.enabled = true;
		rearcam.enabled = false;
		stopgravity = false;
		icounter = 0;
		iscolliding = false;
		triggercounter = 0;
		spawnrotation = transform.rotation;
		attacking = false;
		inpit = false;
		leftx = 0f;
		rightx = 0f;
		lefty = 0f;
		righty = 0f;
		invertcont = true;
		score = 0;
		health = 150;
		spawnto = transform.position;
		camrotation = rearcam.transform.rotation;
		waitsound = false;
		is_hit = false;
		hit2air = false;
		AudioListener.pause = true;
		airtime = 15;
		co = playingsound ();
		StartCoroutine (co);
		onbridge = false;
		where = "no where";
		camcount = 0;
		startrotationup = false;
		startrotationdown = false;
		rotprev = 0f;
		rotcur = 0f;
		rotdiff = 0f;
		camreset = false;
		turnprev = 0f;
		turncur = 0f;
		turndiff = 0f;
		exitscreen = GameObject.Find ("exitscreen");
		exitscreen.SetActive (false);
		msc = GameObject.Find ("MobileSingleStickControl").GetComponent<Canvas>();
		buttonControl = GameObject.Find ("ButtonControl").GetComponent<Canvas>();
		buttonControl.enabled = false;
		buildingtype = "mybridge";
		mystring = new MyString ();

		RenderSettings.skybox = sky2;
		skycount = 1;
		DynamicGI.UpdateEnvironment ();
	}


	void OnControllerColliderHit(ControllerColliderHit coll){
		
		if (coll.gameObject.name == "pit") {
			inpit = true;
		}
	}

	void OnTriggerEnter(Collider coll){
		
		if (coll.gameObject.name != "MYSamurai" && coll.gameObject.name != "Cylinder001" && coll.gameObject.tag != "block") {
				where = coll.gameObject.name;
			triggercounter++;
			//Debug.Log ("Trigger starting with : " + coll.gameObject.name);
			//Debug.Log ("Box Collided with: " + coll.gameObject.name +"  trigger "+triggercounter);
			iscolliding = true;
			//runSpeed = 0f;
		} 
		else if (coll.gameObject.tag == "grass")
			onbridge = true;


		if (coll.gameObject.name == "Needle" || coll.gameObject.name == "Cutter") {
			Debug.Log ("hit by needle");
			sethealth (20);
			fallposition ();

		}
		if (coll.gameObject.name == "GreatAxe") {
			Debug.Log ("hit by needle");
			sethealth (50);
			fallposition ();

		}

		if (coll.gameObject.name == "SawBlade" ) {
			Debug.Log ("hit by needle");
			sethealth (50);
			fallposition ();

		}

	}

	IEnumerator waitattacking (){
		attacking = true;
		GetComponent<Animation> ().Play ("Attack");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Attack").length);
		attacking = false;
	

	}


	IEnumerator exploding (){
		GameObject exp = Instantiate (Resources.Load ("Explosion")) as GameObject;
		exp.transform.position = transform.position;
		exp.transform.rotation = transform.rotation;
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Attack").length+2);
		Destroy (exp);


	}

	IEnumerator waitjumping (){
		
		GetComponent<Animation> ().Play ("Run");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Run").length);



	}

	IEnumerator swipesound (){
		for (int i = 0; i < 2; i++) {
			GetComponent<AudioSource> ().PlayOneShot (sound2);
			yield return new WaitForSeconds (sound2.length);
		}


	}


	IEnumerator playingsound (){
		waitsound = true;
		GetComponent<AudioSource> ().PlayOneShot (sound1);
		yield return new WaitForSeconds (sound1.length);
		waitsound = false;


	}

	public void setdata(Vector3 a, Quaternion b, string c){
		
		mystring.locationx [mystring.count] = a.x;
		mystring.locationy [mystring.count] = a.y;
		mystring.locationz [mystring.count] = a.z;

	    mystring.rotationx [mystring.count] = b.x;
		mystring.rotationy [mystring.count] = b.y;
		mystring.rotationz [mystring.count] = b.z;
		mystring.rotationw [mystring.count] = b.w;
		mystring.type [mystring.count] = c;



		mystring.count++;

		
		Debug.Log ("setdata " + mystring.count);
	}


	public void setscore(int points){
		score = score + points;
	}

	public string getscore(){
		return score.ToString();
	}

	public string getbld(){
		int tempbld = 100 - mystring.count;
		return tempbld.ToString();
	}

	public int gethealth(){
		return health;
	}
	public void sethealth(int val){
		health = health - val;
		//Handheld.Vibrate ();
	}


	public void fallposition(){
		is_hit = true;

	}

	public bool charhit(){
		return is_hit;
	}

	public void save(){
		Debug.Log ("saving....");

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/bridgeData.dat");
		bf.Serialize (file, mystring);
		file.Close ();

	}


	IEnumerator Timer(){
		for (int j = 0; j <mystring2.count; j++) {
			if (mystring2.type[j]=="mybridge"){
				Vector3 tempvec = new Vector3 ();
				tempvec.x=mystring2.locationx[j];
				tempvec.y=mystring2.locationy[j];
				tempvec.z = mystring2.locationz[j];

				Quaternion tempq = new Quaternion (mystring2.rotationx[j], mystring2.rotationy[j], mystring2.rotationz[j], mystring2.rotationw[j]);

				blockX=Instantiate(Resources.Load(mystring2.type[j])) as GameObject; 
				blockX.transform.rotation = tempq;
				blockX.transform.position = tempvec;

				blockX.GetComponent<Bridgescript> ().setoriginal (tempvec, tempq);
				undoflag = false;
			}

		}
		yield return new WaitForSeconds(0.1f);
		for (int j = 0; j <mystring2.count; j++) {
			if (mystring2.type[j]=="mybridge 1"){
				Vector3 tempvec = new Vector3 ();
				tempvec.x=mystring2.locationx[j];
				tempvec.y=mystring2.locationy[j];
				tempvec.z = mystring2.locationz[j];

				Quaternion tempq = new Quaternion (mystring2.rotationx[j], mystring2.rotationy[j], mystring2.rotationz[j], mystring2.rotationw[j]);

				blockX=Instantiate(Resources.Load(mystring2.type[j])) as GameObject; 
				blockX.transform.rotation = tempq;
				blockX.transform.position = tempvec;


				blockX.GetComponent<Bridgescript1> ().setoriginal (tempvec, tempq);
				undoflag = false;
			}

		}
		yield return new WaitForSeconds(0.1f);
		for (int j = 0; j <mystring2.count; j++) {
			if (mystring2.type[j]=="mybridge 2"){
				Vector3 tempvec = new Vector3 ();
				tempvec.x=mystring2.locationx[j];
				tempvec.y=mystring2.locationy[j];
				tempvec.z = mystring2.locationz[j];

				Quaternion tempq = new Quaternion (mystring2.rotationx[j], mystring2.rotationy[j], mystring2.rotationz[j], mystring2.rotationw[j]);

				blockX=Instantiate(Resources.Load(mystring2.type[j])) as GameObject; 
				blockX.transform.rotation = tempq;
				blockX.transform.position = tempvec;

				blockX.GetComponent<Bridgescript2> ().setoriginal (tempvec, tempq);
				undoflag = false;

			}

		}
		yield return new WaitForSeconds(0.1f);
	}

	public void load(){
		Debug.Log ("loadings....");
		if (File.Exists (Application.persistentDataPath + "/bridgeData.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/bridgeData.dat", FileMode.Open);
			mystring2 = new MyString ();
			mystring2 = (MyString)bf.Deserialize (file);
			file.Close ();


		//	StartCoroutine(Timer());

			for (int j = 0; j < mystring2.count; j++) {
				
				Vector3 tempvec = new Vector3 ();
				tempvec.x = mystring2.locationx [j];
				tempvec.y = mystring2.locationy [j];
				tempvec.z = mystring2.locationz [j];

				Quaternion tempq = new Quaternion (mystring2.rotationx [j], mystring2.rotationy [j], mystring2.rotationz [j], mystring2.rotationw [j]);

				blockX = Instantiate (Resources.Load (mystring2.type [j])) as GameObject; 
				blockX.transform.rotation = tempq;
				blockX.transform.position = tempvec;

				if (mystring2.type [j] == "mybridge")
					blockX.GetComponent<Bridgescript> ().setoriginal (tempvec, tempq);
				else if (mystring2.type [j] == "mybridge 1")
					blockX.GetComponent<Bridgescript1> ().setoriginal (tempvec, tempq);
				else if (mystring2.type [j] == "mybridge 2")
					blockX.GetComponent<Bridgescript2> ().setoriginal (tempvec, tempq);

				undoflag = false;
		
			}

		} else
			Debug.Log ("No file to load");

	}


	void OnTriggerExit(Collider coll){
		if (coll.gameObject.name != "MYSamurai"  && coll.gameObject.name != "Cylinder001") {
		//	Debug.Log ("Trigger ended with : " + coll.gameObject.name);
			//runSpeed = 0.15f;
			iscolliding=false;
		}
		else if (coll.gameObject.tag == "grass")
			onbridge = false;
	}

	void ChangeCam(){

		//maincam.enabled = !maincam.enabled;
		//rearcam.enabled = !rearcam.enabled;

		Vector3 scalex = transform.localScale;
		scalex.x = scalex.x * (-1);
		transform.localScale = scalex;


	}

	void spawn(){
		
		transform.position = spawnto;
		transform.rotation = spawnrotation;
		mycontroller.Move (new Vector3 (0f, 0f, 0f));
		Debug.Log ("isgrounded value " + mycontroller.isGrounded);
		//StartCoroutine(exploding());
	}


	public bool isattacking(){
		return attacking;
	}
	// Update is called once per frame
	void Update () {
		//mousex = Input.GetAxis ("Mouse X");


		mousex = CrossPlatformInputManager.GetAxisRaw ("Mouse X")*1.5f;

	    charDirection = new Vector3 (0, mousex*1.5f, 0);


	    transform.Rotate (charDirection);

		//mousey = CrossPlatformInputManager.GetAxisRaw ("Mouse Y") * (-1);
	//	Debug.Log ("mousex: "+mousex + ",  mousey :" + mousey);
	//	camDirection = new Vector3 (mousey, 0, 0);
	//	rearcam.transform.Rotate (camDirection);


        
		if (invertcont) {
			
			leftx = CrossPlatformInputManager.GetAxisRaw ("Horizontal2");
			lefty = CrossPlatformInputManager.GetAxisRaw ("Vertical2")*(-1);
			righty = CrossPlatformInputManager.GetAxisRaw ("Vertical")*-1;
			rightx = CrossPlatformInputManager.GetAxisRaw ("Horizontal")*1;

	/*	
			leftx = Input.GetAxis ("left joy x");;
			lefty = Input.GetAxis ("left joy y");
			righty = Input.GetAxis ("right joy y");
			rightx = Input.GetAxis ("right joy x");
			*/

		} else {
			leftx = Input.GetAxis ("right joy x");
			lefty = Input.GetAxis ("right joy y");
			charDirection = new Vector3 (0, leftx * 2, 0);
			transform.Rotate (charDirection);

			rightx = Input.GetAxis ("left joy x");
			righty = Input.GetAxis ("left joy y");
		}




/*
		if (lefty>-1 && lefty<1){
			rotprev = rotcur;
			rotcur = lefty;
			rotdiff = rotcur - rotprev;


		    camDirection = new Vector3 (rotdiff*30f, 0, 0);


			rearcam.transform.Rotate (camDirection);
			maincam.transform.Rotate (camDirection);

		


		}
*/


		/*
		if (lefty==0 && leftx==0) {
			rearcam.transform.rotation = new Quaternion ();
			maincam.transform.rotation = new Quaternion ();
			rotcur = 0f;

		}
		*/

		/*
		if (lefty ==-1) {
			if (!startrotationup) {
				startrotationdown = false;
				startrotationup = true;
				camcount = 0;
			}
			if (startrotationup && camcount<20) {
				camDirection = new Vector3 (-2f, 0, 0);
        		rearcam.transform.Rotate (camDirection);
				maincam.transform.Rotate (camDirection);
				camcount++;
			}



			
		}
		*/

		if (leftx == -1 && startrotationdown) {
			charDirection = new Vector3 (0, -0.5f, 0);
			transform.Rotate (charDirection);
		}else if (leftx == -1 && !startrotationdown) {
			charDirection = new Vector3 (0, -2.5f, 0);
			transform.Rotate (charDirection);
		}
			

		if (leftx == 1 && startrotationdown) {
			charDirection = new Vector3 (0, 0.5f, 0);
			transform.Rotate (charDirection);
		}else if (leftx == 1 && !startrotationdown) {
			charDirection = new Vector3 (0, 2.5f, 0);
			transform.Rotate (charDirection);
		}


		if (lefty == 1 ) {
			if (!startrotationdown) {
				startrotationup = false;
				startrotationdown = true;
				camcount = 0;
			}
			if (startrotationdown && camcount<20) {
				camDirection = new Vector3 (2f, 0, 0);
				rearcam.transform.Rotate (camDirection);
				maincam.transform.Rotate (camDirection);
				camcount++;
			}




		}

		/*

		if (lefty>-1 && lefty<1) {
			rearcam.transform.rotation = new Quaternion ();
			maincam.transform.rotation = new Quaternion ();
			rotcur = 0f;
			startrotationup = false;
			startrotationdown = false;
			camreset = true;
		}
	*/
		if (lefty >= -1 && lefty < 1) {

			rotcur = 0f;
			startrotationup = false;
			startrotationdown = false;

			if (camcount > 0) {
				camDirection = new Vector3 (-2f, 0, 0);
				rearcam.transform.Rotate (camDirection);
				maincam.transform.Rotate (camDirection);
				camcount--;
			} else {
			//	rearcam.transform.rotation = new Quaternion ();
			//	maincam.transform.rotation = new Quaternion ();

			}


		}


		if (!attacking &&(Input.GetKey ("up") || righty<-0.8 )) {
			//transofrms Vector3.forward moves along its local z
			//controller's Vector3.forward moves along the worlds z
			attacking = false;
			if ((Input.GetKeyDown ("space") || CrossPlatformInputManager.GetButtonDown("Jump") || Input.GetButtonDown("xboxa") )&& stopgravity == false && mycontroller.isGrounded) {
				stopgravity = true;
				icounter = 0;
				runSpeed = 0.25f;
			}else if (!attacking && (Input.GetKeyDown ("a") || Input.GetButtonDown("xboxrb") || CrossPlatformInputManager.GetButtonDown("Attack"))) {
				GetComponent<AudioSource> ().Stop ();
				waitsound = false;
				StopCoroutine (co);
				StartCoroutine (swipesound());
				StartCoroutine (waitattacking());


			} 

			if (!mycontroller.isGrounded && iscolliding) {
				//do nothing
			} else if (!attacking ){
				GetComponent<Animation> ().Play ("Run");
				charDirection = transform.TransformDirection (Vector3.forward);

				if (startrotationdown)
					runSpeed=0.05f;
				else 
					runSpeed=0.15f;

				mycontroller.Move (charDirection * runSpeed);

			
				if (waitsound == false) {
					co = playingsound ();
					StartCoroutine (co);
				}


			}


		} else if (Input.GetKey ("down")||righty>0.8) {

			if (!attacking)
			  GetComponent<Animation> ().Play ("Run");


			charDirection = transform.TransformDirection (Vector3.back);

			if (startrotationdown)
				runSpeed=0.05f;
			else 
				runSpeed=0.15f;
			
			mycontroller.Move (charDirection * runSpeed);

		} else if (Input.GetKeyDown ("c")|| Input.GetButtonDown("xboxlb") ||CrossPlatformInputManager.GetButtonDown("touchcamera") ) {
			ChangeCam ();



		} else if (CrossPlatformInputManager.GetButtonDown("exitbutton") ) {
			exitscreen.SetActive (true);
			buttonControl.enabled = true;
			msc.enabled=false;

			//Application.Quit();

		}else if (Input.GetKeyDown (KeyCode.Escape)||CrossPlatformInputManager.GetButtonDown("exitbutton2") ) {
			Debug.Log ("exiting");
			Application.Quit();

		}else if (Input.GetKeyDown ("m")  || CrossPlatformInputManager.GetButtonDown("musicbutton") ) {
			Debug.Log ("music!!");
			AudioListener.pause = !AudioListener.pause;
			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;


		}else if (CrossPlatformInputManager.GetButtonDown("building1") ) {
			GameObject.Find ("mutebutton").transform.rotation = new Quaternion ();

			buildingtype = "mybridge";
			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;



		}else if (CrossPlatformInputManager.GetButtonDown("building2") ) {
			GameObject.Find ("mutebutton").transform.rotation = new Quaternion ();
			GameObject.Find ("mutebutton").transform.Rotate(new Vector3(0,0,90));

			buildingtype = "mybridge 1";
			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;



		}else if (CrossPlatformInputManager.GetButtonDown("building3") ) {
			GameObject.Find ("mutebutton").transform.rotation = new Quaternion ();
			GameObject.Find ("mutebutton").transform.Rotate(new Vector3(0,0,-180));

			buildingtype = "mybridge 2";
			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;



		}else if (CrossPlatformInputManager.GetButtonDown("timebutton") ) {

			if (skycount == 0) {
				RenderSettings.skybox = sky2;
				skycount = 1;
			} else if (skycount == 1) {
				RenderSettings.skybox = sky1;
				skycount = 0;
			}
			DynamicGI.UpdateEnvironment ();


			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;



		}else if (CrossPlatformInputManager.GetButtonDown("undobutton")) {



			if (blockX.tag == "bridge" && !undoflag && blockX.GetComponent<Bridgescript> ().created) {
				undoflag = true;
				mystring.count--;
				Destroy (blockX);
									
			}else if (blockX.tag == "bridge1" && !undoflag && blockX.GetComponent<Bridgescript1> ().created){
				undoflag = true;
				mystring.count--;
				Destroy (blockX);

			}else if (blockX.tag == "bridge2" && !undoflag && blockX.GetComponent<Bridgescript2> ().created){
				undoflag = true;
				mystring.count--;
				Destroy (blockX);

			}

		


		} 
		else if (Input.GetKeyDown ("s") || Input.GetButtonDown("xboxx") ||CrossPlatformInputManager.GetButtonDown("spawnbutton")) {
			
			spawn ();

			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;




		}else if (CrossPlatformInputManager.GetButtonDown("backbutton")) {

			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;




		}else if (CrossPlatformInputManager.GetButtonDown("savebutton")) {
			save ();
			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;




		}else if (CrossPlatformInputManager.GetButtonDown("loadbutton")) {
			load ();
			exitscreen.SetActive (false);
			msc.enabled=true;
			buttonControl.enabled=false;




		}else if ((Input.GetKeyDown ("space") ||CrossPlatformInputManager.GetButtonDown("Jump") || Input.GetButtonDown("xboxa") ) && stopgravity == false && mycontroller.isGrounded) {
			GetComponent<Animation> ().Play ("Run");
			stopgravity = true;
			icounter = 0;

		} else if (!attacking && (Input.GetKeyDown ("a") || Input.GetButtonDown("xboxrb") || CrossPlatformInputManager.GetButtonDown("Attack"))) {
			//Handheld.Vibrate ();
			
			//attacking=true;
			//GetComponent<Animation> ().Play ("Attack");

			  StartCoroutine (waitattacking());
			StartCoroutine (swipesound());

		} else if ((Input.GetKeyDown ("y") || Input.GetButtonDown("xboxy") || CrossPlatformInputManager.GetButtonDown("mutebutton"))&& mystring.count<100) {
			Vector3 temp9 = new Vector3 ();
			blockX=Instantiate(Resources.Load(buildingtype)) as GameObject; 
			undoflag = false;

			if (buildingtype == "mybridge") {
				blockX.transform.rotation = transform.rotation;
				temp9=transform.position + transform.TransformDirection (Vector3.up) * 2f;
				blockX.transform.position = temp9;
				blockX.GetComponent<Bridgescript> ().setoriginal (temp9, transform.rotation);

			}else if (buildingtype == "mybridge 1") {
				blockX.transform.rotation = transform.rotation;
				rotatebridge = new Vector3 (-20, 0, 0);
				blockX.transform.Rotate (rotatebridge);
				temp9 = transform.position + transform.TransformDirection (Vector3.up) * 2f;
				blockX.transform.position = temp9;



				blockX.GetComponent<Bridgescript1> ().setoriginal (temp9, blockX.transform.rotation);

			}else if (buildingtype == "mybridge 2") {
				blockX.transform.rotation = transform.rotation;
				rotatebridge = new Vector3 (20, 0, 0);
				blockX.transform.Rotate (rotatebridge);

				temp9 = transform.position + transform.TransformDirection (Vector3.up) * 4f;
			//		transform.TransformDirection (Vector3.forward) * 1.7f;
				blockX.transform.position = temp9;



				blockX.GetComponent<Bridgescript2> ().setoriginal (temp9, blockX.transform.rotation);
				
			}



		}else if (Input.GetKeyDown("i")){
			invertcont=!invertcont;
			Debug.Log ("controller inversion: " + invertcont);
		}
		else if (!attacking && mycontroller.isGrounded && righty==0 && rightx==0){;
			 
			GetComponent<Animation> ().Play ("idle");
			runSpeed = 0.0f;
			GetComponent<AudioSource> ().Stop ();
			waitsound = false;
			StopCoroutine (co);


		}




		if (rightx>0.8 && righty>-0.8 && !attacking) {
			Debug.Log ("going right " + rightx);

			  GetComponent<Animation> ().Play ("Run");

			charDirection = transform.TransformDirection (Vector3.right);

			if (startrotationdown)
				runSpeed=0.05f;
			else 
				runSpeed=0.1f;

			mycontroller.Move (charDirection * runSpeed);

		} else if (rightx<-0.8 && righty>-0.8  && !attacking) {
			Debug.Log ("going lefts " + rightx);
		
			  GetComponent<Animation> ().Play ("Run");


			charDirection = transform.TransformDirection (Vector3.left);

			if (startrotationdown)
				runSpeed=0.05f;
			else 
				runSpeed=0.1f;

			mycontroller.Move (charDirection * runSpeed);

		} 









		if (mycontroller.isGrounded==false && stopgravity==false &&!hit2air) {
			airtime++;
			charDirection=transform.TransformDirection(Vector3.down);

			if (airtime < 21) {
				mycontroller.Move (charDirection * 0.17f);

			//	charDirection = transform.TransformDirection (Vector3.forward);
			//	mycontroller.Move (charDirection * runSpeed*0.2f);
			
			}
			else 
				mycontroller.Move(charDirection*0.6f);

		
		}




		/*
		if (mycontroller.isGrounded == false && stopgravity == false && hit2air) {
			charDirection = transform.TransformDirection (Vector3.down);
			mycontroller.Move (charDirection * 0.2f);
			Debug.Log ("hit to air-------------------------");
		} else if (mycontroller.isGrounded && stopgravity == false && hit2air) {
			
			hit2air = false;
			Debug.Log ("back from airr-------------------------");
		}

       */



		if (stopgravity == true && icounter < 13) {
			charDirection = transform.TransformDirection (Vector3.up);
			mycontroller.Move (charDirection * 0.27f);
			icounter++;
		//	charDirection = transform.TransformDirection (Vector3.forward);
		//	mycontroller.Move (charDirection * runSpeed*0.2f);

		}
	  
		else if (stopgravity == true & icounter ==13) {
			
			stopgravity = false;
			//runSpeed=0.05f;
			airtime = 0;
			
		}

		if (health < 0) {
			health = 150;
			spawn ();
		}

		if (inpit) {
			inpit = false;
			spawn ();
		}

		if (is_hit) {
		/*	charDirection = transform.TransformDirection (Vector3.back);
			mycontroller.Move (charDirection * 0.5f);
			charDirection = transform.TransformDirection (Vector3.up);
			mycontroller.Move (charDirection * 0.5f);
		    hit2air = true;
		    is_hit = false;
        */
			is_hit = false;
		}

		//Debug.Log ("triggers "+joytrigg);
	}
}

[Serializable]
class MyStringmobile{
	/*
	public Vector3[] location = new Vector3[50];
	public Quaternion[] rotation = new Quaternion[50];
	public string[] type = new string[50];
	public int count=0;
    */
	public float[] locationx = new float[100];
	public float[] locationy = new float[100];
	public float[] locationz = new float[100];

	public float[] rotationx = new float[100];
	public float[] rotationy = new float[100];
	public float[] rotationz = new float[100];
	public float[] rotationw = new float[100];

	public string[] type = new string[100];
	public int count=0;

}
