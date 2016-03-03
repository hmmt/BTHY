using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogItemScript : MonoBehaviour {
    private int _index;
    public int index {
        get { return _index; }
        set { _index = value; }
    }

    public GameObject textObject;
    public static Text textTarget;
    public static RectTransform imgRect;
    public static RectTransform textRect;
    public float height;

    public void Awake(){
        textTarget = textObject.GetComponent<Text>();
        imgRect = this.GetComponent<RectTransform>();
        textRect = textObject.GetComponent<RectTransform>();
        //textRect.pivot = imgRect.pivot;
    }

    public void SetText(string context, int size, float h_spacing, float v_spacing) {
        textTarget.text = context;
        textTarget.fontSize = size;
        float maxX = imgRect.rect.width;
        float sizeX = textTarget.preferredWidth;
        int cnt = 0;

        if (maxX < sizeX) {
            cnt = (int)(sizeX / maxX);
            //Debug.Log(cnt);
            sizeX = maxX;
        }

        float sizeY = textTarget.fontSize + 2;
        sizeY *= (cnt + 1);
        sizeY += 1f;//for small space
        sizeY = sizeY + v_spacing;
        imgRect.sizeDelta = new Vector2(sizeX + h_spacing , sizeY + v_spacing);
        textRect.sizeDelta = new Vector2(sizeX, sizeY);
        //textRect.localPosition = Vector2.zero;
        textRect.anchoredPosition = new Vector2(h_spacing/2, 0f);
        this.height = sizeY;
        
    }
    


}
