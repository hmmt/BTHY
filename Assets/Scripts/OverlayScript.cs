using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IOverlay{
    void Overlay();
    void Hide();    
}

public class OverlayScript : MonoBehaviour{
    private static OverlayScript _instance;
    public static OverlayScript instance {
        get { return _instance; }
    }
    private Text textBox;
    public RectTransform box;
    private RectTransform parent;
    public RectTransform canvas;
    public float spacingX, spacingY;
    private RectTransform bg;
    private RectTransform contentBox;
    private static float INF = -1000000;
    private bool _displayed;

    void Awake() {
        _instance = this;
        parent = this.GetComponent<RectTransform>();
        bg = box.GetChild(0).GetComponent<RectTransform>();
        //contentBox = box.GetChild(1).GetComponent<RectTransform>();
        contentBox = box.GetComponent<RectTransform>();
        textBox = contentBox.GetComponent<Text>();
        _displayed = false;
    }

    public void Start() {
        Hide();
    }

    private void SetBg() {
        float h = textBox.preferredHeight + spacingY;
        float w = textBox.preferredWidth + spacingX;

        bg.sizeDelta = new Vector2(w, h);
    }

    public void Overlay(GameObject obj, string content){
        if (_displayed) return;
        _displayed = true;
        RectTransform posRect = obj.GetComponent<RectTransform>();
        box.SetParent(posRect);
        box.localPosition = Vector3.zero;
        box.anchoredPosition = new Vector2(posRect.anchoredPosition.x,
                                            posRect.anchoredPosition.y + posRect.rect.height/2);
        //Debug.Log(box.anchoredPosition);
        box.SetParent(parent);
        //Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);
        //Vector2 world = Camera.main.ViewportToWorldPoint(viewportPoint);
        textBox.text = content;
        SetBg();
    }

    public void Hide() {
        if (!_displayed) return;
        _displayed = false;
        box.localPosition = new Vector3(0f, 0f, INF);
        box.SetParent(parent);
        SetBg();
    }

    /*
    private GameObject target;
    //public string text;
    private Text content;
    private Transform parent;
    private EventTrigger trigger;
    
    public void Awake() {
        target = ResourceCache.instance.LoadPrefab("Overlay/StdPanel");
        parent = GameObject.FindWithTag("OverlayLayer").GetComponent<Transform>();
        //target = Instantiate(prefab);
        target.transform.SetParent(transform);
        RectTransform rt = this.GetComponent<RectTransform>();
        Vector2 pos = new Vector2(rt.rect.width/2, rt.rect.height/2);
        Debug.Log(rt.rect);
        Vector3 loc = new Vector3(pos.x + this.transform.localPosition.x,
                                    -pos.y + this.transform.localPosition.y,
                                    0f);
        target.transform.localPosition = loc;
        content = target.transform.GetChild(0).GetComponent<Text>() ;
        trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { PointerEnter(); });
        //trigger.delegates.Add(entry);
        trigger.triggers.Add(entry);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((eventData) => { PointerExit(); });
        trigger.triggers.Add(entry2);

        //target.transform.SetParent(parent, false);
        target.SetActive(false);
    }

    public void Init(string t) {
        this.content.text = t;
    }

    public void PointerEnter()
    {
        target.SetActive(true);
    }

    public void PointerExit()
    {
        target.SetActive(false);
    }

    public void OnNotice(string notice, params object[] param)
    {
        throw new System.NotImplementedException();
    }*/
}
