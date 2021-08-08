using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class controller : MonoBehaviour {
	RaycastHit hit;
	GameObject gob, startobj, startobji, panelobj, newobj, newobji;
	Quaternion originalrot;
	bool ready;
	bool loadactive;

	// Use this for initialization


	IEnumerator waitloading (){
		yield return new WaitForSeconds (1);
		ready = true;
	}






	void Start () {
		gob = null;
		originalrot = Quaternion.Euler (0, 0, 0);
		startobj=GameObject.Find ("TestCube");
		startobji=GameObject.Find("TestCubeInvisible");
		newobj=GameObject.Find ("NewGame");
		newobji=GameObject.Find ("TestCubeInvisibleNew");
		panelobj = GameObject.Find ("Panel");
		ready = false;
		//StartCoroutine (waitloading ());


		if (!File.Exists (Application.persistentDataPath + "/locationData.dat") && !File.Exists (Application.persistentDataPath + "/bridgeData.dat")) {
			startobj.SetActive (false);
			startobji.SetActive (false);
			newobj.transform.position = new Vector3 (-1.52f, 2.93f, 2.89f);
			newobji.transform.position = new Vector3 (-1.52f, 2.93f, 2.89f);
			loadactive = false;
		} else
			loadactive = true;




	}




	void resetrotation(GameObject ob){

		if (ob.transform.rotation.eulerAngles.x >2)
			ob.transform.Rotate (Vector3.right*1f);


	}
	// Update is called once per frame
	void Update () {
		if (!ready)
			StartCoroutine (waitloading ());



		if (OVRInput.GetDown (OVRInput.Button.Back) && ready)
		    OVRManager.PlatformUIConfirmQuit();
		

		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			if (hit.collider != null) {
				if (gob != hit.collider.gameObject) {
					gob = hit.collider.gameObject;
				}

			} else
				gob = null;
		} else
			gob = null;




		if (gob != null) {

			if (gob.name == "TestCube" || gob.name == "TestCubeInvisible") {
				startobj.transform.Rotate (Vector3.right * 2f);


			} else if (gob.name == "NewGame" || gob.name == "TestCubeInvisibleNew") {
				newobj.transform.Rotate (Vector3.right * 2f);


			}



			// reset rotations
			if (loadactive) {
				if (gob.name != "TestCube" || gob.name != "TestCubeInvisible") {
					resetrotation (startobj);


				}
			}

			if (gob.name != "NewGame" || gob.name != "TestCubeInvisibleNew") {
				resetrotation (newobj);


			}





			if ((gob.name == "TestCube" || gob.name == "TestCubeInvisible") && OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger)) {
				transform.gameObject.SetActive (false);
				panelobj.GetComponent<Image> ().enabled = true;
				MyStaticClass.loadstatus = 1;
				SceneManager.LoadScene (1);
			} else if ((gob.name == "NewGame" || gob.name == "TestCubeInvisibleNew") && OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger)) {
				transform.gameObject.SetActive (false);
				panelobj.GetComponent<Image> ().enabled = true;
				MyStaticClass.loadstatus = 0;
				SceneManager.LoadScene (1);
			}


		



		} else {
			if (loadactive)
			   resetrotation (startobj);
			
			resetrotation (newobj);

		}


	}
}
