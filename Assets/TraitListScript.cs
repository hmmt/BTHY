using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TraitListScript : MonoBehaviour {
    public GameObject traitObject;
    public RectTransform parent;
    public List<RectTransform> child;
    public int count;
    public float initialSize;
    public float size;
    private float Ysize;

    public void MakeTrait(string text)
    {
        Debug.Log(traitObject.transform.GetChild(0) + " " + traitObject.transform.GetChild(0).GetChild(0));
        GameObject traitPanel = Instantiate(traitObject);
        GameObject textPanel = traitPanel.transform.GetChild(0).GetChild(0).gameObject;
        textPanel.GetComponent<Text>().text = text;

        textPanel.SetActive(true);
        traitPanel.transform.SetParent(parent, false);
        RectAdd(traitPanel.GetComponent<RectTransform>());
        
    }

    public void RectAdd(RectTransform add) {
        add.SetParent(parent);
        child.Add(add);
    }

    public void SortTrait()
    {
        float posy = 0.0f;
        for (int i = 0; i < child.Count; i++) {

            RectTransform rt = child[i];
            Text t = child[i].GetComponent<Text>();
            float next = size;
            rt.localPosition = new Vector3(0.0f, initialSize - posy, 0.0f);
            Debug.Log("trait pos:" + rt.localPosition);
            posy += next;
        }
    }

    public void DeleteAll() {
        foreach (RectTransform c in child) {
            Destroy(c.gameObject);
        }

        child.Clear();
    }
}
