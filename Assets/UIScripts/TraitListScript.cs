using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TraitListScript : MonoBehaviour {
    public GameObject traitObject;
    public RectTransform parent;
    public List<RectTransform> child;
    public int count;
    public float initialPos;
    public float size;
    private float Ysize;
    public Sprite[] bgImage = new Sprite[2];

    public void MakeTrait(string text)
    {
        //Debug.Log(traitObject.transform.GetChild(0) + " " + traitObject.transform.GetChild(0).GetChild(0));
        GameObject traitPanel = Instantiate(traitObject);
        GameObject textPanel = traitPanel.transform.GetChild(0).GetChild(0).gameObject;
        textPanel.GetComponent<Text>().text = text;

        if ((child.Count % 2) == 0)
        {
            traitPanel.GetComponent<Image>().sprite = bgImage[0];
        }
        else {
            traitPanel.GetComponent<Image>().sprite = bgImage[1];
        }

        textPanel.SetActive(true);
        traitPanel.transform.SetParent(parent, false);
        RectAdd(traitPanel.GetComponent<RectTransform>());
        
    }

    public List<GameObject> MakeTrait(TraitTypeInfo trait) {
        List<GameObject> output = new List<GameObject>();
        GameObject traitPanel = Instantiate(traitObject);
        Text textPanel = traitPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        string[] desc;
        Sprite[] icon = TraitIcon.instnace.GetSpriteByTrait(trait, out desc);
        Transform iconGrid = traitPanel.transform.GetChild(1);

        for (int i = 0; i < icon.Length; i++) {
            GameObject iconObject = ResourceCache.instance.LoadPrefab("Slot/IconImage");
            iconObject.GetComponent<Image>().sprite = icon[i];

            OverlayObject icons = iconObject.AddComponent<OverlayObject>();
            icons.text = desc[i];
            EventTrigger trigger = iconObject.AddComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry();
            EventTrigger.Entry exit = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            exit.eventID = EventTriggerType.PointerExit;
            iconObject.transform.SetParent(iconGrid);
            Vector3 iconposition = Camera.main.WorldToViewportPoint(iconObject.transform.position);
            icons.pos = iconposition;
            enter.callback.AddListener((eventdata) => { icons.Overlay(); });
            exit.callback.AddListener((eventdata) => { icons.Hide(); });
            trigger.triggers.Add(enter);
            trigger.triggers.Add(exit);
            output.Add(iconObject);
        }
        
        textPanel.text = trait.name;

        if ((child.Count % 2) == 0)
        {
            traitPanel.GetComponent<Image>().sprite = bgImage[0];
        }
        else
        {
            traitPanel.GetComponent<Image>().sprite = bgImage[1];
        }

        textPanel.gameObject.SetActive(true);
        traitPanel.transform.SetParent(parent, false);
        RectAdd(traitPanel.GetComponent<RectTransform>());

        return output;
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
            rt.localPosition = new Vector3(0.0f, initialPos - posy, 0.0f);
            //Debug.Log("trait pos:" + rt.localPosition);
            posy += size;
        }
    }

    public void DeleteAll() {
        foreach (RectTransform c in child) {
            Destroy(c.gameObject);
        }

        child.Clear();
    }
}
