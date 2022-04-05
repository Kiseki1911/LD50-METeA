using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EnterGame : MonoBehaviour
{  
    public GameObject input;
    public GameObject notice;
    public void StartTheGame(){
        //Debug.Log("Go to Game Scene");
        if(input.GetComponent<TextMeshProUGUI>().text!="​"){
            //Debug.Log(input.GetComponent<TextMeshProUGUI>().text);
            SceneManager.LoadScene(1);
        }
        else{
            notice.GetComponent<TextMeshProUGUI>().enabled=true;
            Debug.Log("no name");
        }
    }
}
