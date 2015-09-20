using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler{
    public static GameObject itemDragged;
    public static AgentModel srcObj;
    Vector3 startPos;
    public GameObject moveImage;
    public float scaleRate;
    Transform tempParent;
    Transform startParent;
    

   // public Transform briefForParent;//parent설정을 위한 오브젝트 

    void Start() {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        startParent = transform.parent.parent.parent;
        tempParent = GameObject.FindGameObjectWithTag("DummyPanel").transform;
        itemDragged = (GameObject)Instantiate(moveImage);
        srcObj = gameObject.GetComponent<AgentSlotPanelStage>().model;

        itemDragged.transform.localScale *= scaleRate;
        itemDragged.transform.SetParent(startParent);
        startPos = transform.position;
        itemDragged.transform.position = startPos;
        itemDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemDragged.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemDragged.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(itemDragged);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject infoslot = GameObject.FindWithTag("InfoSlotPanel");
        infoslot.GetComponent<InfoSlotScript>().SelectedAgent(gameObject);
    }
}
