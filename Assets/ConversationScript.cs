using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ConversationScript : MonoBehaviour {
    public RectTransform list;
    public RectTransform selectList;
    public RectTransform maximumSize;
    public GameObject Left;
    public GameObject Right;
    public GameObject Select;
    public GameObject Sysmessage;
    
    public int cnt = 0;
    public float spacing;

    public float posleft,posright;
    private List<GameObject> selectObject = new List<GameObject>();
    private float tempvalue;
    private float startPosy;
    private float startHeight;
    private float posy = 0.0f;
    public Day1Script dayScript;

    public void Start() {
        if (spacing < 1.0f) {
            spacing = 1.0f;
        }
        startHeight = maximumSize.sizeDelta.y;
        //dayScript = this.GetComponent<Day1Script>();
    }

    public void ToUpper() {
        list.anchoredPosition = new Vector2(list.anchoredPosition.x, 0.0f);
    }

    public void LeftMake(string text) {
        MakeText(Left, text, -50f);
    }

    public void SelectMake(string[] text) {
        //ClearSelect();
        int[] indexAry = new int[text.Length];
        
        float yPos = 0.0f;
        for (int i = 0; i < text.Length; i++) {
            indexAry[i] = i;
            GameObject obj = Instantiate(Select);

            Text te = obj.GetComponent<Text>();
            te.text = text[i];
            float size = te.preferredHeight;
            obj.transform.SetParent(selectList);
            obj.transform.localScale = Vector3.one;
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0.0f, -10f - yPos);
            yPos += size + 10f;

            EventTrigger trigger = obj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            Debug.Log(obj.transform.GetSiblingIndex());
            entry.callback.AddListener((eventdata) => { OnSelect(obj.transform.GetSiblingIndex()); });
            trigger.triggers.Add(entry);
            selectObject.Add(obj);
        }
    }

    public void SysMake(string text) {
        MakeText(Sysmessage, text, 0f);
    }

    public void OnSelect(int index) {
        dayScript.OnSelect(index + 1);
    }

    public void ClearSelect() {
        foreach (GameObject obj in selectObject) {
            Destroy(obj);

        }
        selectObject.Clear();
    }

    public void RightMake(string text) {
        MakeText(Right, text, 50f);
    }

    public void MakeText(GameObject target) {
        GameObject box = Instantiate(target); 
        box.transform.localScale = Vector3.one;
        RectTransform rect = box.GetComponent<RectTransform>();
        RectTransform textRect = box.transform.GetChild(0).GetComponent<RectTransform>();

        string str = cnt+ "";
        cnt++;
        //Text t = box.transform.GetComponent<Text>();
        Text t = textRect.GetComponent<Text>();
        t.text = str;
        rect.sizeDelta = textRect.sizeDelta;
        rect.SetParent(list);
       
        SetList(rect);
    }

    public void MakeText(GameObject target, string text, float posX)
    {
        GameObject box = Instantiate(target);
        
        RectTransform rect = box.GetComponent<RectTransform>();
        RectTransform child = box.transform.GetChild(0).GetComponent<RectTransform>();
        string str = text;

        Text t = child.GetComponent<Text>();
        t.text = str;
        tempvalue = t.preferredHeight;
        rect.sizeDelta = new Vector2(child.rect.width, t.preferredHeight + 5f);
        child.localPosition = new Vector3(child.localPosition.x, -5f, 0.0f);
        SetList(rect, posX);
    }

    public void SetList(RectTransform rect) {

        rect.SetParent(list.transform);
        rect.localScale = new Vector3(1f,1f, 1f);

        float add = rect.sizeDelta.y;
        rect.localPosition = new Vector3(0.0f, -posy, 0.0f);
        posy += add;
        if (posy > startHeight) {
            list.sizeDelta = new Vector2(list.sizeDelta.x, posy);
            list.anchoredPosition = new Vector2(list.anchoredPosition.x, posy - startHeight - 5f);
        }
    }

    public void SetList(RectTransform rect, float posx)
    {

        rect.SetParent(list.transform);
        rect.localScale = new Vector3(1f, 1f, 1f);

        float add = tempvalue;
        tempvalue = 0.0f;
        rect.localPosition = new Vector3(posx, -posy, 0.0f);
        posy += (add + spacing);
        //Debug.Log("local y: " + rect.localPosition.y + "posy: " + posy);
        if (posy > startHeight)
        {
            list.sizeDelta = new Vector2(list.sizeDelta.x, posy);
            
            list.anchoredPosition = new Vector2(list.anchoredPosition.x, posy - startHeight);
        }
    }

    public void FixedUpdate() {
        /*
        if (Input.GetKeyDown(KeyCode.Q)) {
            MakeText(Left, "test : " + cnt, -240f);
            cnt++;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MakeText(Right, "test : " + cnt, 240f);
            cnt++;
        }*/
    }
}
