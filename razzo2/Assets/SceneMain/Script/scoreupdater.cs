using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreupdater : MonoBehaviour {

    static public string text17;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<Text> ().text = string.Concat(text17, pointver2.points.ToString());
	}
}
