using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructstarmain : MonoBehaviour {
	private GameObject newstars;

	void OnTriggerExit (Collider other)
	{

		///if OnTriggerEnter occhio al fatto che si ricrea e ricollide, una soluzione è la funzione invoke
		if (other.tag == "Player" || other.tag == "star" )
		{
			//newstars = gameObject;
			Destroy (gameObject);
			//newstars.transform.localScale = new Vector3 (transform.localScale.x/2, transform.localScale.y/2, transform.localScale.z/2);

			//Instantiate(newstars, transform.position, transform.rotation);//transform.position+ new Vector3(17,0,0)
			//Instantiate(newstars, transform.position, transform.rotation);
			AudioClip bling = Resources.Load("starso") as AudioClip;
			AudioSource.PlayClipAtPoint(bling,new Vector3(0,0,0));

			pointver2.points+=1;
			print (pointver2.points);

		}



	}

}
