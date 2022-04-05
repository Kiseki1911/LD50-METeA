using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float mod = 1f;
    public SpriteRenderer sr;
    private Transform hands;
    private float inputX;
    private float inputY;
    private bool isMoving;
    private Vector2 movementInput;
    private Animator[] animators;

    private Transform holdingTea;

    public teacup cup;
    public teacup backupCup;
    private int currency;
    public GameObject logic;
    public int prosthesis;
    public UnityEngine.UI.Text text;
    public Vector3 respawn;

    #region 
    private SpriteRenderer headR;
    private SpriteRenderer bodyR;
    private SpriteRenderer legsR;
    private SpriteRenderer armsR;
    private float increase=1f;

    public bool head = false;
    public bool body = false;
    public bool legs = false;
    public bool arms = false;
    #endregion
    private void Awake() {
        rb=GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        hands = transform.GetChild(3).transform;
        holdingTea = transform.GetChild(4).transform;
        holdingTea.GetComponent<SpriteRenderer>().enabled=false;
        headR = transform.GetChild(5).GetComponent<SpriteRenderer>();
        bodyR = transform.GetChild(6).GetComponent<SpriteRenderer>();
        legsR = transform.GetChild(7).GetComponent<SpriteRenderer>();
        armsR = hands.GetChild(0).GetComponent<SpriteRenderer>();
        headR.enabled=false;
        bodyR.enabled=false;
        legsR.enabled=false;
        armsR.enabled=false;
        cup=new teacup();
        backupCup=new teacup();
        currency=0;
    }
    private void OnEnable() {
        if(prosthesis>=1)
            legs=true;
        if(prosthesis>=2)
            arms=true;
        if(prosthesis>=3)
            body=true;
        if(prosthesis>=4)
            head=true;
        transform.position=respawn;
        headR.enabled=head;
        bodyR.enabled=body;
        legsR.enabled=legs;
        armsR.enabled=arms;
        cup=new teacup();
        backupCup=new teacup();
    }
    private void OnDisable() {
        text.text="";
    }
    private void Update() {
        GetComponent<Animator>().enabled = GameObject.Find("Main Camera").GetComponent<CameraTask>().case2;
        if(!GetComponent<Animator>().enabled){
            PlayerInput();
            SwitchAnimation();
            Actions();
            CupAction();
        }
        sr.enabled=arms;
        foreach(var a in sr.transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>()){
            a.enabled=false;
        }
        sr.transform.GetChild(0).GetChild(((int)backupCup.tea+1)).GetComponent<SpriteRenderer>().enabled=backupCup.exist;
        sr.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled=backupCup.exist;
        sr.transform.GetChild(0).GetChild(6).GetComponent<SpriteRenderer>().enabled=backupCup.temprature;
    }
    private void FixedUpdate() {
        Movement();
    }
    private void PlayerInput(){
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(KeyCode.LeftShift)){
            mod=1.5f;
        }
        else{
            mod=1f;
        }
        if(inputX > 0)
        {
            transform.localScale = new Vector2(1f, 1f);
        } 
        else if(inputX < 0)
        {
            transform.localScale = new Vector2(-1f, 1f);
        }
        if(inputX!=0 && inputY!=0){
        inputX*=0.707f;
        inputY*=0.707f;
        }
        
        movementInput = new Vector2(inputX,inputY);
        isMoving = movementInput != Vector2.zero;
        

        if(prosthesis>=2){
            if(Input.GetKey("e")){
                if(cup.exist&&!backupCup.exist){
                    backupCup=cup;
                    cup=new teacup();
                    
                }
            }
            if(Input.GetKeyDown("q")){
                if(backupCup.exist){
                    var temp=backupCup;
                    backupCup=cup;
                    cup=temp;
                }
            }
        }
        if(prosthesis>=3){
            if(Input.GetKey("f")){
                if(cup.exist&&cup.tea!=TeaType.TYPECOUNT)
                    cup.temprature=true;
            }
        }

        
    }

    private void CupAction(){
        holdingTea.GetComponent<SpriteRenderer>().enabled=cup.exist;
        //holdingTea.GetChild((int)cup.tea).GetComponent<SpriteRenderer>().enabled=true;
        var srs = holdingTea.GetComponentsInChildren<SpriteRenderer>();
        transform.GetChild(9).GetComponent<SpriteRenderer>().enabled=cup.temprature;
            for(int i =0; i< 5;i++){
                if(i==(int)cup.tea)
                    srs[i+1].enabled=true;
                else
                    srs[i+1].enabled=false;
            }
        
    }
    private void Actions(){
        if(cup.exist){
            hands.localScale = new Vector2(1f,-1f);
        }
        else{
            hands.localScale = new Vector2(1f,1f);
        }
        if(prosthesis>=1)
            increase=1.5f;
        headR.enabled=head;
        bodyR.enabled=body;
        legsR.enabled=legs;
        armsR.enabled=arms;
    }
    private void Movement(){
        //rb.AddForce(movementInput*speed);
        rb.MovePosition(rb.position+movementInput*mod*speed*Time.deltaTime*increase);
    }
    private void SwitchAnimation(){
        foreach(var anim in animators){
            anim.SetBool("IsMoving",isMoving);
            //if(isMoving){
                //anim.SetFloat("InputX",inputX);
                anim.SetFloat("InputY",inputY);
            //}
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag=="item"){
            text.text=other.gameObject.name;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag=="item"){
            text.text="";
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(Input.GetKey("space")){
            if(other.gameObject.tag=="item"){
                switch(other.gameObject.name){
                    case("cups"):
                        cup.exist=true;
                        break;
                    case("garbage"):
                        cup=new teacup();
                        break;
                    case("drinks-white"):
                        if(cup.exist&&(cup.tea==TeaType.TYPECOUNT))
                            cup.tea=TeaType.WHITE;
                        break;
                    case("drinks-brown"):
                        if(cup.exist&&(cup.tea==TeaType.TYPECOUNT))
                            cup.tea=TeaType.BROWN;
                        break;
                    case("drinks-pink"):
                        if(cup.exist&&(cup.tea==TeaType.TYPECOUNT))
                            cup.tea=TeaType.PINK;
                        break;
                    case("drinks-orange"):
                        if(cup.exist&&(cup.tea==TeaType.TYPECOUNT))
                            cup.tea=TeaType.ORANGE;
                        break;
                    case("drinks-green"):
                        if(cup.exist&&(cup.tea==TeaType.TYPECOUNT))
                            cup.tea=TeaType.GREEN;
                        break;
                    case("heater"):
                        if(cup.exist&&(cup.tea!=TeaType.TYPECOUNT)){
                            cup.temprature=true;
                        }
                        break;
                    case("logo"):
                        if(cup.exist&&(cup.tea!=TeaType.TYPECOUNT)){
                            logic.SendMessage("sendTea",cup);
                        }
                        break;
                    case("logo1"):
                        if(cup.exist&&(cup.tea!=TeaType.TYPECOUNT)){
                            cup.desk=1;
                            logic.SendMessage("sendTea",cup);
                        }
                        break;
                }
            }
        }
    }

    public bool addProsthesis(){
        if(currency<5000)
            return false;
        switch(prosthesis){
            case(1):
                Debug.Log("leg");
                increase=1.5f;
                legs=true;
                break;
            case(2):
                Debug.Log("arm");
                arms=true;
                break;
            case(3):
                Debug.Log("chest");
                body=true;
                break;
            case(4):
                Debug.Log("gameEnd");
                head=true;
                break;
        }
        prosthesis++;
        return true;
    }

    public void addForceProsthesis(){
        prosthesis++;
        currency-=5000;
        switch(prosthesis){
            case(1):
                Debug.Log("leg");
                increase=1.5f;
                legs=true;
                break;
            case(2):
                Debug.Log("arm");
                arms=true;
                break;
            case(3):
                Debug.Log("chest");
                body=true;
                break;
            case(4):
                Debug.Log("gameEnd");
                head=true;
                break;
        }
    }

    public void recieved(){
        cup=new teacup();
        GetComponent<AudioSource>().Play();
    }
    public void pay(int bounes){
        currency+=500*bounes;
    }
}

public class teacup{
    public bool exist {get;set;}
    public TeaType tea {get;set;}
    public bool temprature {get;set;}
    public int desk{get;set;}
    public teacup(){
        exist=false;
        tea=TeaType.TYPECOUNT;
        temprature=false;
        desk=0;
    }
    public bool Equals(teacup o){
        return o.tea==tea&&temprature==temprature&&desk==desk;
    }

    static public bool operator==(teacup o,teacup a){
        return o.tea==a.tea&&o.temprature==a.temprature&&o.desk==a.desk;
    }

    static public bool operator!=(teacup o,teacup a){
        return !(o.tea==a.tea&&o.temprature==a.temprature&&o.desk==a.desk);
    }
}