using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
    public StageUI StageUI;
    
    public void OnDrop(PointerEventData eventData)
    {
        StageUI.SetAgentSefriaButton(DragScript.srcObj);
        Destroy(DragScript.itemDragged);        
    }
}
