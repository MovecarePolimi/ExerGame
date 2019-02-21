using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // mostra classe boundaryes in unity 
public class  Boundary
{
	public float xMin, xMax, yMin, yMax;
}

public class playercontrollerendur : MonoBehaviour {  //inheriting from Monobehaviour
	private Rigidbody rb;
	public float speed;
	public Boundary boundary;
	public float tilt;
    private float posizione;

    void main() { rb.isKinematic = true; }


	private void FixedUpdate() //info :  ctrl shift M   ctrl h
	{
        pointver2.simulazione();
//		float moveHorizontal = Input.GetAxis("Horizontal");
//		float moveVertival = Input.GetAxis("Vertical");
//		Vector3 movement = new Vector3(/*moveHorizontal*/0.0f,  moveVertival,0.0f);
		rb = GetComponent<Rigidbody>();  // vecchia versione rigidbody. velocity
                                         //		rb.velocity = movement* speed;
                                         /*
        rb.position = new Vector3(  //assegno il valore del vettore alla trasformazione(posizione)     
			Mathf.Clamp(rb.position.x , boundary.xMin, boundary.xMax),   //all math function
			Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
			0.0f
		);
        */

		rb.rotation = Quaternion.Euler( rb.velocity.y *-tilt,0.0f ,-90f);
        /*
		if (pointver2.press>pointver2.sogliaprecipita) {

			rb.velocity = new Vector3 (0, +6, 0);
		}
		else
			rb.velocity = new Vector3 (0, -4, 0);
	*/

        posizione = 13f + (33f - 13f)*pointver2.feedbackendurance;//31.3

        rb.transform.position = Vector3.Lerp(rb.transform.position, new Vector3(-14,posizione,0.05f), 0.05f);//Time.time / 100f //0.01 poco reattivo
       
    }



}




