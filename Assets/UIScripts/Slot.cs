using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
    public StageUI StageUI;
    
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("Found");
        //Debug.Log(DragScript.srcObj);
        StageUI.SetAgentSefriaButton(DragScript.srcObj);
        Destroy(DragScript.itemDragged);
        //DragScript.itemDragged.transform.SetParent(transform);
       //  DragScript.itemDragged
        
    }
}
