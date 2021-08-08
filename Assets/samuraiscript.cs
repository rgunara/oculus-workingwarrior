using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
public class samuraiscript : MonoBehaviour {

	// Notes:  Original Rear camera:  -0.8899665, 1.88, -1.9 
	//         Original Main camera: -0.7, 1.5. -1


	String typepanel="";

	GameObject welcomepanel1, welcomepanel2, finishedpanel;
	bool gamefinished;

	GameObject healthpanel, healthbox, enemybox;

	Vector2 touchpad;
	GameObject cameraStand, cameraRig, mygearcontroller, Samhead, panelLoad, trackingspace, mygearcontrollerleft;
	private float runSpeed= 0.15f;
	private float mousex, mousey, gearx, gearxleft;
	int score, health, airtime, camcount;
	CharacterController mycontroller;
	Vector3 charDirection, camDirection, spawnto, temprot, rotatebridge, cameraPosition, controlerrotation;
	private Camera maincam, rearcam;
	bool stopgravity, iscolliding, attacking, inpit, is_hit, hit2air, onbridge;
    bool invertcont;
	private int icounter;
	private int triggercounter;
	Quaternion spawnrotation, camrotation;
	GameObject blockX;
	float leftx, lefty, rightx, righty, joytrigg, dpadx, dpady;
	public AudioClip sound1, sound2, sound3;
	bool waitsound;
	IEnumerator co;
	string where, buildingtype;
	bool startrotationup, startrotationdown, camreset;
	float rotprev, rotcur, rotdiff, turnprev, turncur, turndiff;
	int skycount=0;
	int accelCount=0;
	int accelNegCount=0;
	int accelPosCount=0;
	int isjoggingcount=20;

	bool islanding=false;
	GameObject exitscreen;
	Canvas msc, buttonControl;
	MyString mystring, mystring2, mylastpos;
	public Material sky1, sky2;

	Vector3 phoneAccel;

	bool undoflag=true;
	int bb=0; 
	bool isloading=false;
	bool isjogging=false;
	String whatsound="";
	int countungrounded=0;
	bool isloaded=false;


