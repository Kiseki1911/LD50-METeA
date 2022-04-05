using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Staging : MonoBehaviour
{
    public GameObject shop;
    public GameObject next;
    public TextAsset conversationList;
    private SortedList<int,string> upgradeconverList = new SortedList<int,string>();
    private SortedList<int,string> dayconverList = new SortedList<int,string>();
    private SortedList<int,string> shopLit = new SortedList<int,string>();
    public GameObject panel,logicUnit,wall,shadow,logo,collision,table,player,panel2,panel3;
    private bool inShop=false;
    private bool started=false;
    public int dayIndex,upgradeIndex;
    // Start is called before the first frame update
    void Awake()
    {
        logicUnit.SetActive(false);
        panel2.SetActive(true);
        shop.SetActive(false);
        next.SetActive(false);
        dayIndex=0;
        upgradeIndex=0;
        var textList = new List<string>(conversationList.text.Split('\n'));
        foreach(var json in conversationList.text.Split('\n')){
            var res=JsonUtility.FromJson<Content>(json);
            if(res.isMain==0)
                dayconverList.Add(res.index,res.text);
            else if(res.isMain==1)
                upgradeconverList.Add(res.index,res.text);
            else
                shopLit.Add(res.index,res.text);
        }
        shadow.SetActive(true);
        wall.SetActive(true);
        collision.SetActive(true);
        logo.SetActive(false);
        table.SetActive(false);
        shop.SetActive(false);
        next.SetActive(false);
        panel.GetComponent<conversation>().readText(Resources.Load<TextAsset>("firstTalk"));
        panel.SetActive(true);
    }


    public void conversationStart(dialog d){
        panel2.SetActive(true);
        string textAsset=d.ToString();
        string path;
        if((d.dialogType&1)==1){
            path=upgradeconverList[player.GetComponent<Player>().prosthesis];
            textAsset += "\n"+Resources.Load<TextAsset>(path).text;
            upgradeIndex++;
        }
        if((d.dialogType&2)==2&&dayIndex<3)
        {
            path=dayconverList[dayIndex];
            textAsset += "\n"+Resources.Load<TextAsset>(path).text;
            upgradeIndex++;
        }
        panel.GetComponent<conversation>().readText(new TextAsset(textAsset));
        panel.SetActive(true);
    }
    public void conversationEnd(){
        panel.SetActive(false);
        if(!started){
            logicUnit.SetActive(true);
            panel2.SetActive(false);
            started=true;
            return;
        }
        shop.SetActive(true);
        next.SetActive(true);
    }
    // Update is called once per frame
    
    public void onNextClicked(){
        if(!inShop){
            panel.SetActive(false);
            shop.SetActive(false);
            next.SetActive(false);
            panel2.SetActive(false);
            panel3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="";
            panel3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text="";
            panel3.SetActive(false);
            if(player.GetComponent<Player>().prosthesis<4){
                logicUnit.SendMessage("next");
                }
            else{
            
            GameObject.Find("Main Camera").GetComponent<CameraTask>().case1=true;
            GameObject.Find("Main Camera").GetComponent<CameraTask>().case2=true;
            player.SetActive(true);
        }
        }
        else {   
            TextAsset textAsset;
            if(!player.GetComponent<Player>().addProsthesis()){
                textAsset = new TextAsset("You need more money!\n");
            }
            else{
                textAsset = new TextAsset("Thank you for shopping\n");
            }
            panel3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text=textAsset.text;
            shop.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="Shop";
            next.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="Next";
            inShop=false;
        }
    }

    public void onShopClicked(){
        if(!inShop){
            inShop=true;
            shop.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="Back";
            next.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="Buy";
            panel3.SetActive(true);
            panel3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="AI:";
            panel3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text=Resources.Load<TextAsset>(shopLit[player.GetComponent<Player>().prosthesis]).text;

        }
        else{
            panel3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="";
            panel3.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text="";
            panel3.SetActive(false);
            shop.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="Shop";
            next.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text="Next";
            inShop=false;
        }


    }

    public void expand(){
        shadow.SetActive(false);
        wall.SetActive(false);
        collision.SetActive(false);
        logo.SetActive(true);
        table.SetActive(true);
    }

}

[System.Serializable]
public class Content
{
    public int isMain;
    public int index;
    public string text;
    public Content(int isMain,int index,string text){
        this.isMain=isMain;
        this.index=index;
        this.text=text;
    }
}