using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class decollo4 : MonoBehaviour {
	private Rigidbody razzo;
	
	public float loadspeed;
	private float newPos;
    
	private MeshRenderer backgr2; 
	private MeshRenderer backgr1; 
	//Camera cam1;
	Camera cam2;
	private GameObject objfuelslid;
	private GameObject objrideslid;
	private Slider fuelslid;
	private Slider rideslid;
	private GameObject LifeImageuno;
	private GameObject LifeImagedue;
	private Image [] sliderimages ;
	Animator anim;  
	RectTransform CountGo;
	private RectTransform changecanvas;
	private bool chiamauna=false;
	private bool pippo = true;
	private bool pippo2 = true;
    private bool pippo3 = true;
    private Collider razzocollider;
	private AudioSource motorsound;
    private Text undercountd;
    static public string text26;
    static public string text27;
    static public string text28;
    static public string text29;
    static public string text30;
    static public string text31;

    void Start () {

        GameObject.Find("Countdown1").gameObject.GetComponent<Text>().text = text26;
        GameObject.Find("Countdown2").gameObject.GetComponent<Text>().text = text27;
        GameObject.Find("Countdown3").gameObject.GetComponent<Text>().text = text28;
        GameObject.Find("CountdownGo").gameObject.GetComponent<Text>().text = text29;
        GameObject.Find("CountdownStop").gameObject.GetComponent<Text>().text = text30;
        GameObject.Find("WellDone").gameObject.GetComponent<Text>().text = text31;

        razzo = GameObject.Find ("Rocket").gameObject.GetComponent<Rigidbody> ();
		razzocollider=GameObject.Find ("Rocket").gameObject.GetComponent<SphereCollider> ();
		backgr2 = GameObject.Find ("backg2").gameObject.GetComponent<MeshRenderer> ();
		backgr1 = GameObject.Find ("backg1").gameObject.GetComponent<MeshRenderer> ();
		backgr1.enabled = false;
		//cam1=GameObject.Find ("Camera").gameObject.GetComponent<Camera> ();
		cam2=GameObject.Find ("Camera2").gameObject.GetComponent<Camera> ();
		changecanvas= GameObject.Find ("Canvas").gameObject.GetComponent<RectTransform> ();
		objfuelslid = GameObject.Find ("Slider1").gameObject;
		objrideslid = GameObject.Find ("Slider2").gameObject;
		fuelslid = GameObject.Find ("Slider1").gameObject.GetComponent<Slider>();
		rideslid = GameObject.Find ("Slider2").gameObject.GetComponent<Slider>();
		//fuelslid.interactable = false;
		LifeImageuno=GameObject.Find ("LifeImage1").gameObject;
		LifeImagedue=GameObject.Find ("LifeImage2").gameObject;
        undercountd = GameObject.Find("undercount").GetComponent<Text>();
		fuelslid.value=0F;
		rideslid.maxValue = 100F;
		rideslid.minValue = -37F;
		objrideslid.SetActive(false);
		anim =GameObject.Find ("Canvas").gameObject.GetComponent <Animator> ();
		CountGo = GameObject.Find ("CountdownGo").gameObject.GetComponent<RectTransform>();
		anim.SetTrigger ("CountdownTrigger");
		sliderimages = GameObject.Find ("Slider1").gameObject.GetComponentsInChildren<Image>();
		if (pointver2.life == 3) {
			GameObject.Find ("LifeImage1").gameObject.GetComponent<RawImage>().enabled=true;
			GameObject.Find ("LifeImage2").gameObject.GetComponent<RawImage>().enabled=true;
		}
				if (pointver2.life == 2) {
			GameObject.Find ("LifeImage1").gameObject.GetComponent<RawImage>().enabled=false;
			GameObject.Find ("LifeImage2").gameObject.GetComponent<RawImage>().enabled=true;
		}
				if (pointver2.life == 1) {
			GameObject.Find ("LifeImage1").gameObject.GetComponent<RawImage>().enabled=false;
			GameObject.Find ("LifeImage2").gameObject.GetComponent<RawImage>().enabled=false;
		}
		motorsound=gameObject.GetComponent<AudioSource> ();
		motorsound.Play ();
        undercountd.enabled = false;
        undercountd.text="1";

    }

    
		void FixedUpdate () {
        pointver2.simulazione();
        if (CountGo.GetComponent<Text>().isActiveAndEnabled)
        {
            
            pointver2.statogameforce = true;//false da visual   altrim ritardo
            
        }
        else
        { pointver2.statogameforce = false; }
        if (  CountGo.localScale.x>0.8F && CountGo.localScale.x<0.96F) {
			//print ("Load");
			motorsound.volume=motorsound.volume+0.01f;

           
            if (pointver2.press> pointver2.sogligonfiaggio) {

			razzo.transform.position -= new Vector3(0,0.0004F,0);
					
	
			pointver2.points+=1;
//			print (pointver2.points);
			razzo.transform.localScale += new Vector3 (+0.0002F, 0, 0);
					razzo.transform.localScale += new Vector3 (0, 0, +0.0002F);
			        //razzo.transform.localScale += new Vector3 (0,-0.0002F ,0 );
			}//if get

			sliderimages [1].CrossFadeColor(Color.green, 3f, true, true);
            if (fuelslid.value > 0.33 && fuelslid.value < 0.66) { undercountd.text = "2"; }
            if (fuelslid.value > 0.66) { undercountd.text = "3"; }
        } 

			if (  CountGo.localScale.x>0.96F){
				if (!chiamauna){
					//print (newPos);
				objfuelslid.SetActive(false);
					cam2.enabled = true;
				objrideslid.SetActive(true);
					chiamauna = true;
				backgr1.enabled = true;
					backgr2.enabled = false;
				changecanvas.localScale = new Vector3 (0.2F, 0.2F, 1F);
				changecanvas.localPosition = new Vector3 (changecanvas.localPosition.x, 35, changecanvas.localPosition.y);
				LifeImageuno.SetActive (false);
				LifeImagedue.SetActive (false);
				motorsound.volume = 0.1f;

            
            }




            newPos = -35 + (1 * 125) * pointver2.feedbackpress; //-35 dove parte e 125 abs del percorso
			if (pippo) {
				if (razzo.position.y < (newPos-10)) {
					Vector3 vett = new Vector3 (0, Mathf.Abs (razzo.position.y - newPos), 0);
					razzo.MovePosition (razzo.position + vett * loadspeed * Time.fixedUnscaledDeltaTime);
					pippo = !pippo;
				}
				//prosegue corsa
				else{

					if (pippo2 == true) {
						razzocollider.enabled = false;
						pippo2 = !pippo2;
					}

							razzo.AddForce (new Vector3 (0, 5, 0), ForceMode.VelocityChange);

				}
			
			
			
			}
			else{
					if(razzo.transform.localScale.x>0.05F ){		
					razzo.transform.localScale += new Vector3 (-0.0002F, 0, 0);
						razzo.transform.localScale += new Vector3 (0, 0, -0.0002F);


				}
				pippo = !pippo;
			}

            if (razzo.transform.position.y > 100)
            {
                if (pointver2.life == 3 ) { SceneManager.LoadScene("intro", LoadSceneMode.Single); }
                else
                SceneManager.LoadScene("collect2", LoadSceneMode.Single);

            }
        }
		



	}

void LateUpdate(){//fuelslid.value = newPos / 100;
         undercountd.rectTransform.position = new Vector3 (fuelslid.handleRect.position.x+1, fuelslid.handleRect.position.y+2, fuelslid.handleRect.position.z);

		rideslid.value = razzo.transform.position.y;}   









}