	void Start () {
		gamefinished = false;
		AudioListener.pause = true;
		Samhead = GameObject.Find ("Bip001 Head Sam");
		cameraStand= GameObject.Find ("CameraStand");
		cameraRig= GameObject.Find ("OVRCameraRig");
		trackingspace=GameObject.Find ("TrackingSpace");
		mygearcontroller=GameObject.Find("mycontroller");
		mygearcontrollerleft=GameObject.Find("mycontrollerleft");
		welcomepanel1 = GameObject.Find ("WelcomePanel1");
		welcomepanel2 = GameObject.Find ("WelcomePanel2");
		finishedpanel = GameObject.Find ("FinishedPanel");


		healthpanel = GameObject.Find ("Panel (1)");
		healthbox = GameObject.Find ("HealthText");
		enemybox = GameObject.Find ("EnemyHealth");


		mycontroller = GetComponent<CharacterController> ();
		maincam = Camera.allCameras[0];
	//	rearcam = Camera.allCameras[1];
	//	maincam.enabled = true;
	//	rearcam.enabled = false;
		stopgravity = false;
		icounter = 0;
		iscolliding = false;
		triggercounter = 0;
		spawnrotation = transform.rotation;
		attacking = false;
		inpit = false;
		dpadx = 0f;
		dpady = 0f;
		leftx = 0f;
		rightx = 0f;
		lefty = 0f;
		righty = 0f;
		invertcont = false;
		score = 0;
		health = 150;
		spawnto = transform.position;
		camrotation = maincam.transform.rotation;

		waitsound = false;
		is_hit = false;
		hit2air = false;

		airtime = 15;
		co = playingsound ();
		//StartCoroutine (co);
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
	//	exitscreen = GameObject.Find ("exitscreen");
	//	exitscreen.SetActive (false);
	//	msc = GameObject.Find ("MobileSingleStickControl").GetComponent<Canvas>();
	//	buttonControl = GameObject.Find ("ButtonControl").GetComponent<Canvas>();
	//	buttonControl.enabled = false;
		buildingtype = "mybridge";
		mystring = new MyString ();
		mylastpos = new MyString ();
		ChangeCam ();
		panelLoad = GameObject.Find ("PanelLoad");

		if (MyStaticClass.loadstatus == 1) {
			load ();

			StartCoroutine (waitloading ());


		} else {
			
			AudioListener.pause = false;
			spawn ();
			panelLoad.GetComponent<Image> ().enabled = false;
			typepanel = "panel1";

			welcomepanel1.GetComponent<Image> ().enabled = true;

		}
		
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






	IEnumerator waitloading (){
		yield return new WaitForSeconds (1);
		load2 ();
		yield return new WaitForSeconds (0.5f);
		panelLoad.GetComponent<Image> ().enabled = false;
		AudioListener.pause = false;
	}


	IEnumerator waitattacking (){
		attacking = true;
		GetComponent<Animation> ().Play ("Attack2");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Attack2").length);
		attacking = false;
	

	}

	IEnumerator exploding (){
		GameObject exp = Instantiate (Resources.Load ("Explosion")) as GameObject;
		exp.transform.position = transform.position;
		exp.transform.rotation = transform.rotation;
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Attack2").length+2);
		Destroy (exp);


	}


	IEnumerator waitjumping (){
		
		GetComponent<Animation> ().Play ("Run");
		yield return new WaitForSeconds (GetComponent<Animation>().GetClip("Run").length);



	}

	IEnumerator swipesound (){
		whatsound = "swipe";
		yield return new WaitForSeconds (sound2.length*8f);
		for (int i = 0; i < 1; i++) {
			GetComponent<AudioSource> ().PlayOneShot (sound2);
			yield return new WaitForSeconds (sound2.length*8f);
			//GetComponent<AudioSource> ().Stop ();
		}

		whatsound = "";
	}


	IEnumerator playingsound (){
		
		whatsound = "running";
		waitsound = true;
		GetComponent<AudioSource> ().PlayOneShot (sound1);
		yield return new WaitForSeconds (sound1.length);

		waitsound = false;
		whatsound = "";

	}

	IEnumerator playingsound2 (){
		whatsound = "bridge";
		waitsound = true;
		GetComponent<AudioSource> ().PlayOneShot (sound3);
		yield return new WaitForSeconds (sound3.length);

		waitsound = false;
		whatsound = "";

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


		save ();


		if (waitsound == false) {
			co = playingsound2 ();
			StartCoroutine (co);
		}


		//save after each building






		
		Debug.Log ("setdata " + mystring.count);
	}


	public void setscore(int points){
		score = score + points;
	}


	public string getscore(){
		//return score.ToString() +"MS "+gearx.ToString()+ " >";

		//return score.ToString() +"MS: "+mystring.count.ToString()+ " MS2: "+mystring2.count.ToString();

		return "Accel: " + phoneAccel.ToString();

	}


	public string getbld(){
		int tempbld = 200 - mystring.count;
		return tempbld.ToString();
	}

	public string getbldtype(){
		return bb.ToString ();
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

	public void save2(){
		mylastpos.locationx[0] = transform.position.x;
		mylastpos.locationy[0] = transform.position.y;
		mylastpos.locationz[0] = transform.position.z;

		mylastpos.rotationx [0] = transform.rotation.x;
		mylastpos.rotationy [0] = transform.rotation.y;
		mylastpos.rotationz [0] = transform.rotation.z;
		mylastpos.rotationw [0] = transform.rotation.w;

		Debug.Log ("saving2....");
		BinaryFormatter bf2 = new BinaryFormatter ();
		FileStream file2 = File.Create (Application.persistentDataPath + "/locationData.dat");

		bf2.Serialize (file2, mylastpos);
		file2.Close ();

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
				//undoflag = false;
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
				//undoflag = false;
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
				//undoflag = false;

			}

		}
		yield return new WaitForSeconds(0.1f);
	}

	public void load(){
		isloading = true;
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

				//don't want to allow any undo on load so keeping it commented
				//undoflag = false;
		
			}

