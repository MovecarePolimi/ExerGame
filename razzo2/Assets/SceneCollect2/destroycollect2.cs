using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class destroycollect2 : MonoBehaviour {


    private GameObject newstars1;
    private GameObject newstars2;
    private GameObject newstars3;
    private GameObject newstars4;
    private GameObject newstars5;

    static public string text18; //"Hai ottenuto 10 punti bonus"
    static public string text19; //"Hai ottenuto 50 punti bonus"
    static public string text20; //"Hai ottenuto 20 punti bonus"
    static public string text21; //"Hai ottenuto 30 punti bonus"
    static public string text22; //"Hai ottenuto 40 punti bonus"

    void OnTriggerEnter(Collider other)
    {


        if (other.tag == "star")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled=false;
            newstars1 = other.gameObject;
            newstars2 = other.gameObject;
            newstars3 = other.gameObject;
            newstars4 = other.gameObject;
            newstars5 = other.gameObject;
            float scale= 0.1f;
            newstars1.transform.localScale = new Vector3(scale, scale, scale);
            newstars2.transform.localScale = new Vector3(scale, scale, scale);
            newstars3.transform.localScale = new Vector3(scale, scale, scale);
            newstars4.transform.localScale = new Vector3(scale, scale, scale);
            newstars5.transform.localScale = new Vector3(scale, scale, scale);



            newstars1 = Instantiate(newstars1, new Vector3(other.transform.position.x+0, other.transform.position.y +2, other.transform.position.z), other.transform.rotation);
            newstars2 = Instantiate(newstars2, new Vector3(other.transform.position.x+3, other.transform.position.y +1, other.transform.position.z), other.transform.rotation);
            newstars3 = Instantiate(newstars3, new Vector3(other.transform.position.x+1, other.transform.position.y -2, other.transform.position.z), other.transform.rotation);
            newstars4 = Instantiate(newstars4, new Vector3(other.transform.position.x-1, other.transform.position.y -2, other.transform.position.z), other.transform.rotation);
            newstars5 = Instantiate(newstars5, new Vector3(other.transform.position.x-3, other.transform.position.y +1, other.transform.position.z), other.transform.rotation);

            

            newstars1.GetComponent<Rigidbody>().AddForce(new Vector3(  0f, 20f, 0f));
            newstars2.GetComponent<Rigidbody>().AddForce(new Vector3(+30f, 10f, 0f));
            newstars3.GetComponent<Rigidbody>().AddForce(new Vector3(+10f,-20f, 0f));
            newstars4.GetComponent<Rigidbody>().AddForce(new Vector3(-10f,-20f, 0f));
            newstars5.GetComponent<Rigidbody>().AddForce(new Vector3(-30f, 10f, 0f));
       


            



            Destroy(other.gameObject);
            Destroy(gameObject);

            AudioClip bling = Resources.Load("starso") as AudioClip;
            AudioSource.PlayClipAtPoint(bling, new Vector3(0, 0, 0));

     GameObject.Find("Textbonus").GetComponent<Text>().enabled = true;
            //codeice per dare punti ad una stella
            
            if (other.name == "starroul10")
            {
                GameObject.Find("Textbonus").GetComponent<Text>().text = text18;
                pointver2.points += 10;
                print(pointver2.points);
            }
            GameObject.Find("Textbonus").GetComponent<Text>().enabled = true;
            if (other.name == "starroul20")
            {
                GameObject.Find("Textbonus").GetComponent<Text>().text = text19;
                pointver2.points += 50;
                print(pointver2.points);
            }
            if (other.name == "starroul30")
            {
                GameObject.Find("Textbonus").GetComponent<Text>().text = text20;
                pointver2.points += 20;
                print(pointver2.points);
            }
            if (other.name == "starroul40")
            {
                GameObject.Find("Textbonus").GetComponent<Text>().text = text21;
                pointver2.points += 30;
                print(pointver2.points);
            }
            if (other.name == "starroul50")
            {
                GameObject.Find("Textbonus").GetComponent<Text>().text = text22;
                pointver2.points += 40;
                print(pointver2.points);
            }
            
            /*
            float random = Random.value;
            int puntibonus=0;
            Debug.Log(random);
            if (random >= 0f && random < 0.2f) puntibonus=10;
             if  (random>=0.2&& random <0.4) puntibonus =20;
             if(random>=0.4 &&random <0.6) puntibonus =30;
            if(random>=0.6 &&random <0.8) puntibonus =40;
            if (random>=0.8 &&random <=1) puntibonus =50;
            GameObject.Find("Textbonus").GetComponent<Text>().text = "Hai ottenuto "+puntibonus.ToString() +" punti bonus";
            pointver2.points += puntibonus;
            print(pointver2.points);
            */
        }



    }
}
