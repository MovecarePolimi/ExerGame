using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveshoot2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<Rigidbody>().velocity = transform.up*+8f;
        
    }
}
