using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spostamenti : MonoBehaviour {

	public float speeds;
	public float speedr;
	public Vector3 newPos;

	private Rigidbody ciao;
	void Start(){

		ciao = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {

		if (ciao.position.x < newPos.x && ciao.position.y < newPos.y ) {
			Vector3 direction = (newPos - transform.position).normalized;
			ciao.MovePosition(transform.position + direction * speeds * Time.fixedDeltaTime);
		}

		if (ciao.transform.rotation.eulerAngles.x > 340 || ciao.transform.rotation.eulerAngles.x <= 0 )
			transform.Rotate (Vector3.right*Time.deltaTime*-10, Space.Self);
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }
}
