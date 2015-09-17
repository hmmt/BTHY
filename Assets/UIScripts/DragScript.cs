using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemDragged;
    public static AgentModel srcObj;
    Vector3 startPos;
    public GameObject moveImage;
    public float scaleRate;
    Transform tempParent;
    Transform startParent;
    

   // public Transform briefForParent;//parent설정을 위한 오브젝트 
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        //AgentModelScript = transform.GetComponent<AgentSlotPanelStage>();
        //startParent = transform.parent.parent.parent;
        startParent = transform.parent.parent.parent;
        //itemDragged = gameObject;
        tempParent = GameObject.FindGameObjectWithTag("DummyPanel").transform;
        itemDragged = (GameObject)Instantiate(moveImage);
        srcObj = gameObject.GetComponent<AgentSlotPanelStage>().model;
        Debug.Log(gameObject.GetComponent<AgentSlotPanelStage>().model);
        //Debug.Log(gameObject.GetComponent<AgentSlotPanelStage>().model);
        itemDragged.transform.localScale *= scaleRate;
        itemDragged.transform.SetParent(startParent);
        startPos = transform.position;
        itemDragged.transform.position = startPos;
        itemDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //model = transform.GetComponent<AgentSlotPanelStage>().model;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemDragged.transform.position = Input.mousePosition;
        //Debug.Log(itemDragged.transform.parent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log(itemDragged.transform.parent);
        itemDragged.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(itemDragged);

        //Destroy(itemDragged);
        //itemDragged = null;
        //transform.position = startPos;
    }
}
