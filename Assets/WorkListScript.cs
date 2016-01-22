using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class WorkListScript : MonoBehaviour {
    public GameObject item;
    public RectTransform[] parentAry;
    private WorkSlot targetSlot;
    private Sefira currentSefira;

    public List<SkillTypeInfo> skillList = new List<SkillTypeInfo>();
    
    public void Init()
    {
        currentSefira = SefiraManager.instance.getSefira(SefiraName.Malkut);
        SkillTypeInfo[] tempitem = SkillTypeList.instance.GetList();
        skillList.Add(tempitem[0]);
        skillList.Add(tempitem[1]);
        skillList.Add(tempitem[2]);
        skillList.Add(tempitem[3]);
        SlotCall(0);
    }

    public void Select(WorkSlot target) {
        targetSlot = target;
    }

    public void ClearSlot(int index) {
        foreach (RectTransform rect in parentAry[index]) {
            skillList.Remove(rect.GetComponent<SkillTypeInfo>());
            Destroy(rect.gameObject);
        }
        skillList.Clear();
    }

    public void SlotCall(int index) {
        float posy = 0.0f;
        foreach (SkillTypeInfo s in skillList) {
            GameObject slot = CreateItem(s);
            slot.transform.SetParent(parentAry[0]);
            RectTransform rect = slot.GetComponent<RectTransform>();
            float size = rect.sizeDelta.y;
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            rect.anchoredPosition = new Vector2(0.0f, posy);
            posy -= size;

        }
        parentAry[index].sizeDelta = new Vector2(parentAry[index].sizeDelta.x, -posy);
    }

    public GameObject CreateItem(SkillTypeInfo skill)
    {
        GameObject newObj = Instantiate(item);
        WorkItemScript script = newObj.GetComponent<WorkItemScript>();

        script.SetInfo(skill);
        AddEventTrigger(script.GetIconObject(), script);
        //Add eventTrigger?
        return newObj;
    }

    private void AddEventTrigger(GameObject target, WorkItemScript script) {

        EventTrigger trigger = target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { this.OnClick(script); });
        trigger.triggers.Add(entry);
    }

    public void OnClick(WorkItemScript script) {
        targetSlot.SetCurrentSkill(script.info);
        //this.gameObject.SetActive(false);
    }

    public void OnRightClick(BaseEventData eventData) {
        PointerEventData data = eventData as PointerEventData;
        if (data.button != PointerEventData.InputButton.Right) return;
        this.targetSlot = null;
        this.GetComponent<WorkInventory>().Close();
        //this.gameObject.SetActive(false);
    }
}
