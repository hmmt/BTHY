using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class WorkListScript : MonoBehaviour {
    /*
    public GameObject item;
    public RectTransform[] parentAry;
    private WorkSlot targetSlot;
    private Sefira currentSefira;

    //public List<SkillTypeInfo> skillList = new List<SkillTypeInfo>();
    
    public void Init()
    {
        currentSefira = SefiraManager.instance.getSefira(SefiraName.Malkut);


		Dictionary<long, SkillTypeInfo> directSkillList = new Dictionary<long, SkillTypeInfo>();
		Dictionary<long, SkillTypeInfo> indirectSkillList = new Dictionary<long, SkillTypeInfo>();
		Dictionary<long, SkillTypeInfo> blockSkillList = new Dictionary<long, SkillTypeInfo>();

		foreach (AgentModel agent in AgentManager.instance.GetAgentList())
		{
			foreach (SkillTypeInfo skill in agent.GetDirectSkillList()) {
				directSkillList [skill.id] = skill;
			}
			foreach (SkillTypeInfo skill in agent.GetIndirectSkillList()) {
				indirectSkillList [skill.id] = skill;
			}
			foreach (SkillTypeInfo skill in agent.GetBlockSkillList()) {
				blockSkillList [skill.id] = skill;
			}
		}

		SlotCall (0, new List<SkillTypeInfo>(directSkillList.Values));
		SlotCall(1, new List<SkillTypeInfo>(indirectSkillList.Values));
		SlotCall(2, new List<SkillTypeInfo>(blockSkillList.Values));
    }

    public void Select(WorkSlot target) {
        targetSlot = target;
    }

    public void ClearSlot(int index) {
		/*
        foreach (RectTransform rect in parentAry[index]) {
            skillList.Remove(rect.GetComponent<SkillTypeInfo>());
            Destroy(rect.gameObject);
        }
        skillList.Clear();
    }

	public void SlotCall(int index, List<SkillTypeInfo> skillList) {
        float posy = 0.0f;
        foreach (SkillTypeInfo s in skillList) {
            GameObject slot = CreateItem(s);
            slot.transform.SetParent(parentAry[index]);
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
    }*/
    public GameObject Item;

    private Sefira currentSefira;
    private WorkSlot targetSlot;

    public List<SkillTypeInfo> skillList;
    public List<Sefira.AgentSkillCategory> categories;
    public RectTransform[] parentAry;

    public void Init() {
        skillList = new List<SkillTypeInfo>();
        currentSefira = SefiraManager.instance.getSefira(SefiraName.Malkut);
        categories = new List<Sefira.AgentSkillCategory>(currentSefira.GetSkillCategories());

        for (int i = 0; i < categories.Count; i++)
        {
            List<SkillTypeInfo> tempSkillList = new List<SkillTypeInfo>();
            SkillCategory cat = categories[i].category;
            
            int currentIndex = GetIndex(cat);
            if (currentIndex == -1) { 
                //not have category
                continue;
            }
            if (categories[i].agentList.Count == 0)
            {
                continue;
            }

            for (int j = 1; j <= categories[i].maxLevel; j++)
            {
                foreach (SkillTypeInfo item in cat.GetByLevel(j)) {
                    tempSkillList.Add(item);
                }
            }
            SlotCall(currentIndex, tempSkillList);
            //SlotCall();
        }
    }

    public int GetIndex(SkillCategory target) {
        int output = -1;
        for (int i = 0; i < parentAry.Length; i++) {
            WorkSlotListScript script = parentAry[i].GetComponent<WorkSlotListScript>();
            if (target.name.Equals(script.CategoryName)) {
                output = i;
                break;
            }
        }
        return output;
    }

    public void Start()
    { 
        
    }

    public void SlotCall(int index, List<SkillTypeInfo> skillList)
    {
        float posy = 0.0f;
        foreach (SkillTypeInfo s in skillList)
        {
            //Debug.Log("slot call " + s.name);
            GameObject slot = CreateItem(s);
            slot.transform.SetParent(parentAry[index]);
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
        GameObject newObj = Instantiate(Item);
        WorkItemScript script = newObj.GetComponent<WorkItemScript>();

        script.SetInfo(skill);
        AddEventTrigger(script.gameObject, script);
        //Add eventTrigger?
        return newObj;
    }

    public void Select(WorkSlot target)
    {
        targetSlot = target;
    }

    /*
    public void ClearSlot(int index) {
		
        foreach (RectTransform rect in parentAry[index]) {
            skillList.Remove(rect.GetComponent<SkillTypeInfo>());
            Destroy(rect.gameObject);
        }
        skillList.Clear();
    }
    */

    private void AddEventTrigger(GameObject target, WorkItemScript script)
    {

        EventTrigger trigger = target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { this.OnClick(script); });
        trigger.triggers.Add(entry);
    }

    public void OnClick(WorkItemScript script)
    {
        targetSlot.SetCurrentSkill(script.info);
        //this.gameObject.SetActive(false);
    }

    public void OnRightClick(BaseEventData eventData)
    {
        PointerEventData data = eventData as PointerEventData;
        if (data.button != PointerEventData.InputButton.Right) return;
        this.targetSlot = null;
        this.GetComponent<WorkInventory>().Close();
        //this.gameObject.SetActive(false);
    }

}
