using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class destroycontactandinitiate : MonoBehaviour {
	
	private GameObject newstars;



	void OnTriggerEnter (Collider other)
	{


		if (other.tag == "star")
		{
			newstars = other.gameObject;
			newstars.transform.localScale = new Vector3 (other.transform.localScale.x/2, other.transform.localScale.y/2, other.transform.localScale.z/2);

			Instantiate(newstars, other.transform.position, other.transform.rotation);
			Instantiate(newstars, other.transform.position, other.transform.rotation);
			Destroy (other.gameObject);
			Destroy (gameObject);

			AudioClip bling = Resources.Load("starso") as AudioClip;
			AudioSource.PlayClipAtPoint(bling,new Vector3(0,0,0));


			pointver2.points+=1;
			print (pointver2.points);
		}



	}
}
