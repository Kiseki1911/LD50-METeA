using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTask : MonoBehaviour
{
    public bool case1=false;
    public bool case2 = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(case1&&!case2){
            transform.position=Vector3.Lerp(new Vector3(4.06f,-1.52f,-10),new Vector3(-1.61f,-1.52f,-10f),2f);
        }
        if(case1&&case2){
            transform.position=Vector3.Lerp(new Vector3(-1.61f,-1.52f,-10f),new Vector3(4.06f,-1.52f,-10),2f);
            GetComponent<Camera>().orthographicSize=Mathf.MoveTowards (Camera.main.orthographicSize, 2.7f, 2.0f * Time.deltaTime);
        }
    }
}
