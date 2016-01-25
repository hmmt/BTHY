using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OnCursorEnter : MonoBehaviour {
    //public    Text popup;
    public Image popup;
    RectTransform textRect;
    Vector3 cursorPos;

    float screenW, screenH;
    int delaycnt = 10;
    int cnt;
    bool cursorEnter = false;

	// Use this for initialization
	void Start () {
        cnt = 0;
       // popup.gameObject.SetActive(false);
        textRect = popup.GetComponent<RectTransform>();
        setPopupActive(false);
        screenW = Screen.width;
        screenH = Screen.height;
        //Debug.Log(screenW + " " + screenH);
	}
	
	// Update is called once per frame
	void Update () {
        if (cursorEnter == true) {
            cursorPos = Input.mousePosition;
            Rect rect = popup.rectTransform.rect;
            Vector3 setPos;
            float tempX;
            float tempY;

            tempX = cursorPos.x + rect.width / 2;
            tempY = cursorPos.y - rect.height / 2;

            if (cursorPos.x + rect.width > screenW)
            {
                tempX = cursorPos.x - rect.width/2;
            }

            if (cursorPos.y - rect.height < 0)
            {
                tempY = cursorPos.y + rect.height/2;
            }
            
            setPos = new Vector3(tempX, tempY);
            popup.rectTransform.position = setPos;
        }
	}

    IEnumerator EnableUI()
    {
        yield return new WaitForSeconds(0.5f);
       // Debug.Log(Input.mousePosition.x + " " + Input.mousePosition.y);
        //Debug.Log(textRect.width + "+" + textRect.height);
        if (cursorEnter == true) {
            setPopupActive(true);
            //textRect.
            
        }
        
    }

    private void setPopupActive(bool state) {
        popup.gameObject.SetActive(state);
    }

    public void Enter() {
        if (cursorEnter != true) {
            cursorEnter = true;
            //Debug.Log(cursorEnter);
            StartCoroutine(EnableUI());
        }
    }
    
    public void Exit() {
        cursorEnter = false;
        Debug.Log("exit");
        //나갈 때 코루틴 제거부분이 필요할 수 있음.
        setPopupActive(false);
    }

    
}
