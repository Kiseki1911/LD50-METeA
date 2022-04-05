using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TeaType{
    WHITE=0,
    BROWN=1,
    PINK=2,
    ORANGE=3,
    GREEN=4,
    TYPECOUNT=5
};

public class NPCGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    public int gameStage;
    public Difficualty difficualty;
    public float patients,curPatient;
    public teacup tea;
    public int state;
    public GameObject newObject;
    public Sprite[] imageList;
    public Sprite[] normal;
    public Sprite angry;
    private bool showed=false;
    private bool inposition=true;
    public Vector3[] pos;
    private float speed=0.07f;
    public bool expand;
    float difficualt;
    private Rigidbody2D rb;
    void genNpc(){
        tea=new teacup();
        //SpriteRenderer sr=gameObject.GetComponent<SpriteRenderer>();
        //sr.sprite=normal;
        transform.GetComponent<SpriteRenderer>().sprite=normal[Random.Range(0,3)];
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled=false;
        difficualt=0;
        for(int i=0;i<this.difficualty.gameStage;i++){
            difficualt+=Random.value;
        }
        difficualt+=(difficualty.gameStage+1)/2f;
        patients=(int)(10f/difficualt+3f);
        if(difficualty.expand)
            {if(this.difficualty.gameStage<3){
                tea.tea=(TeaType)Random.Range(0,2);
            }
            else if(this.difficualty.gameStage<6){
                tea.tea=(TeaType)Random.Range(0,3);
            }
            else if(this.difficualty.gameStage<9){
                tea.tea=(TeaType)Random.Range(0,4);
            }
            else if(this.difficualty.gameStage<12){
                tea.tea=(TeaType)Random.Range(0,5);
            }
            else{
                tea.tea=(TeaType)Random.Range(0,6);
            }
            }
        else{
            if(this.difficualty.gameStage<4){
                tea.tea=(TeaType)Random.Range(0,2);
            }
            else{
                tea.tea=(TeaType)Random.Range(0,3);
            }
        }
        if(difficualty.gameStage>4){
            tea.temprature=Random.value>0.5f;
        }
        if(expand){
            tea.desk=1;
        }
        else{
            tea.desk=0;
        }
        foreach(var temp in transform.GetChild(1).GetComponentsInChildren<SpriteRenderer>()){
            temp.enabled=false;
        }
        curPatient=patients;
    }

    void finished(){
        state=4;
        Destroy(newObject);
        showed=false;
    }

    void restart(int temp){
        state=temp-1;
    }

    void showTea(){
        transform.GetChild(1).GetComponent<SpriteRenderer>().enabled=true;
        transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().enabled=true;
        transform.GetChild(1).GetChild((int)tea.tea+1).GetComponent<SpriteRenderer>().enabled=true;
        transform.GetChild(1).GetChild(6).GetComponent<SpriteRenderer>().enabled=tea.temprature;
        Debug.Log(tea.tea.ToString()+tea.temprature.ToString());
        showed=true;
    }

    void nextState(){
        state--;
        if(state==-2)
            state=2;
        inposition = false;
    }

    public void reciveTea(teacup teaType){
        
        if(state==0&&showed){
            Debug.Log(teaType.tea.ToString()+teaType.temprature.ToString());
            if(teaType==tea){
                Debug.Log(true);
                int score;
                Debug.Log(gameObject.name);
                if(curPatient/patients>0.6)
                score=3;
                else if(curPatient/patients>0.3)
                score=2;
                else
                score=1;
                GameObject.Find("LogicUnit").SendMessage("recieved",score);
                transform.GetChild(1).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(1).GetChild((int)tea.tea+1).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(1).GetChild(6).GetComponent<SpriteRenderer>().enabled=false;
                showed=false;
            }
            else{
                Debug.Log(gameObject.name);
                GameObject.Find("LogicUnit").SendMessage("recieved",-1);
            }
                
        }
    }
    void start(Difficualty diff){
        difficualty=diff;
        genNpc();
        switch(state){
            case 0:gameObject.transform.position=pos[1]; break;
            case 1:gameObject.transform.position=pos[2]; break;
            case 2:gameObject.transform.position=pos[3]; break;
        }
        if(state==0){
            showTea();
        }
    }
    private void Awake() {
        rb = transform.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    private void FixedUpdate() {
        if(showed&&state==0){
            curPatient-=Time.deltaTime;
            if(curPatient<0){
                GameObject.Find("LogicUnit").SendMessage("timeout",expand);
                
                //SpriteRenderer sr=gameObject.GetComponent<SpriteRenderer>();
                //sr.sprite=angry;
                transform.GetChild(1).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(1).GetChild((int)tea.tea+1).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(1).GetChild(6).GetComponent<SpriteRenderer>().enabled=false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled=true;
                showed=false;
            }
        }
        switch(state){
            case -1:
                if(!inposition)
                    if((gameObject.transform.position-pos[0]).magnitude>=0.4)
                        rb.MovePosition(gameObject.transform.position+((pos[0]-gameObject.transform.position).normalized)*(speed+0.03f*difficualt));
                    else{
                        genNpc();
                        gameObject.transform.position=pos[4];
                        state=2;
                    }
                break;
            case 0:
                if(!inposition)
                    if((gameObject.transform.position-pos[1]).magnitude>=0.2)
                        rb.MovePosition(gameObject.transform.position+((pos[1]-gameObject.transform.position).normalized)*(speed+0.03f*difficualt));
                    else if(!showed){
                        showTea();
                        inposition = true;
                    }
                break;
            case 1:
                if(!inposition)
                    if((gameObject.transform.position-pos[2]).magnitude>=0.2)
                        rb.MovePosition(gameObject.transform.position+((pos[2]-gameObject.transform.position).normalized)*(speed+0.03f*difficualt));
                    else{
                        inposition = true;
                    }
                break;
            case 2:
                if(!inposition)
                    if((gameObject.transform.position-pos[3]).magnitude>=0.2)
                        rb.MovePosition(gameObject.transform.position+((pos[3]-gameObject.transform.position).normalized)*(speed+0.03f*difficualt));
                    else{
                        inposition = true;
                    }
                break;
        }
    }
}
