﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LogListScript : MonoBehaviour, IObserver {
    public GameObject LogText;
    public RectTransform List;
    public List<RectTransform> child;

    public float initialPos;
    public float initialX;


    void OnEnable()
    {
        Notice.instance.Observe("AddSystemLog", this);
    }

    void OnDisable()
    {
        Notice.instance.Remove("AddSystemLog", this);
    }

    public void OnNotice(string notice, params object[] param)
    {

        if ("AddSystemLog" == notice)
        {
            MakeTextWithBg(" " + (string)param[0]);
            initialX = List.rect.width / 2;
        }

        SortBgList();
    }
    
    public void MakeTextWithBg(string text)
    {
        GameObject addText = Instantiate(LogText);
        RectTransform rt = addText.GetComponent<RectTransform>();


        if ((child.Count % 2) == 0)
        {
            addText.transform.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi"); ;
        }
        else
        {
            addText.transform.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
        }


        addText.transform.GetChild(0).GetComponent<Text>().text = text;

        addText.SetActive(true);//might not be needed
        addText.transform.SetParent(List, false);
        float h = addText.transform.GetChild(0).GetComponent<Text>().preferredHeight;
        rt.sizeDelta = new Vector2(List.rect.width, h);

        AddComponents(rt);
        child.Add(rt);
    }

    public void AddComponents(RectTransform add)
    {
        add.SetParent(List);
    }

    public void SortBgList()
    {
        float posy = 0.0f;

        initialPos = List.rect.height / 2;
        Debug.Log("InitialPos: " + initialPos);
        for (int i = 0; i < child.Count; i++)
        {
            RectTransform rt = child[i];
            
            float locY = - (rt.rect.height/2 + posy);
            //rt.localPosition = Vector3.zero;
            rt.localPosition = new Vector3(initialX, locY, 0.0f);
            posy += rt.rect.height;
           
        }
    }

    public void DeleteAll()
    {
        foreach (RectTransform c in child)
        {
            Destroy(c.gameObject);
        }
        child.Clear();
    }


}