			//spawn


		} else
			Debug.Log ("No file to load");

	}



	public void load2(){
		
		if (File.Exists (Application.persistentDataPath + "/locationData.dat")) {
			BinaryFormatter bf2 = new BinaryFormatter ();
			FileStream file2 = File.Open (Application.persistentDataPath + "/locationData.dat", FileMode.Open);
			MyString tempstring = new MyString ();
			tempstring = (MyString)bf2.Deserialize (file2);
			file2.Close ();
			transform.position = new Vector3 (tempstring.locationx [0], tempstring.locationy [0], tempstring.locationz [0]);
			mycontroller.Move (new Vector3 (0f, 0f, 0f));
			Quaternion temprot = new Quaternion (tempstring.rotationx[0], tempstring.rotationy[0], tempstring.rotationz[0], tempstring.rotationw[0]);
			trackingspace.transform.rotation = temprot;




		} else {
			Debug.Log ("No file to load");
			spawn ();
		}
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

	  //  Vector3 scalex = transform.localScale;
	//	scalex.x = scalex.x * (-1);
	//	transform.localScale = scalex;

		if (skycount == 0) {
		//	RenderSettings.skybox = sky2;
			skycount = 1;
		} else if (skycount == 1) {
		//	RenderSettings.skybox = sky1;
			skycount = 0;
		}
		RenderSettings.skybox = sky2;
		//DynamicGI.UpdateEnvironment ();

	}

	void spawn(){

		/*
		transform.position = spawnto;
		transform.rotation = spawnrotation;
		*/
		transform.position = new Vector3 (57f, 0.58f, -210f);
		transform.rotation = spawnrotation;

		mycontroller.Move (new Vector3 (0f, 0f, 0f));
		Debug.Log ("isgrounded value " + mycontroller.isGrounded);
		//StartCoroutine(exploding());
	}


	public bool isattacking(){
		return attacking;
	}







	void OnApplicationPause(){

		save2 ();

	}


	IEnumerator switchpanel1 (){
		
		yield return new WaitForSeconds (0.5f);
		welcomepanel1.GetComponent<Image> ().enabled = false;
		welcomepanel2.GetComponent<Image> ().enabled = true;

		typepanel = "panel2";
	}

	IEnumerator switchpanel2(){
		
		yield return new WaitForSeconds (0.5f);
		welcomepanel2.GetComponent<Image> ().enabled = false;
		typepanel = "";
	}

	IEnumerator switchpanel3(){

		yield return new WaitForSeconds (0.5f);
		finishedpanel.GetComponent<Image> ().enabled = false;
		typepanel = "";
	}



	string healthbar(int v){
		string bar = "";
		for (int i = 1; i <= Mathf.CeilToInt(v/6); i++) {
			bar=bar+"I";

		}
		return bar;
	}



	// Update is called once per frame
	void Update () {
		//mousex = Input.GetAxis ("Mouse X");
		//phoneAccel = Input.acceleration;

		// Game Messages
		if (typepanel=="panel1" && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)){
			StartCoroutine (switchpanel1 ());
		}else if (typepanel=="panel2" && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)){
			StartCoroutine (switchpanel2 ());
		}else if (typepanel=="panel3" && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)){
			StartCoroutine (switchpanel3 ());
		}


		// health
		if (enemybox.GetComponent<Text> ().text != "") {
			healthpanel.GetComponent<Image> ().enabled = true;
			healthbox.GetComponent<Text> ().text = healthbar (gethealth ());
		} else {
			healthpanel.GetComponent<Image> ().enabled = false;
			healthbox.GetComponent<Text> ().text = "";
		}


		if (mystring.count == 10 && !gamefinished) {
			finishedpanel.GetComponent<Image> ().enabled = true;
			gamefinished = true;
			typepanel = "panel3";

		}


		phoneAccel = OVRManager.display.acceleration;


		


		touchpad = OVRInput.Get (OVRInput.Axis2D.PrimaryTouchpad);
		if (touchpad.x > 0.8)
			buildingtype = "mybridge";
		else if (touchpad.y > 0.8)
			buildingtype = "mybridge 1";
		else if (touchpad.y < -0.8)
			buildingtype = "mybridge 2";


		//mousey = CrossPlatformInputManager.GetAxisRaw ("Mouse Y") * (-1);
	//	Debug.Log ("mousex: "+mousex + ",  mousey :" + mousey);
	//	camDirection = new Vector3 (mousey, 0, 0);
	//	rearcam.transform.Rotate (camDirection);



		mygearcontroller.transform.rotation = OVRInput.GetLocalControllerRotation( OVRInput.Controller.RTrackedRemote);


		gearx=mygearcontroller.transform.forward.y;

		mygearcontrollerleft.transform.rotation = OVRInput.GetLocalControllerRotation( OVRInput.Controller.LTrackedRemote);
		gearxleft=mygearcontrollerleft.transform.forward.y;

		//Debug.Log ("gearx  " + gearx);

		dpadx=Input.GetAxis ("itemsx");
		dpady=Input.GetAxis ("itemsy");

		if (dpadx == 1 || dpadx == -1)
			buildingtype = "mybridge";
		else if (dpady == -1)
			buildingtype = "mybridge 1";
		else if (dpady == 1)
			buildingtype = "mybridge 2";



        // rightjoyx for android is the right stick left to right axis. This is different for PC
		if (invertcont) {
			

	
			leftx = Input.GetAxis ("left joy x");
			lefty = Input.GetAxis ("left joy y");
			righty = Input.GetAxis ("right joy y");
			rightx = Input.GetAxis ("right joy x");
		

		} else {
			leftx = Input.GetAxis ("right joy x");
			lefty = Input.GetAxis ("right joy y");
			//charDirection = new Vector3 (0, leftx * 0.05f, 0);
			//transform.Rotate (charDirection);
			//transform.rotation=Quaternion.Euler(maincam.transform.rotation.eulerAngles);
	

			/*
			charDirection = new Vector3 (transform.rotation.eulerAngles.x, maincam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			transform.rotation = Quaternion.Euler (charDirection);
			cameraPosition = new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z);
			//maincam.transform.position = cameraPosition;
			cameraStand.transform.position=cameraPosition;
            */



			//cameraRig
			charDirection = new Vector3 (transform.rotation.eulerAngles.x, maincam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			transform.rotation = Quaternion.Euler (charDirection);
			//cameraPosition = new Vector3 (Samhead.transform.position.x, Samhead.transform.position.y +0.5f, Samhead.transform.position.z);
			cameraPosition = new Vector3 (transform.position.x, transform.position.y +2f, transform.position.z);

			cameraRig.transform.position=cameraPosition;
	




			//Debug.Log ("Vector forward "+ mygearcontroller.transform.forward);

			rightx = Input.GetAxis ("left joy x");
			righty = Input.GetAxis ("left joy y");
		}



		//used to be 25f
		if (phoneAccel.sqrMagnitude > 12f && Mathf.Abs (phoneAccel.y) > 5f) {
			isjoggingcount = 0;

			if (attacking) {
				StopCoroutine (waitattacking ());
				attacking = false;

			}


		}
		
        


		if (!attacking &&(Input.GetKey ("up") || righty<-0.8 || isjoggingcount<20)) {
			
			isjogging = true;
			isjoggingcount++;



			//(mygearcontroller.transform.rotation.eulerAngles.x>180 && mygearcontroller.transform.rotation.eulerAngles.x<270 )   

			//transofrms Vector3.forward moves along its local z
			//controller's Vector3.forward moves along the worlds z
			attacking = false;
			if (typepanel== "" &&(Input.GetKeyDown ("space")  || Input.GetButtonDown("xboxa") || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) )&& stopgravity == false && mycontroller.isGrounded) {
				stopgravity = true;
				icounter = 0;
				runSpeed = 0.25f;


				if (whatsound == "running") {
					GetComponent<AudioSource> ().Stop ();
					StopCoroutine (co);
					waitsound = false;
					whatsound = "";
				}



			}else if (!attacking && !isjogging &&  (Input.GetKeyDown ("a") || Input.GetButtonDown("xboxrb") || (gearx>0.85f && gearx<=1f)  || (gearxleft>0.85f && gearxleft<=1f)   )) {


				GetComponent<AudioSource> ().Stop ();
				StopCoroutine (co);
				waitsound = false;
				whatsound = "";

				/*
				StopCoroutine (co);
				StartCoroutine (swipesound());
                */


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

			 //running sound
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

		} else if (Input.GetKeyDown ("c") ) {
			ChangeCam ();



		} else if (Input.GetKeyDown (KeyCode.Escape)) {
			//Debug.Log ("exiting");
			//Application.Quit();

		}else if (Input.GetKeyDown ("m")   ) {
			Debug.Log ("music!!");
			AudioListener.pause = !AudioListener.pause;



		}
		else if (Input.GetKeyDown ("s") || Input.GetButtonDown("xboxx") ) {

			spawn ();






		}else if (Input.GetKeyDown ("z")) {
			save ();





		}else if (Input.GetKeyDown ("x") && !isloading) {
			load ();




		}else if (typepanel=="" && (Input.GetKeyDown ("space") || Input.GetButtonDown("xboxa") || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) ) && stopgravity == false && mycontroller.isGrounded) {
			GetComponent<Animation> ().Play ("Run");
			stopgravity = true;
			icounter = 0;

		} else if (!attacking && !isjogging &&  (Input.GetKeyDown ("a") || Input.GetButtonDown("xboxrb") || (gearx>0.85f && gearx<=1f) || (gearxleft>0.85f && gearxleft<=1f)   )) {
			//Handheld.Vibrate ();
			
			//attacking=true;
			//GetComponent<Animation> ().Play ("Attack");

		
			GetComponent<AudioSource> ().Stop ();
			StopCoroutine (co);
			waitsound = false;
			whatsound = "";

			/*
			StopCoroutine (co);
			StartCoroutine (swipesound());
		    */

			  StartCoroutine (waitattacking());


		} else if (typepanel == "" &&(Input.GetKeyDown ("y") || Input.GetButtonDown("xboxy") || (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad) && touchpad.x > -0.8)              )&& mystring.count<200) {



			Debug.Log ("shelll");
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
		}else if (Input.GetButtonDown("xboxback")  ) {
			bb = bb + 1;
			if (bb == 3)
				bb = 0;



		} else if (Input.GetButtonDown("xboxb") || (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad) && touchpad.x < -0.8) ) {



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
		
			save ();


		} 


		else if (!attacking && mycontroller.isGrounded && righty==0 && rightx==0){;

			GetComponent<Animation> ().Play ("idle");
			isjogging = false;
			runSpeed = 0.0f;
			if (whatsound == "running") {
				Debug.Log ("stopped in idle");
				GetComponent<AudioSource> ().Stop ();
				StopCoroutine (co);
				waitsound = false;
				whatsound = "";
			}

		}







		if (Input.GetButton("xboxback") && Input.GetButton("xboxstart")) {
			Debug.Log ("exiting");
			Application.Quit();

		}else if (Input.GetButton("xboxa") && Input.GetButtonDown("xboxrb")) {
			spawn ();
			save ();

		}else if (Input.GetButton("xboxx") && Input.GetButtonDown("xboxrb") && !isloading) {
			spawn ();
			load ();

		}


		if (OVRInput.GetUp (OVRInput.Button.Back) || Input.GetKeyDown (KeyCode.Escape)) {
			
			save2 ();
			SceneManager.LoadScene (0);

		}


		if (!OVRPlugin.userPresent) {
			
			save2 ();
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


			if (whatsound == "running" && countungrounded>2) {
				Debug.Log ("stopped in gravity");
				GetComponent<AudioSource> ().Stop ();
				StopCoroutine (co);
				waitsound = false;

				whatsound = "";
			}


			countungrounded++;
			//Debug.Log ("Ungrounded time" + countungrounded);


			airtime++;
			charDirection=transform.TransformDirection(Vector3.down);

			if (airtime < 21) {
				mycontroller.Move (charDirection * 0.17f);

			//	charDirection = transform.TransformDirection (Vector3.forward);
			//	mycontroller.Move (charDirection * runSpeed*0.2f);
			
			}
			else {
				mycontroller.Move(charDirection*0.5f);

			}
		
		}

		if (mycontroller.isGrounded)
			countungrounded = 0;


		  



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
class MyString{
	/*
	public Vector3[] location = new Vector3[50];
	public Quaternion[] rotation = new Quaternion[50];
	public string[] type = new string[50];
	public int count=0;
    */
	public float[] locationx = new float[200];
	public float[] locationy = new float[200];
	public float[] locationz = new float[200];

	public float[] rotationx = new float[200];
	public float[] rotationy = new float[200];
	public float[] rotationz = new float[200];
	public float[] rotationw = new float[200];

	public string[] type = new string[200];
	public int count=0;

}
