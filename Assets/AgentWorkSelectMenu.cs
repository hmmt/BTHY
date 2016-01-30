using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AgentWorkSelectMenu : MonoBehaviour {
    public MenuScript menuScript;
    //public ScrollRect scroll;
    public RectTransform Notselected;
    public RectTransform Selected;
    

    public void Awake() {
        //OnClick();
    }

    public void Start() {
        Notselected.gameObject.SetActive(true);
        Selected.gameObject.SetActive(false);
    }
    /*
    public void OnClick() {
        RectTransform rect = menuScript.GetSelectedRect();
        scroll.content = rect;
    }
    */

    private void OnClick() {
        Notselected.gameObject.SetActive(false);
        Selected.gameObject.SetActive(true);
    }

    public void Revert() {
        Notselected.gameObject.SetActive(true);
        Selected.gameObject.SetActive(false);
    }

    public void OnClick(BaseEventData eventData) {
        if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Right)) {
            Revert();
        }
        else if ((eventData as PointerEventData).button.Equals(PointerEventData.InputButton.Left))
        {
            OnClick();
        }
    }
}
