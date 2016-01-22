﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class SpeakerID {
    public const short ANGELA = 0;
    public const short PLAYER = 1;
    public const short ANONYMOUS = 2;
    public const short OTHER = 3;
}

public class ConversationScript : MonoBehaviour {
    public enum TextPos
    {
        LEFT,
        MID,
        RIGHT
    }
    [System.Serializable]
    public class ConversationTextPanel
    {
        public GameObject textObject;
        public TextPos pos;
    }
    [System.Serializable]
    public class TextPosFloat
    {
        public float left;
        public float mid;
        public float right;
    }

    public TextPosFloat pos;
    public RectTransform list;
    public RectTransform selectList;
    public RectTransform maximumSize;
    public ConversationTextPanel[] objectList;
    //public GameObject Left;
    //public ConversationTextPanel Left;
    //public GameObject Right;
    //public ConversationTextPanel Right;
    public GameObject Select;
    //public ConversationTextPanel Select;
    //public GameObject Anonymous;
    //public ConversationTextPanel Anonymous;
    public GameObject Sysmessage;
    
    private int cnt = 0;
    public float spacing;

    public float posleft,posright;
    private List<GameObject> selectObject = new List<GameObject>();
    private float tempvalue;
    private float startPosy;
    private float startHeight;
    private float posy = 0.0f;

    private float GetPos(TextPos p) {
        switch (p) { 
            case TextPos.LEFT:
                return pos.left;
            case TextPos.MID:
                return pos.mid;
            case TextPos.RIGHT:
                return pos.right;
            default:
                return 0.0f;
        }
    }

    private float GetPos(ConversationTextPanel p)
    {
        return this.GetPos(p.pos);
    }

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

    private ConversationTextPanel GetObject(short id)
    {
        /*
        switch (id) { 
            case SpeakerID.ANGELA://angela
                return objectList[0];
            case SpeakerID.PLAYER://player
                return objectList[1];
            case SpeakerID.ANONYMOUS://anonymous
                return objectList[2];
            case SpeakerID.OTHER:
                return objectList[3];
            default:
                return objectList[0];
        }*/
        if (id < 0 || id > objectList.Length) {
            Debug.Log("index out error in get object");
            return objectList[0];
        }
        return objectList[id];
    }

    public void MakeTextByID(short speaker, string text) {
        ConversationTextPanel temp = GetObject(speaker);
        float posx = GetPos(temp);
        MakeText(temp.textObject, text, posx);
    }

    public void SelectMake(string[] text) {
        ClearSelect();
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
            entry.callback.AddListener((eventdata) => { OnSelect(obj.transform.GetSiblingIndex()); });
            trigger.triggers.Add(entry);
            selectObject.Add(obj);
        }
    }

    public void SysMake(string text) {
        MakeText(Sysmessage, text, 0f);
    }

    public void OnSelect(int index) {
        ConversationUnit.instance.OnSelect(index + 1);
    }

    public void ClearSelect() {
        foreach (GameObject obj in selectObject) {
            Destroy(obj);

        }
        selectObject.Clear();
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
