using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//this script needs 3 object: creator(may be button), list parent(RectTransform), text for instantiate
public class TextListScript : MonoBehaviour{
    public GameObject makeObject;
    public RectTransform List;
    public List<RectTransform> child;
    public int count = 0;
    private float posy = 0.0f;
    private bool sorted = false;
    private float Ysize;
    public float initialPos;
    public float spacing;
    public int fontSize;

    public void MakeTextWithBg(string text) {
        GameObject addText = Instantiate(makeObject);
        RectTransform rt = addText.GetComponent<RectTransform>();
                
        if ((child.Count % 2) == 0)
        {
            addText.transform.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi"); ;
        }
        else
        {
            addText.transform.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
        }

        if (fontSize == 0)
        {
            fontSize = addText.transform.GetChild(0).GetComponent<Text>().fontSize;
        }
        else {
            addText.transform.GetChild(0).GetComponent<Text>().fontSize = fontSize ;
        }


        addText.transform.GetChild(0).GetComponent<Text>().text = text;
        
        addText.SetActive(true);//might not be needed
        addText.transform.SetParent(List, false);
        float h = addText.transform.GetChild(0).GetComponent<Text>().preferredHeight;
        rt.sizeDelta = new Vector2(List.rect.width, h + spacing);
       
        AddComponents(rt);
        child.Add(rt);
    }

    public void AddComponents(RectTransform add) {
        add.SetParent(List);
    }

    public void getDelta(int index) {
        RectTransform rt = transform.GetChild(index).GetComponent<RectTransform>();
    }


    public void SortBgList()
    {
        float posy = 0.0f;

        initialPos = List.rect.height / 2;
        for (int i = 0; i < child.Count; i++)
        {
            RectTransform rt = child[i];
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y );
            rt.localPosition = new Vector3(0.0f, - posy, 0.0f);
            posy += rt.rect.height;
        }

        Vector2 scrollRectSize = List.sizeDelta;
        scrollRectSize.y = posy;
        List.sizeDelta = scrollRectSize;
    }

    public void DeleteAll()
    {
        foreach (RectTransform c in child) {
            Destroy(c.gameObject);
        }
        child.Clear();
        count = 0;
        posy = 0.0f;
    }

}
