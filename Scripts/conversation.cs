using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class conversation : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI label;
    public GameObject panel;
    public Canvas canves;
    public int index;
    public bool endline;
    List<string> textList = new List<string>();
    public float talkSpeed;
    private void Awake() {
        index=0;
        text.text="";
        label.text="";
    }
    public void readText(TextAsset txt){
        textList.Clear();
        index=0;
        textList = new List<string>(txt.text.Split('\n'));
    }

    IEnumerator showText(){
        endline=false;
        talkSpeed=0.05f;
        text.text="";
        switch(textList[index].Trim()){
            case("B:"):label.text="BOSS:\n";index++; break;
            case("A:"):label.text="AI:\n";index++;break;
            case("P:"):label.text="You:\n";index++;break;
        }
        foreach(char c in textList[index]){
            text.text+=c;
            yield return new WaitForSeconds(talkSpeed);
        }
        endline = true;
        index++;
    }

    void printText(){
        if(Input.GetKeyDown("space")&&index==textList.Count){
            panel.SetActive(false);
            index=0;
            canves.SendMessage("conversationEnd");
            return;
        }
        if(Input.GetKeyDown("space")&&endline){
            StartCoroutine(showText());
        }
        else if(Input.GetKeyDown("space")){
            talkSpeed=0;
        }
    }
    // Start is called before the first frame update
    private void OnEnable() {
        panel.SetActive(true);
        endline = true;
        StartCoroutine(showText());
    }

    private void OnDisable() {
        panel.SetActive(false);
        endline = true;
        index=0;
        text.text="";
        label.text="";
        textList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        printText();
    }
}
