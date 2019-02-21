using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class countdownscript : MonoBehaviour {

    static public string text23; //" secondi al prossimo lancio"
                                 // Use this for initialization
    void Start () {
		StartCoroutine(aspetta15());
	//	ciao= gameObject.GetComponent<Text>();

	}
	
	IEnumerator aspetta15()
	{
		
		for (int h = 16; h > -1;h--) {//16
			gameObject.GetComponent<Text>().text=string.Concat(h.ToString(),text23);
			yield return new WaitForSecondsRealtime (1);
		}
		yield return new WaitForSecondsRealtime (1);
		pointver2.life -= 1;
		if (pointver2.life == 0 ) {
			SceneManager.LoadScene ("intro", LoadSceneMode.Single);
		} else {
			SceneManager.LoadScene ("main", LoadSceneMode.Single);
		}
	}
}
