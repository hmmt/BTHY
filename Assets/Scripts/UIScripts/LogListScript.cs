using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LogListScript : MonoBehaviour, IObserver {
    public GameObject LogText;
    public RectTransform List;
    public RectTransform criteria;
    public List<RectTransform> child;

    public Sprite[] bgImage;

    public bool narration = false;
    public float initialPos;
    public float initialX;
    public float spacing;
    public float initialY;
    private float scrollInitial;

    public void Start() {
        initialY = criteria.rect.height;
        scrollInitial = List.localPosition.y;
    }

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

        if ("AddSystemLog" == notice && !narration)
        {
            MakeTextWithBg("  " + (string)param[0]);
            initialX = List.rect.width / 2;
        }

        SortBgList();
    }

    public void pEnter() {
        Debug.Log(List.rect.height);
    }
    
    public void MakeTextWithBg(string text)
    {
        GameObject addText = Instantiate(LogText);
        RectTransform rt = addText.GetComponent<RectTransform>();

        /*
        if ((child.Count % 2) == 0)
        {
            addText.transform.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi"); ;
        }
        else
        {
            addText.transform.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
        }
        */
        addText.transform.GetComponent<Image>().sprite = bgImage[0];

        addText.transform.GetChild(0).GetComponent<Text>().text = text;

        addText.SetActive(true);//might not be needed
        addText.transform.SetParent(List, false);
        float h = addText.transform.GetChild(0).GetComponent<Text>().preferredHeight;
        rt.sizeDelta = new Vector2(List.rect.width-10f, h + 10f);

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
        float posx;
        posx = List.rect.width / 2;
       // initialPos = List.rect.height / 2;
        //Debug.Log("InitialPos: " + initialPos);
        for (int i = 0; i < child.Count; i++)
        {
            RectTransform rt = child[i];
            
            //float locY = - (rt.rect.height/2 + posy);  
            float size = rt.rect.height;
           
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);
            rt.localPosition = new Vector3(posx, posy, 0.0f);
            posy -= (size + spacing);
           
        }
        Vector2 rectSize = List.sizeDelta;
        rectSize.y = -posy - initialY;
        //Debug.Log("posy"+ posy);
        List.sizeDelta = rectSize;

        float move = List.rect.height;
        if (move > initialY) {
            float heightformove = move - initialY;
            List.localPosition = new Vector3(List.localPosition.x, scrollInitial+ heightformove, 0.0f);
        
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
