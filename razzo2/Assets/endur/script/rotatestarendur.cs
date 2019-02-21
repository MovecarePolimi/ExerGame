using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatestarendur : MonoBehaviour {
	public float tumble;//caduta
	// Use this for initialization
	void Start () {

		GetComponent<Rigidbody> ().angularVelocity = new Vector3(0.0f,0.0f,Random.value * tumble);//Random.insideUnitSphere*tumble;//return a vector3
		//remember to stop drag (resistenza aria) o drang angle
		// GetComponent<Rigidbody>().transform.forward * speed

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
