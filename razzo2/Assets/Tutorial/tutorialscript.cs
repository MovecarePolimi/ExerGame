using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
//using Windows.Storage;

public class tutorialscript : MonoBehaviour
{
    private double controllotesto;
    private Quaternion rotation = Quaternion.identity;
    // Use this for initialization
    //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
    static public string findBall_c;//"Sto cercando la pallina...";
    


    void Start()
    {
        GameObject.Find("saltaintro").SetActive(false);

    }
    private bool pippo = true;
    // Update is called once per frame
    void Update()
    {

        pointver2.simulazione();

        if (pointver2.abilitadopocalibraz == true)
        { SceneManager.LoadScene("intro", LoadSceneMode.Single); }

        if (pointver2.devicenotfound && pointver2.batFlag)
        {
            pointver2.life = -1;
            SceneManager.LoadScene("intro", LoadSceneMode.Single);
        }


        if (pippo == true)
        {/*
            GameObject.Find("Video Player").GetComponent<VideoPlayer>().url = storageFolder.Path + "/lavoltabuona2.mp4";
            rotation.eulerAngles = new Vector3(90, 180, 0);

            GameObject.Find("Planevideo1").transform.rotation = rotation;

            GameObject.Find("Video Player").GetComponent<VideoPlayer>().Play();
            
            */

                GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = tutorialscript.findBall_c;
                pippo = false;
            
        }

        /*controllotesto = GameObject.Find("Video Player").GetComponent<VideoPlayer>().time;
         if (controllotesto > 0 && controllotesto <= 4)
         {
             GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = "prendi la pallina";

             if (pointver2.abilitaprimacalibraz == false) // uncomment for fast calibartion
             {
                 pointver2.abilitaprimacalibraz = true;

             }
         }
         if (controllotesto > 4 && controllotesto <= 18)
         {
             GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = "afferrala con la mano dominante";

         }

         if (controllotesto > 18 && controllotesto <= 32)
         {
             GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = "Siediti comodo, appoggia l’avambraccio";
         }
         if (controllotesto > 33 && controllotesto < 36)
         {
             GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = "ora non stringere la pallina \ncalibrazione in corso... ";
         }
         if (controllotesto > 36)
         {
             GameObject.Find("Video Player").GetComponent<VideoPlayer>().Stop();
             GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = "ora non stringere la pallina \ncalibrazione in corso... ";
             if (pointver2.abilitaprimacalibraz == false)
             {
                 pointver2.abilitaprimacalibraz = true;

             }

         }*/
        if (pointver2.abilitaprimacalibraz == false)
        {
            pointver2.abilitaprimacalibraz = true;

        }

    }

}
