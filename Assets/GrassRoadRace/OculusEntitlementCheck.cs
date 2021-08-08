using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;

//note: Must have Oculus Platform SDK for Unity installed!

public class OculusEntitlementCheck : MonoBehaviour {

	// Use this for initialization

	public GameObject panel;
	bool response;

	IEnumerator loadmsg (){
		panel.SetActive(true);
		yield return new WaitForSeconds (2);
		UnityEngine.Application.Quit();
	}



	void Start () {

		response = true;


		Core.Initialize("2384842961596707");
		Entitlements.IsUserEntitledToApplication().OnComplete(
			(Message msg) =>
			{
				if (msg.IsError)
				{
					print("fired oculus platform, is not entitled");
					// User is NOT entitled.
					response=false;

				} else 
				{   
					
					response=true;
					print("Oculus platform enetitlement check passed");

				}
			}
		);
	}

	void Update () {
		if (!response)
			StartCoroutine (loadmsg ());



	}



}