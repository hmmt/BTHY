using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public interface IOverlapOnclick {
    void OnClick() ;
}

public class OverlappedUIElement : MonoBehaviour {
    public bool isEnabled;
    public bool hasSelected = false;
    public int currentIndex;

    public IOverlapOnclick target;

    public MonoBehaviour scriptTarget;

    public void Start() {
        OverlappedUIManager.instance.Register(this);
        target = scriptTarget as IOverlapOnclick;
    }

    public void SetCurrentIndex(int index) {
        this.currentIndex = index;   
    }

    public int GetCurrentIndex() {
        return this.currentIndex;
    }
    
    public void OnClick(BaseEventData eventData){
        OverlappedUIManager.instance.OnClick(this, eventData);
    }

    public void OnSelected() {
        target.OnClick();
    }

}
