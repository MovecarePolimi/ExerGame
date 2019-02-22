using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using System.Diagnostics;
using System.IO;



public class changInstr : MonoBehaviour {
    private Slider sli;
    private Text under;
    private Image[] sliderimagesIntro;
  //  private Rigidbody mano;
    public bool setT = false;
    private bool pippo = false;
    
    private Animator animatorI;
    private Animator animatorS;

    static public string text1; //= "Ruota la pallina per sparare ad una stella";
    static public string text2; //= "Quando sei pronto premi leggeremente\nla pallina";
    static public string text3; //= "Ora premi più forte che puoi fino a fine gioco";
    static public string text4; //= "Quando ti senti pronto \n inizia a mantenere la presa";
    static public string text5; //= "Al VIA premi più forte che puoi per tre secondi ";
    static public string text6; //= "Quando sei pronto premi leggeremente\nla pallina";
    static public string text7; //Fine! hai totalizzato ", pointver2.points.ToString(), "punti,  grazie per aver giocato! 
    static public string text8; //" Fine gioco,  non hai eseguito le istruzioni correttamente "
    static public string text9; //"No"
    static public string text10; //"punteggio precedente: " + pointver2.puntiprecedenti + " Bravo!";
    static public string text11; //"punteggio precedente: " + pointver2.puntiprecedenti + "puoi fare di meglio!";
    static public string text12;
    static public string text13; 
    static public string text14;
    static public string text15;
    static public string text16;
    static public string text32;
    static public string text33;

    static public bool timeoutflag = false;
    static public bool nuevoFlag = false;



    void Start () {
        animatorI = GameObject.Find("Rigged Hand6").gameObject.GetComponent<Animator>();
        animatorS = GameObject.Find("shootintro").gameObject.GetComponent<Animator>();
        // mano = GameObject.Find("Rigged Hand6").gameObject.GetComponent<Rigidbody>();
        if (pointver2.life >0 && pointver2.instrDone == true )
        {
            GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = text1;
            GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = text2;
            GameObject.Find("undercountintro").gameObject.SetActive(false);
            GameObject.Find("SlideIntro").gameObject.SetActive(false);
            // Destroy(GameObject.Find("Canvas2").gameObject);


            //animatorI.enabled = false;
            animatorI.SetBool("strettabool", false);
            animatorI.SetBool("repeatbool", false);
            animatorI.SetBool("ciaobool", true);
            animatorI.SetBool("ciaoboolr", true);
            animatorS.SetBool("triggershoot", true);

            //mano.position=new Vector3(-38f,70f,-52f);
            // mano.rotation = Quaternion.Euler(new Vector3(166.7f ,2.4f , 0)); 


            pippo = false;
        }
         
        if (pointver2.life == 0) {
			GameObject.Find ("TextInstr1").gameObject.GetComponent<Text> ().text = text3;
			GameObject.Find ("TextContinue1").gameObject.GetComponent<Text> ().text = text4;
            
            GameObject.Find("undercountintro").gameObject.SetActive(false);
            GameObject.Find("SlideIntro").gameObject.SetActive(false);
            GameObject.Find("shootintro").gameObject.SetActive(false);
            GameObject.Find("Rocketintro").gameObject.SetActive(false);
            //GameObject.Find("Rigged Hand6").gameObject.SetActive(false);
        }

		if (pointver2.life>0 && pointver2.instrDone == false )
        {

            GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = text5;
			GameObject.Find ("TextContinue1").gameObject.GetComponent<Text> ().text = text6;

            under = GameObject.Find("undercountintro").GetComponent<Text>();
            sli = GameObject.Find("SlideIntro").gameObject.GetComponent<Slider>();
            sliderimagesIntro = GameObject.Find("SlideIntro").gameObject.GetComponentsInChildren<Image>();
            GameObject.Find("shootintro").gameObject.SetActive(false);
            GameObject.Find("Rocketintro").gameObject.SetActive(false);
            animatorI.SetBool("strettabool", true);
            animatorI.SetBool("repeatbool", true);
            animatorI.SetBool("ciaobool", false);
            animatorI.SetBool("ciaoboolr", false);


            pippo = true;

        }
		if (pointver2.life == -1) {
            if (pointver2.provenoneseguite == false)
            {
                GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = string.Concat(text7,pointver2.points.ToString());
                if (pointver2.puntiprecedenti > 0)
                {
                    if (pointver2.puntiprecedenti < pointver2.points)
                    {
                        GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = string.Concat(text10, pointver2.puntiprecedenti);
                    }
                    else
                        GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = string.Concat(text11, pointver2.puntiprecedenti);
                }
                else {
                    GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = text9;

                }

            }
            else {
                GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = text8;
            }
           //premi R per ricominciare
            GameObject.Find("undercountintro").gameObject.SetActive(false);
            GameObject.Find("SlideIntro").gameObject.SetActive(false);
            GameObject.Find("Rigged Hand6").gameObject.SetActive(false);
            GameObject.Find("shootintro").gameObject.SetActive(false);
            GameObject.Find("Rocketintro").gameObject.SetActive(false);
        }


    }

