using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
//using NUnit.Framework;
using System.IO;
using Windows.Storage;


public class pointver2 :MonoBehaviour
{


    //static public int points = 0;                   
    //static public int life = 3;  //mettere a 1 se si vuole fare una sola vita, 3 per il gioco completo                       
    //static public bool instrDone = false;             
    //static public float press=75;
    //static public float sogliacalibrazioneavvenuta = 70;
    //static public float sogliaskip = 300; //sogliaskip=5000;                    
    //static public float sogligonfiaggio = 300; //sogligonfiaggio = 5000;             
    //static public float valore_massimo = 3000; //old=30000; valore massimo raggiunto dopo il lancio
    ////static public float valore_minimo=1;
    //static public float feedbackpress; //tra 0 e 1
    //static public float pressmax=0;
    //static private float previoustime=0;
    //static private float previoupress=0;
    //static public bool skip = false;
    //static public bool abilitaskip = true;
    //static public bool abilitadopocalibraz = false;
    //static public bool abilitaprimacalibraz = false;
    //static public bool statogameforce = false;
    //static public bool statogameendur = false;
    //static public bool maxofpercentilecomputed = false;
    //static public bool provenoneseguite = false;
    //static public bool endurancenotdone = false;
    //static public int duratamassimaendurance = 60;
    //static public float sogliarettification = 10000;


    ////ATTENZIONE I PERCENTILI DEVO PER FORZA CALCOLARLI FUORI DA UNITY 
    ////mi fido di più di .net che dell'update di unity
    ////qui sono messi a caso
    //static public float percentile1=0;  //attento percentile prima prova ma la vita è la 3
    //static public float percentile2=0;
    //static public float percentile3 = 0; //23000;

    //static public float maxofpercentile = 0;//23000;
    //static public float maxperendurance = 0;
    //static public float pressaveraged = 0;
    //static public float feedbackendurance; //tra 0 e 1

    //static public bool registrazioneok= false;
    //static public int puntiprecedenti=0;
    //static public bool statodurantecalibrazione = true;
    //static public bool devicenotfound = false;
    //static public int secondidiricerca = 300;
    //static public bool sparoinbonus = false;

    //impossible corutine in static 

    static public int points;
    static public int life;  //mettere a 1 se si vuole fare una sola vita, 3 per il gioco completo                       
    static public bool instrDone;
    static public float press;
    static public float sogliacalibrazioneavvenuta;
    static public float sogliaskip; //sogliaskip=5000;                    
    static public float sogligonfiaggio; //sogligonfiaggio = 5000;             
    static public float valore_massimo; //old=30000; valore massimo raggiunto dopo il lancio
    //static public float valore_minimo=1;

    static public float pressmax;
    static private float previoustime = 0;
    static private float previoupress = 0;
    static public bool skip;
    static public bool abilitaskip;
    static public bool abilitadopocalibraz;
    static public bool abilitaprimacalibraz;
    static public bool statogameforce;
    static public bool statogameendur;
    static public bool maxofpercentilecomputed;
    static public bool provenoneseguite;
    static public bool endurancenotdone;
    static public int duratamassimaendurance;
    static public float sogliarettification;

    static public bool valore_massimo_flag = false;

    //ATTENZIONE I PERCENTILI DEVO PER FORZA CALCOLARLI FUORI DA UNITY 
    //mi fido di più di .net che dell'update di unity
    //qui sono messi a caso
    static public float percentile1;  //attento percentile prima prova ma la vita è la 3
    static public float percentile2;
    static public float percentile3; //23000;

    static public float maxofpercentile;//23000;
    static public float maxperendurance;
    static public float pressaveraged;

    static public bool registrazioneok;
    static public int puntiprecedenti;
    static public bool statodurantecalibrazione;
    static public bool devicenotfound;
    static public int secondidiricerca;
    static public bool sparoinbonus;

    static public float feedbackpress; //tra 0 e 1
    static public float feedbackendurance; //tra 0 e 1

    static public bool batFlag = true;  // true = charged; false = discharged

    static public void simulazione ()
    {

        
        //simulazione
        if (Input.GetKeyDown("1")) { press = 15; }
        if (Input.GetKeyDown("2")) { press = 7000; }
        if (Input.GetKeyDown("3")) { press = 15000; }
        if (Input.GetKeyDown("4")) { press = 22000; }
        if (Input.GetKeyDown("5")) { press = 25000; }
        if (press > 0)
        {
            pressaveraged = press;
        }
        else { pressaveraged = 0; }


        //Si salva il valore massimo la prima volta che si gioca!
        if (press >= pressmax && statogameforce && valore_massimo_flag== false) { pressmax = press; }


        if (abilitadopocalibraz == false && press< sogliacalibrazioneavvenuta) { abilitadopocalibraz = true; }
      
        if(abilitadopocalibraz == true) { 
        if (press < sogliaskip) abilitaskip = true;

        if ((Time.time - previoustime) > 0.5 )
        {
           // Debug.Log(Time.time);
            previoustime = Time.time;
            if (press >= previoupress && press > sogliaskip )
            {
                    // Debug.Break();
                    
                if (abilitaskip == true)
                {
                    skip = true; previoupress = press;
                    abilitaskip = false;
                }
            }
            else
            {
                previoupress = press;
            }
          
            
        }
        else { skip = false; }
        //Debug.Log(" prev   "+previoupress);
        // Debug.Log(" attual "+press);
        //Debug.Log(" valore " + skip);

        if (life == 3)
        {
            feedbackpress = 1 / valore_massimo * percentile1;

         }
        if (life == 2)
        {
            feedbackpress = 1 / valore_massimo * percentile2;

        }
        if (life == 1)
        {
            feedbackpress = 1 / valore_massimo * percentile3;

        }


    
            
            if (life == 0 && maxofpercentilecomputed == false)
            {

                //ora calcolo sogliaprecipita
                if (percentile1 > sogliaskip || percentile2 > sogliaskip || percentile3 > sogliaskip)
                {
                    maxofpercentile = Mathf.Max(percentile1, percentile2, percentile3);//da cambiare num
                    //maxperendurance = maxofpercentile * 0.75f;
                }
                else
                {
                    life = -1;
                    SceneManager.LoadScene("intro", LoadSceneMode.Single);
                    provenoneseguite=true;
                }

                maxofpercentilecomputed = true;
            }

            double thresh = 0.79; //0.79 è il livello a centro stella, che mi serve per avere 0.75 a fine stella
            if (life == 0) {

                /*if (pressaveraged > maxperendurance)//percentuale in cui rimane fisso
                { feedbackendurance = 1; }
                else
                {
                    feedbackendurance = 1 / maxperendurance * pressaveraged;

                }*/

                // inizio nuova versione
                if (pressaveraged >= maxofpercentile * thresh)//percentuale in cui rimane fisso (fine stella è a livello 0.86)
                { feedbackendurance = 1; }
                else
                {
                    if (pressaveraged <= maxofpercentile * 0.5)
                    {
                        feedbackendurance = 0;
                    }

                    else
                    {


                        feedbackendurance = (pressaveraged - (maxofpercentile * 0.5f)) / ((maxofpercentile * (float)thresh) - (maxofpercentile * 0.5f));

                    }


                } //fine nuova versione

            }




        }
       
    }
   

}