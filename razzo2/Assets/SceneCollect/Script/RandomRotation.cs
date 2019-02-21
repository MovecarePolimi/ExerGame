using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour {

    public float tumble;//caduta
    private void Start()
    {


        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere*tumble;//return a vector3
        //remember to stop drag (resistenza aria) o drang angle
        // GetComponent<Rigidbody>().transform.forward * speed

    }

}
