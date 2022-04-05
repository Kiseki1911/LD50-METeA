using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class LogicUnit : MonoBehaviour
{
    public teacup tea;
    public dialog d;
    public int gameStage;
    public int kpi;
    public int kpiTarget;
    public Difficualty difficualty; 
    
    public TextMeshProUGUI textM;
    private int miss;
    public float time,curtime;

    public GameObject set0,set1,player;
    public Canvas canvas;

    public bool expand;
    private bool first;
    void recieved(int res){
        if(res>0){
            switch(tea.tea){
                case (TeaType.GREEN):
                case (TeaType.PINK): res*=2;break;
                case (_):break;
            }
            if(tea.desk==0)
                set0.BroadcastMessage("nextState");
            else
                set1.BroadcastMessage("nextState");
            GameObject.Find("Player").SendMessage("recieved");
            d.diliverd++;
            kpi+=res;
        }
        Debug.Log(res);
    }

    void timeout(bool desk){
        if(desk)
            set1.BroadcastMessage("nextState");
        else
            set0.BroadcastMessage("nextState");

        kpi-=++miss;
    }

    void next(){
        curtime=time;
        difficualty.gameStage++;
        d = new dialog();
        GameObject.Find("NPC0").SendMessage("restart",1);
        GameObject.Find("NPC1").SendMessage("restart",2);
        GameObject.Find("NPC2").SendMessage("restart",3);
        set0.BroadcastMessage("start",difficualty);
        if(expand){
            GameObject.Find("NPC3").SendMessage("restart",1);
            GameObject.Find("NPC4").SendMessage("restart",2);
            GameObject.Find("NPC5").SendMessage("restart",3);
            set1.BroadcastMessage("start",difficualty);
        }
        player.SetActive(true);
    }

    void sendTea(teacup tea){
        this.tea=tea;
        set0.BroadcastMessage("reciveTea",tea);
        if(expand)
        set1.BroadcastMessage("reciveTea",tea);
    }
    // Start is called before the first frame update
    void Start()
    {
        difficualty = new Difficualty();
        d = new dialog();
        difficualty.gameStage=gameStage;
        kpi=0;
        //kpiTarget=10;
        miss=0;
        curtime=time;
        tea=new teacup();
        GameObject.Find("NPC0").SendMessage("restart",1);
        GameObject.Find("NPC1").SendMessage("restart",2);
        GameObject.Find("NPC2").SendMessage("restart",3);
        set0.BroadcastMessage("start",difficualty);
        d.bounes=0;
        d.dialogType=0;
        d.diliverd=0;
        d.kpi=0;
        d.target=0;
        first=true;
    }

    // Update is called once per frame
    private void FixedUpdate(){
        d.kpi=0;
        d.target=0;
        d.miss=miss;
        if(curtime>0){
            curtime-=Time.deltaTime;
            if(curtime<=0){
                set0.BroadcastMessage("finished");
                if(expand)
                    set1.BroadcastMessage("finished");
                if(kpi>kpiTarget){
                    d.bounes=kpi-kpiTarget;
                    d.kpi=kpi;
                    d.target=kpiTarget;
                    player.SendMessage("pay",d.bounes);
                    kpiTarget+=(d.bounes+1)/2;
                    kpi=0;
                    miss=0;
                }
                else{
                    d.bounes=0;
                    d.kpi=kpi;
                    d.target=kpiTarget;
                    if(!first)
                        player.BroadcastMessage("addForceProsthesis",options:SendMessageOptions.DontRequireReceiver);
                    first=true;
                    d.dialogType|=1;
                }
                if((kpiTarget>40||difficualty.gameStage>9||(difficualty.gameStage>4&&kpiTarget>=20))&&!expand){
                        expand=true;
                        set1.SetActive(true);
                        Debug.Log("should expand");
                        set1.BroadcastMessage("start",difficualty);
                        canvas.SendMessage("expand");
                        d.str="B:\n We have claimed the store on the other side of the building.\n Since you are doing your job well, the Algorithm has decided to let you operate the shop. Congrats.\nP:\n I am so burnt out after these days and now I\'m having a double workload??\n I require an assistant!\nB:\n Unfortunately we are lacking human resources, and they cost a lot.\n You should count more on TECHNOLOGY, it makes your life easier.\nA:\n And you have my assistance already.";
                        GameObject.Find("Main Camera").GetComponent<CameraTask>().case1=true;
                    }
                if(difficualty.gameStage%3==2)
                    d.dialogType|=2;
                player.SetActive(false);
                canvas.SendMessage("conversationStart",d);
                kpi=0;
                miss=0;
            }
        }
        textM.text="Current/Target:"+kpi.ToString()+"/"+kpiTarget.ToString();
        //text.text=kpi.ToString()+"\t"+kpiTarget.ToString()+"\t"+((int)curtime).ToString();
    }
}

public class Difficualty{
    public int gameStage;
    public bool expand;
    public Difficualty(){
        gameStage=0;
        expand=false;
    }
}

public class dialog{
    public int dialogType {get;set;}
    public int kpi {get;set;}
    public int target {get;set;}
    public int bounes{get;set;}
    public int miss{get;set;}
    public int diliverd{get;set;}
    public string str{get;set;}
    public dialog(){
        dialogType=0;
        kpi=0;
        target=0;
        bounes=0;
        miss=0;
        diliverd=0;
        str="";
    }
    public override string ToString(){
        string str="A:\nYou have delivered "+diliverd.ToString()+" cups of dirnk,\nmissed "+miss.ToString()+" cups of drink today.\nYou have got "+kpi.ToString()+" kpi, which Target is "+target.ToString()+", earned $"+(500*bounes)+" bounes.\n"+this.str;
        return str;
    }
}