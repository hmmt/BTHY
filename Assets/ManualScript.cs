using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManualScript : MonoBehaviour {
    private Animator anim;
    private bool animActivate;
    private bool extended = false;
    private RectTransform rect;

    private Sprite[] ImageList;
    private int _cnt;
    private int _max;
    private bool _onDrag;
    private bool _exported;
    private RectTransform destroyArea;

    public GameObject Std, Extended;
    public Image front;
    public Image left;
    public Image right;
    public Sprite None;
    public string directory;

    public void Start() {
        anim = this.GetComponent<Animator>();
        animActivate = anim.gameObject.activeSelf;
        rect = this.GetComponent<RectTransform>();
        ImageList = Resources.LoadAll<Sprite>("Sprites/Manual/"+directory);
        _cnt = 0;
        _onDrag = false;
        _max = (int)ImageList.Length / 2;
        Std.gameObject.SetActive(true);
        Extended.gameObject.SetActive(false);
        front.sprite = ImageList[0];
        _exported = false;
        destroyArea = transform.parent.parent.GetChild(0).GetComponent<RectTransform>();
        
    }

    public IEnumerator DelayToClose() {
        Debug.Log("make delay");
        yield return new WaitForSeconds(2f);
        this._exported = true;
    }

    public void OnClick(BaseEventData eventData)
    {
        anim.enabled = false;
        if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Left))
        {
            if (!extended)
            {
                extended = true;
                Std.gameObject.SetActive(false);
                Extended.gameObject.SetActive(true);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x * 2, rect.sizeDelta.y);
                _cnt++;
                SetSrpite();
            }
            else {
                if (_cnt == _max) {
                    return;
                }
                _cnt++;
                
                SetSrpite();
            }
            /*
            if (extended)
            {
                Std.gameObject.SetActive(false);
                Extended.gameObject.SetActive(true);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x * 2, rect.sizeDelta.y);
            }
            else
            {
                Std.gameObject.SetActive(true);
                Extended.gameObject.SetActive(false);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y);
            }*/
        }
    }

    public void OnClickLeftPage(BaseEventData eventData) {
        if (_onDrag) return;

        if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Left))
        {
            if (_cnt == 1)
            {
                return;
            }
            _cnt--;
            SetSrpite();
            transform.SetAsLastSibling();
        }
        else if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Right))
        {
            if (_cnt > 0)
            {

                Close();
            }
        }
    }

    public void OnClickRightPage(BaseEventData eventData)
    {
        if (_onDrag) return;
        if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Left))
        {
            if (_cnt == _max)
            {
                return;
            }
            _cnt++;
            SetSrpite();
            transform.SetAsLastSibling();
        }
        else if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Right)) {
            if (_cnt > 0) {

                Close();
            }
        }

    }

    public void OnClickRight(BaseEventData eventData) {
        if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Right))
        {
            Close();
        }
    }

    private void Close() {
        extended = false;
        Std.gameObject.SetActive(true);
        Extended.gameObject.SetActive(false);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y);
        _cnt = 0;
        SetSrpite();
    }

    void SetSrpite() {
        if (!extended) return;
        left.sprite = ImageList[_cnt*2-1];
        if (_cnt * 2 == ImageList.Length)
        {
            right.sprite = None;
            return;
        }
        right.sprite = ImageList[_cnt*2];
    }

    public void Drag(BaseEventData eventData) {

        gameObject.transform.position = (eventData as PointerEventData).position;
        //피벗 기준으로 옮길 것
    }

    public void BoundaryCheck() {
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        float width, height;
        
        width = parentRect.rect.width / 2;
        height = parentRect.rect.height / 2;

        if (rect.anchoredPosition.x < -width) {
            rect.anchoredPosition = new Vector2(parentRect.anchoredPosition.x - width, rect.anchoredPosition.y);
        }
        else if (rect.anchoredPosition.x > width)
        {
            rect.anchoredPosition = new Vector2(parentRect.anchoredPosition.x + width, rect.anchoredPosition.y);
        }

        if (rect.anchoredPosition.y < -height)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, parentRect.anchoredPosition.y - height);
        }
        else if (rect.anchoredPosition.y > height)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, parentRect.anchoredPosition.y + height);
        }
        
    }

    public void CloseManual() {
        float width, height;

        width = destroyArea.rect.width / 2;
        height = destroyArea.rect.height / 2;

        if ((rect.anchoredPosition.x > (destroyArea.anchoredPosition.x - width)) 
            && (rect.anchoredPosition.x < (destroyArea.anchoredPosition.x + width))){
            if ((rect.anchoredPosition.y > (destroyArea.anchoredPosition.y - height))
                &&(rect.anchoredPosition.y < (destroyArea.anchoredPosition.y + height))){
                    
                    if (_exported) {
                        anim.enabled = true;
                        if (_cnt > 0) {
                            Close();
                        }
                        Debug.Log("Close");
                        _exported = false;
                        anim.SetBool("Play", false);
                        anim.SetBool("Close", true);
                        //StoryMenuScript.instance.CloseManual(this.directory);
                    }
            }
        }
    }

    public void Update() {
        BoundaryCheck();
        CloseManual();
    }
}
