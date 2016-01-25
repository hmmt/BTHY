using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*this script determines the text object's position, not content*/
public class TextExtendScript : MonoBehaviour {    
    public float spacing;
    public int childIndexLength;
    public List<GameObject> list;
    public int count = 0;

    public void Start(){
        
        
    }

    public void addComponets(RectTransform add) { 
    
    }

    public void setPos() {
        float posy = 0.0f;
        float posx = transform.position.x;
        float posz = transform.position.z;

        for (int i = 0; i < list.Count; i++) {
            GameObject child = list[i];
            RectTransform tr = child.GetComponent<RectTransform>();
            TextObjectScript script = child.GetComponent<TextObjectScript>();
            tr.localScale = Vector3.one;
            tr.offsetMin = Vector2.zero;

            tr.offsetMax = new Vector2(0.0f, posy);
           // Debug.Log(script.getDt());
           // float next = script.getDt();
            /*
            if (next > 0) {
                next *= -1;
            }
            posy = posy + (next * 2) - spacing;
            */
        }
        /*
        foreach (GameObject child in list) {
            RectTransform rect = child.GetComponent<RectTransform>();
                   
            
            TextObjectScript script = child.GetComponent<TextObjectScript>();
            
        } */   
    }
    /*
    public void Update() {
        if (indexCnt != list.Count) {
            foreach(


            indexCnt = list.Count;
        }
    }*/
}
