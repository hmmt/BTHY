using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//this script needs 3 object: creator(may be button), list parent(RectTransform), text for instantiate
public class TextListScript : MonoBehaviour{
    public GameObject makeObject;
    public RectTransform List;
    public List<RectTransform> child;
    
    public int count = 0;
    private float posy = 0.0f;
    private bool sorted = false;
    private float Ysize;
    private List<GameObject> iconObjectList = new List<GameObject>();
    public float initialPos;
    public float spacing;
    public int fontSize;
    private Color bright, dark;
    public Sprite BS, DS;

    public void Start() {
        dark = new Color(230, 209, 179);
        bright = new Color(249, 240, 226);
    }

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

    public void MakeTraits(TraitTypeInfo trait)
    {
        GameObject addObject = Instantiate(makeObject);
        RectTransform rt = addObject.GetComponent<RectTransform>();
        RectTransform iconGrid = addObject.transform.GetChild(1).GetComponent<RectTransform>();
        string[] desc;
        Sprite[] iconList = TraitIcon.instnace.GetSpriteByTrait(trait, out desc);

        for (int i = 0; i < iconList.Length; i++) {
            GameObject iconObject = ResourceCache.instance.LoadPrefab("Slot/IconImage");
            iconObject.GetComponent<Image>().sprite = iconList[i];
            OverlayObject icons = iconObject.GetComponent<OverlayObject>();
            icons.text = desc[i];

            EventTrigger tri = iconObject.AddComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry();
            EventTrigger.Entry exit = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            exit.eventID = EventTriggerType.PointerExit;
            //iconObject.GetComponent<OverlayScript>().text = desc[i];
            //iconObject.GetComponent<OverlayScript>().Init(desc[i]);
            iconObject.transform.SetParent(iconGrid);
            
            enter.callback.AddListener((eventdata) => { icons.Overlay(); });
            exit.callback.AddListener((eventdata) => { icons.Hide(); });
            tri.triggers.Add(enter);
            tri.triggers.Add(exit);
            iconObjectList.Add(iconObject);
        }
        
        if ((child.Count % 2) == 0)
        {
            addObject.transform.GetComponent<Image>().sprite = BS;
        }
        else
        {
            addObject.transform.GetComponent<Image>().sprite = DS;
        }

        Text addText = addObject.transform.GetChild(0).GetComponent<Text>();

        if (fontSize == 0)
        {
            fontSize = addText.fontSize;
        }
        else
        {
            addText.fontSize = fontSize;
        }

        addObject.SetActive(true);//might not be needed
        addObject.transform.SetParent(List, false);
        float h = addText.preferredHeight;

        addText.text = "" + trait.name;

        rt.sizeDelta = new Vector2(List.rect.width, h + spacing);

        RectTransform textRect = addText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(List.rect.width * 0.45f, textRect.sizeDelta.y);

        AddComponents(rt);
        child.Add(rt);
        return ;
    }

    public void MakeTextWithTraits(string text, float size) {
        GameObject addText = Instantiate(makeObject);
        RectTransform rt = addText.GetComponent<RectTransform>();
        size = 150f;
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
        else
        {
            addText.transform.GetChild(0).GetComponent<Text>().fontSize = fontSize;
        }

        Text t = addText.transform.GetChild(0).GetComponent<Text>();
        t.text = text;

        addText.SetActive(true);//might not be needed
        addText.transform.SetParent(List, false);
        float h = addText.transform.GetChild(0).GetComponent<Text>().preferredHeight;
        rt.sizeDelta = new Vector2(List.rect.width * 0.45f, h + spacing);

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
           // rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y );
            rt.localPosition = new Vector3(0.0f, - posy, 0.0f);
            posy += rt.rect.height;
        }

        Vector2 scrollRectSize = List.sizeDelta;
        scrollRectSize.y = posy;
        List.sizeDelta = scrollRectSize;
    }

    public void SortBgListWithTraits() {
        float posy = 0.0f;

        initialPos = List.rect.height / 2;
        for (int i = 0; i < child.Count; i++)
        {
            RectTransform rt = child[i];
            //rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);
            rt.localPosition = new Vector3(-List.rect.width/2,  -posy, 0.0f);
            posy += rt.rect.height;
        }

        Vector2 scrollRectSize = List.sizeDelta;
        scrollRectSize.y = posy;
        List.sizeDelta = scrollRectSize;
    }

    public void DeleteAll()
    {
        foreach (GameObject o in iconObjectList) {
            Destroy(o);
        }

        foreach (RectTransform c in child) {
            Destroy(c.gameObject);
        }
        child.Clear();
        count = 0;
        posy = 0.0f;
    }

    public void Dummy() {
        Debug.Log(":>ADFdaf");
    }
}
