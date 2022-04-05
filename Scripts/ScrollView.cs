using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{
    private ScrollRect m_ScrollView;
    // Start is called before the first frame update
    void Start()
    {
        m_ScrollView = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        m_ScrollView.onValueChanged.AddListener(ScrollValueChange);
    }

    private void ScrollValueChange(Vector2 ve2){
        //Debug.Log("moving");
        if (ve2==new Vector2(1,1)){
            Debug.Log("top");
        }
        if (ve2==new Vector2(0,0)){
            Debug.Log("bottom");
        }
    }
}