		void Update () {
            pointver2.simulazione();

        if (pointver2.life > 0) {

            if (pippo == true)
                {
                    sliderimagesIntro[1].CrossFadeColor(Color.green, 3f, true, true);
                    if (sli.value < 0.33) { under.text = "1"; }
                    if (sli.value > 0.33 && sli.value < 0.66) { under.text = "2"; }
                    if (sli.value > 0.66) { under.text = "3"; }
                    under.rectTransform.position = new Vector3(sli.handleRect.position.x + 1, sli.handleRect.position.y + 2, sli.handleRect.position.z);
                }
            if (pointver2.skip) {
             
               if (pointver2.instrDone == false)
               {
                        SceneManager.LoadScene("main", LoadSceneMode.Single);
                        pointver2.instrDone = !pointver2.instrDone;
               }
               else
               {
                   SceneManager.LoadScene("collect2", LoadSceneMode.Single);
               }
            }
        }
		if (pointver2.life == 0) {
           
            if (pointver2.skip) {
			    SceneManager.LoadScene ("endur", LoadSceneMode.Single);	
			}
		}
        if (pointver2.devicenotfound == false)
        { 
		    if (pointver2.life == -1)
            {
                if (pointver2.provenoneseguite == false)
                {
                    GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = string.Concat(text7, pointver2.points.ToString());
                    if (pointver2.puntiprecedenti > 0)
                    {
                        if (pointver2.puntiprecedenti < pointver2.points)
                            {
                                GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = string.Concat(text13, pointver2.points.ToString(), text14,pointver2.puntiprecedenti, text15);
                                GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = "";
                            }
                        else
                            {
                                GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = string.Concat(text13, pointver2.points.ToString(), text14, pointver2.puntiprecedenti, text16);
                                GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = "";
                            }
                    }
                    else
                    {
                        GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = "";
                    }
                        
                }
                else
                {
                    GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = text8;
                }
                //Cristian
                Invoke("waitime", 4);

            }
            if (nuevoFlag)
            {
                Invoke("waitime", 1);
            }
        }
        else
        {
            if (!pointver2.batFlag)
            {
                SceneManager.LoadScene("tutorial", LoadSceneMode.Single);
                GameObject.Find("Texttutorial").gameObject.GetComponent<Text>().text = text33;
                GameObject.Find("saltaintro").gameObject.GetComponent<Text>().text = text32;
            }
            else
            {
                GameObject.Find("TextInstr1").gameObject.GetComponent<Text>().text = text12;
                GameObject.Find("TextContinue1").gameObject.GetComponent<Text>().text = text32;
            }

            //if (changInstr.timeoutflag)
            //{
                Invoke("waitime", 15);
           // }
        }
    }
   
    void waitime()
    {
        Application.Quit();
    }
}
