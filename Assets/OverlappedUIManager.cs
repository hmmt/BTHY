using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class OverlappedUIManager : MonoBehaviour {
    private static OverlappedUIManager _instance = null;
    public static OverlappedUIManager instance {
        get {
            return _instance;
        }
    }

    public List<OverlappedUIElement> selectedList;
    public int currentSelectedIndex = -1;
    public OverlappedUIElement currentSelected = null;

    int cnt = 0;

    public void Awake() {
        _instance = this;
        selectedList = new List<OverlappedUIElement>();

    }

    public void Register(OverlappedUIElement target) {
        target.SetCurrentIndex(this.cnt);
        this.cnt++;
    }

    public void OnClick(OverlappedUIElement selected, BaseEventData eventData) {

        PointerEventData pData = eventData as PointerEventData;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pData, result);

        List<OverlappedUIElement> tempResult = new List<OverlappedUIElement>();

        foreach (RaycastResult item in result) {
            OverlappedUIElement element = item.gameObject.GetComponent<OverlappedUIElement>();
            if (element == null) continue;
            else {
                if(element.isEnabled)
                    tempResult.Add(element);
            }
        }

        if(selectedList.Count > 0)
        {
			List<OverlappedUIElement> rmList = new List<OverlappedUIElement> ();
            foreach (OverlappedUIElement element in selectedList) { 
                if (!tempResult.Contains(element)){
                    element.hasSelected = false;
					rmList.Add (element);
                    //selectedList.Remove(element);
                }
            }

			foreach (OverlappedUIElement rmE in rmList)
			{
				selectedList.Add (rmE);
			}
        }

        
        foreach (OverlappedUIElement element in tempResult) {
            if (selectedList.Contains(element))
            {
                continue;
            }
            selectedList.Add(element);
        }

        OverlappedUIElement output = null;
        foreach (OverlappedUIElement element in selectedList) {
            if (element.hasSelected == true) continue;
            output = element;
            break;
        }

        if (output == null) {
            foreach (OverlappedUIElement element in selectedList)
            {
                element.hasSelected = false;
            }
            output = selected;
        }

        output.hasSelected = true;
        output.OnSelected();
    }
}
