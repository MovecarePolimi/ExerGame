using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gamecontrollerendur : MonoBehaviour {
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
    static public string text25;




	void Start()
	{
		StartCoroutine(SpawnWaves());
        GameObject.Find("Textcolpisci").GetComponent<Text>().text = text25;

    }
    bool pippo = false;
	void Update()
	{
        if (pointver2.statogameendur == true) { pippo = true; }
        if (pointver2.statogameendur==false && pippo==true) {
            pointver2.life = -1;
            SceneManager.LoadScene("intro", LoadSceneMode.Single);
        }
       if( pointver2.endurancenotdone == true) {
            pointver2.life = -1;
            SceneManager.LoadScene("intro", LoadSceneMode.Single);
        }

    }

	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds(startWait);
		while (true)
		{
			for (int i = 0; i < hazardCount; i++)
			{
				GameObject hazard = hazards[Random.Range(0, hazards.Length)];
				Vector3 spawnPosition = new Vector3(spawnValues.x, spawnValues.y, spawnValues.z);
				//Quaternion spawnRotation = new Quaternion (0f,0f,0f,0f);
				Instantiate(hazard, spawnPosition,Quaternion.Euler(90F,0F,0F)); //spawnRotation);
				yield return new WaitForSeconds(spawnWait);
			}
			yield return new WaitForSeconds(waveWait);


		}
	}




    /*
	void OnTriggerEnter (Collider other)
	{


		if ( other.tag=="Finish")
			{
	     pointver2.life = -1;
			SceneManager.LoadScene ("intro", LoadSceneMode.Single);


		}
       


	}	
 */
}
