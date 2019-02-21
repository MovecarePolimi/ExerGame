using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlColl2 : MonoBehaviour
{
    Rigidbody rigido;
    public GameObject shoot;
    float angolo;
    Vector3 mia;// = new Vector3(0f,0f,0f);
    static public string text24;
    // Use this for initialization
    void Start()
    {
        rigido =GameObject.Find("Rocket10").gameObject.GetComponent<Rigidbody>();
        GameObject.Find("Textbonus").GetComponent<Text>().enabled = false;
        pointver2.sparoinbonus = false;
        GameObject.Find("Textcolpisci").GetComponent<Text>().text = text24;
    }
bool pippo = true;
    bool pippo2 = true;
    // Update is called once per frame
    void Update()
    {
        pointver2.simulazione();
        if (pointver2.sparoinbonus && pippo == true && pippo2==false)
        {
            angolo = rigido.transform.transform.eulerAngles.z;

            if (angolo < 90)//sono uguali per fortuna ma gli ho lasciati per chiarezza, occhio che gli angoli possono anche diventare negativi ..-90 etc
            {
                mia.x = rigido.transform.position.x -10 * Mathf.Sin(rigido.transform.transform.eulerAngles.z* Mathf.PI/180);
                mia.y = rigido.transform.position.y +10 * Mathf.Cos(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
                
            }
            if (angolo > 90 && angolo < 180)
            {
                mia.x = rigido.transform.position.x - 10 * Mathf.Sin(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
                mia.y = rigido.transform.position.y + 10 * Mathf.Cos(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
               
            }
            if (angolo > 180 && angolo < 270)
            {
                mia.x = rigido.transform.position.x - 10 * Mathf.Sin(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
                mia.y = rigido.transform.position.y + 10 * Mathf.Cos(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
            }
            if (angolo > 270)

            {
                mia.x = rigido.transform.position.x - 10 * Mathf.Sin(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
                mia.y = rigido.transform.position.y + 10 * Mathf.Cos(rigido.transform.transform.eulerAngles.z * Mathf.PI / 180);
            }


            mia.z = rigido.transform.position.z;
            // Vector3 shoottrasf = new Vector3(rigido.transform.position.x, rigido.transform.position.y+10, rigido.transform.position.z);
            Instantiate(shoot, mia, rigido.rotation);
        
            pippo = false; pointver2.sparoinbonus = false;
        }


      if(pippo2==true&& rigido.position.y > 40f) { pippo2 = false; pointver2.sparoinbonus = false; }

        
       
        if (rigido.position.y < (42 )&&pippo2==true)
        {
            if (pippo == true)
            {
                Vector3 vett = new Vector3(0, Mathf.Abs(rigido.position.y - 50), 0);
                rigido.MovePosition(rigido.position + vett * 0.7f  *Time.deltaTime);//* Time.fixedUnscaledDeltaTime);
                rigido.AddTorque(new Vector3(0F, 0F, 0.2F), ForceMode.Acceleration);
            }
            else { }
           
        }



    }
}
