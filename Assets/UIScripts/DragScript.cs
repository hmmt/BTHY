using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemDragged;
    Vector3 startPos;
    public GameObject moveImage;
    Transform startParent;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent;
        //itemDragged = gameObject;
        itemDragged = (GameObject)Instantiate(moveImage);
        itemDragged.transform.SetParent(transform.parent);
        startPos = transform.position;
        itemDragged.transform.position = startPos;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemDragged.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //itemDragged = null;
        //itemDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (itemDragged.transform.parent == startParent) {
            Destroy(itemDragged);
        }
       
        //itemDragged = null;
        //transform.position = startPos;
    }
}
