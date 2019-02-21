using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ruotatestar2 : MonoBehaviour {
    public float tumble;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, Random.value * tumble,0.0f );
    }
}
