using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//this script needs 3 object: creator(may be button), list parent(RectTransform), text for instantiate
public class TextListScript : MonoBehaviour , IObserver{
    public GameObject makeObject;
    public RectTransform List;
    public List<RectTransform> child;
    public int count = 0;
    private float posy = 0.0f;
    private bool sorted = false;
    private float Ysize;
    public float initialPos;

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
            MakeText(" " + (string)param[0]);
        }
        SortList();
    }

    
    public void MakeText(string text) {
        GameObject addText = Instantiate(makeObject);
        
        //addText.GetComponent<TextObjectScript>().SetIndex(count);
        addText.GetComponent<Text>().text = text;
        addText.SetActive(true);//might not be needed
        addText.transform.SetParent(List, false);
        AddComponents(addText.GetComponent<RectTransform>());
        child.Add(addText.GetComponent<RectTransform>());
    }

    public void AddComponents(RectTransform add) {
        if(count == 0)
            Ysize = add.rect.yMax - add.rect.yMin;
        add.SetParent(List);
    }

    public void getDelta(int index) {
        RectTransform rt = transform.GetChild(index).GetComponent<RectTransform>();
    }

    public void SortList() {
        float posy = 0.0f;
        for (int i = 0; i < child.Count; i++)
        {
            RectTransform rt = child[i];
            Text t = child[i].GetComponent<Text>();            

            float next = t.preferredHeight;
            rt.localPosition = new Vector3(0.0f, initialPos - posy, 0.0f);
            Debug.Log(rt.localPosition);
            posy += next;
        }
    }

    public void DeleteAll()
    {
        foreach (RectTransform c in transform.transform) {
            Destroy(c.gameObject);
        }
        child.Clear();
        count = 0;
        posy = 0.0f;
    }

}
