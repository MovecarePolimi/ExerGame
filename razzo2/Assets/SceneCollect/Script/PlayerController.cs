using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // mostra classe boundaryes in unity 
public class  Boundaryend
{
  public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour {  //inheriting from Monobehaviour
    private Rigidbody rb;
    public float speed;
    public Boundary boundary;
    public float tilt;

   


    private void FixedUpdate() //info :  ctrl shift M   ctrl h
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertival = Input.GetAxis("Vertical");
		Vector3 movement = new Vector3(moveHorizontal,  moveVertival,0.0f);
        rb = GetComponent<Rigidbody>();  // vecchia versione rigidbody. velocity
        rb.velocity = movement* speed;
        rb.position = new Vector3(  //assegno il valore del vettore alla trasformazione(posizione)     
            Mathf.Clamp(rb.position.x , boundary.xMin, boundary.xMax),   //all math function
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
			0.0f
            );

		rb.rotation = Quaternion.Euler(0.0f, rb.velocity.x *-tilt,0.0f);
    }



    //to copy 
     public GameObject shot;


    //fire 
    public float fireRate;
    private float nextFire;
    private void Update()
    {
        // Instantiate: clone object
        //Shot spawn as object child player, its coordinates are referred to player
        

	
		if (Time.time > nextFire)//(Input.GetButton("Fire1") && Time.time > nextFire)
        {
			Vector3 cambiovariab2 = new Vector3 (rb.transform.position.x, rb.transform.position.y+10f, rb.transform.position.z);
            nextFire = Time.time + fireRate;
			Instantiate(shot,cambiovariab2, rb.rotation);// as GameObject;  don't require control of it;          
            
        }
   
    } 
    }




