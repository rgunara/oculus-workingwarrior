using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class lightsript : MonoBehaviour {
	private GameObject samurai;
	Vector3 charDirection, spawnto;
	public Image image;
	// Use this for initialization
	void Start () {
		samurai = GameObject.Find ("MYSamurai");
		spawnto = transform.position;
		image = this.GetComponent<Image> ();
	}


	//Update is called once per frame
	void Update () {
		//GetComponent<Text> ().text = ""+samurai.GetComponent<samuraiscript>().gethealth().ToString()+"%";

		if (samurai.GetComponent<samuraiscript> ().charhit ()) {
			image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
		} else {
			image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

		}
			

		
	}
}
