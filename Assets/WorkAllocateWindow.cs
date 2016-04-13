using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorkAllocateWindow : MonoBehaviour {

    public Transform anchor;
    public RectTransform agentScrollTarget;
    public List<RectTransform> skillCategoryTraget;

    public GameObject agentSlot;

    private CreatureModel targetCreature = null;
    private Transform attachedNode = null;
    private WorkType workType;
    private List<WorkAllocateSlot> slotList;
    
    public static WorkAllocateWindow currentWindow;
    bool agentListDisplayed = false;
    Sefira currentSefira;

    [HideInInspector]
    public List<SkillTypeInfo> skillList;
    [HideInInspector]
    public List<AgentModel> agentList;

    float posy = 0f;
    
    public long selectedSkillId;
    public AgentModel selectedAgent;
    
    public delegate void ClickedEvent(long id);

    public static WorkAllocateWindow CreateWindow(CreatureModel creature, WorkType workType){

        if (currentWindow.gameObject.activeSelf)
        {
            //현재 창이 켜져있는 상태 -> 다시 누르면 꺼지게 해야된다?
            // 위의 상황이 맞다면
            // currentWindow.CloseWindow();

            //환상체 교체 필요
            if (creature == currentWindow.targetCreature)
            {
                return currentWindow;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.CloseAgentList();
        }

        if (!creature.sefira.Equals(currentWindow.currentSefira))
        {
            //세피라가 교체된 상태이므로 직원 리스트를 새로 받아온다.
            currentWindow.currentSefira = creature.sefira;
            currentWindow.CloseAgentList();
            currentWindow.ResetAgentList();
            currentWindow.GetAgentList();
        }

        currentWindow.targetCreature = creature;
        currentWindow.workType = workType;

        return currentWindow;    
    }

    //void Awake(){
	void Start(){
        currentWindow = this;
        currentWindow.gameObject.SetActive(false);
        currentWindow.Init();
        //->부르는 시점을 약간 늦춰서 스킬 리스트가 완성되면 부르게 해야될듯;
    }

    public void AddingEventTrigger(RectTransform target, long clickedInfo, ClickedEvent eventMethod) {
        EventTrigger attachedTrigger = target.GetComponent<EventTrigger>();
        if (attachedTrigger == null) {
            attachedTrigger = target.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { eventMethod(clickedInfo); });
        attachedTrigger.triggers.Add(entry);
    }

    public void Init() {
        this.skillList = new List<SkillTypeInfo>();
        this.agentList = new List<AgentModel>();
        this.slotList = new List<WorkAllocateSlot>();
        //스킬 계열 초기화 필요

		this.skillList.Add (SkillTypeList.instance.GetData (1));
		this.skillList.Add (SkillTypeList.instance.GetData (2));
		this.skillList.Add (SkillTypeList.instance.GetData (3));
		this.skillList.Add (SkillTypeList.instance.GetData (4));
		this.skillList.Add (SkillTypeList.instance.GetData (5));

        for (int i = 0; i < this.skillList.Count; i++) {
            if (i > this.skillCategoryTraget.Count) break;
            AddingEventTrigger(skillCategoryTraget[i], skillList[i].id, new ClickedEvent(OnClickSkill));
        }
    }

    public void CloseWindow()
    {
        this.CloseAgentList();
        this.targetCreature = null;
        this.selectedSkillId = -1;
        this.selectedAgent = null;
        this.attachedNode = null;
        this.gameObject.SetActive(false);
    }

    void FixedUpdate(){
        
    }

    void UpdatePosition(){
        
    }
        
    public void ShowAgentList(){
        if (this.agentListDisplayed) return;
        this.agentListDisplayed = true;

        this.agentScrollTarget.gameObject.SetActive(true);
    }

    public void CloseAgentList() {
        this.agentListDisplayed = false;
        this.agentScrollTarget.gameObject.SetActive(false);
    }

    public void ResetAgentList() {
        posy = 0f;
        foreach (RectTransform rect in this.agentScrollTarget) {
            Destroy(rect.gameObject);
        }
    }

    public void GetAgentList() {
        this.agentList.Clear();
        this.slotList.Clear();
        foreach (AgentModel model in this.currentSefira.agentList) {
            this.agentList.Add(model);
            GameObject slot = Instantiate(this.agentSlot);
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            WorkAllocateSlot slotScript = slot.GetComponent<WorkAllocateSlot>();

            slotRect.SetParent(this.agentScrollTarget);
            slotRect.localScale = Vector3.one;

            slotRect.anchoredPosition = new Vector2(0f, - posy);
            posy += slotRect.rect.height;

            AddingEventTrigger(slot.GetComponent<RectTransform>(), model.instanceId, new ClickedEvent(OnClickAgent));
            slotScript.Init(model);
        }
        this.agentScrollTarget.sizeDelta = new Vector2(this.agentScrollTarget.sizeDelta.x, posy);
    }

    /// <summary>
    /// 계열 슬롯을 클릭하였을 때 작동
    /// </summary>
    /// <param name="id"></param>
    public void OnClickSkill(long id) {
        SkillTypeInfo targetSkill = GetSkill(id);
        if (targetSkill == null) {
            Debug.Log("Getting skill was failed");
            return;
        }
        this.selectedSkillId = targetSkill.id;
		Debug.Log ("Click skill : " + selectedSkillId);
        ShowAgentList();
    }

    public SkillTypeInfo GetSkill(long id) {
        SkillTypeInfo output = null;

        foreach (SkillTypeInfo info in this.skillList)
        {
            if (info.id == id) {
                output = info;
                break;
            }
        }

        return output;
    }

    /// <summary>
    /// 직원 슬롯을 눌렀을 때 호출하게 됨 (이벤트 트리거 처리)
    /// </summary>
    /// <param name="id">직원 id, 이벤트 트리거 추가시에 자동으로 할당</param>
    public void OnClickAgent(long id) {
        AgentModel target = GetAgent(id);
        if (target == null) {
            Debug.Log("Getting Agent Failed");
            return;
        }

		SkillTypeInfo skillTypeInfo = SkillTypeList.instance.GetData (selectedSkillId);
		if (targetCreature.manageDelay > 0)
		{
			Debug.Log ("not ready creature.. remain : " + targetCreature.manageDelay);
			return;
		}

		if (target.IsReadyToUseSkill (skillTypeInfo) == false)
		{
			Debug.Log ("not ready skill.. remain : " + target.GetSkillDelay(skillTypeInfo));
			return;
		}

        //Clicked Event

		target.ManageCreature (targetCreature, SkillTypeList.instance.GetData (selectedSkillId));

		CloseWindow ();
    }

    public AgentModel GetAgent(long id) {
        AgentModel output = null;
        foreach (AgentModel model in this.agentList) {
            if (model.instanceId == id) {
                output = model;
                break;
            }
        }

        return output;
    }

    public void SetCoolTimeI() {
        foreach (WorkAllocateSlot slot in this.slotList) {
            slot.SetCoolTime();
        }
    }
}
