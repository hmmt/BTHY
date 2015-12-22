using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler /*,IPointerClickHandler*/{
    public static GameObject itemDragged;
    public static AgentModel srcObj;
    Vector3 startPos;
    public GameObject moveImage;
    public float scaleRate;
    Transform startParent;
    

   // public Transform briefForParent;//parent설정을 위한 오브젝트 

    void Start() {
        if (scaleRate < 0.0f) {
            scaleRate = 1.0f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent.parent.parent.parent;
        itemDragged = (GameObject)Instantiate(moveImage);
        srcObj = gameObject.transform.parent.GetComponent<AgentSlotScript>().model;
        Debug.Log(srcObj);
        itemDragged.transform.GetChild(0).GetComponent<Image>().sprite = ResourceCache.instance.GetSprite(srcObj.bodyImgSrc);
        itemDragged.transform.GetChild(1).GetComponent<Image>().sprite = ResourceCache.instance.GetSprite(srcObj.hairImgSrc);
        itemDragged.transform.GetChild(2).GetComponent<Image>().sprite = ResourceCache.instance.GetSprite(srcObj.faceImgSrc);
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
    
}
