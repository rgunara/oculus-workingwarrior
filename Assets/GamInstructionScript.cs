using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GamInstructionScript : MonoBehaviour {

	public Material oculus;
	public GameObject obj;
	public GameObject panel;
	// Use this for initialization
	void Start () {
		if (OVRPlugin.GetSystemHeadsetType () == OVRPlugin.SystemHeadset.Oculus_Go) {
			obj.GetComponent<Renderer> ().material = oculus;

		}
	}

	// Update is called once per frame
	void Update () {

		if (!OVRInput.IsControllerConnected (OVRInput.Controller.RTrackedRemote) && !OVRInput.IsControllerConnected (OVRInput.Controller.LTrackedRemote))
			panel.SetActive (true);
		else
			panel.SetActive (false);
		
	}
}
