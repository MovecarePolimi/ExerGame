using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class streamingdatascript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject.Find("Textpressrealtime").GetComponent<Text>().text = pointver2.press.ToString();
        // GameObject.Find("Textpressrealtime").GetComponent<Text>().text = "●";

    }
}
