using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorkAllocateWindow : MonoBehaviour, IActivatableObject {

    public Transform anchor;
    public RectTransform agentScrollTarget;
    public List<RectTransform> skillCategoryTraget;
	public RectTransform specialSkillTarget;

    public GameObject agentSlot;

    private CreatureModel targetCreature = null;
    private Transform attachedNode = null;
    private WorkType workType;
    public List<WorkAllocateSlot> slotList;
    
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

    public ActivatableObjectPos windowPos = ActivatableObjectPos.ISOLATE;
    public RectTransform eventTriggerTarget;
    bool activatableObjectInitiated = false;
    SkillTypeInfo specialSkill = null;
    
    public delegate void ClickedEvent(long id);

    public static WorkAllocateWindow CreateWindow(CreatureModel creature, WorkType workType){
        if (currentWindow.gameObject.activeSelf)
        {

            if (currentWindow.targetCreature == creature)
            {
                return currentWindow;
            }
            else {
                currentWindow.targetCreature = creature;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.targetCreature = creature;
            currentWindow.Activate();
            currentWindow.CloseAgentList();
            
        }

        if (currentWindow.targetCreature.script != null
            && currentWindow.targetCreature.script.GetSpecialSkill() != null)
        {
            currentWindow.specialSkillTarget.gameObject.SetActive(true);
            currentWindow.specialSkill = currentWindow.targetCreature.script.GetSpecialSkill();
            currentWindow.skillList.Add(currentWindow.specialSkill);
            currentWindow.InitSpecialSkillEventTrigger(currentWindow.specialSkill.id);
        }
        else
        {
            currentWindow.specialSkillTarget.gameObject.SetActive(false);
        }

        if (!creature.sefira.Equals(currentWindow.currentSefira))
        {
            //세피라가 교체된 상태이므로 직원 리스트를 새로 받아온다.
            currentWindow.currentSefira = creature.sefira;
            currentWindow.CloseAgentList();
            currentWindow.ResetAgentList();
            currentWindow.GetAgentList();
        }

        currentWindow.workType = workType;

        Canvas canvas = currentWindow.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        return currentWindow;    
    }

    //void Awake(){
	void Start(){
        currentWindow = this;
        currentWindow.gameObject.SetActive(false);

        if (currentWindow.activatableObjectInitiated == false)
        {
            currentWindow.UIActivateInit();
        }

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

	public void InitSpecialSkillEventTrigger(long clickedInfo)
	{
		RectTransform target = specialSkillTarget;
		ClickedEvent eventMethod = new ClickedEvent (OnClickSkill);

		EventTrigger attachedTrigger = target.GetComponent<EventTrigger>();
		if (attachedTrigger == null) {
			attachedTrigger = target.gameObject.AddComponent<EventTrigger>();
		}

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.RemoveAllListeners ();
		entry.callback.AddListener((eventData) => { eventMethod(clickedInfo); });
		attachedTrigger.triggers.Clear ();
		attachedTrigger.triggers.Add(entry);
	}

    public void Init() {
        this.skillList = new List<SkillTypeInfo>();
        this.agentList = new List<AgentModel>();
        //스킬 계열 초기화 필요

		this.skillList.Add (SkillTypeList.instance.GetData (1));
		this.skillList.Add (SkillTypeList.instance.GetData (2));
		this.skillList.Add (SkillTypeList.instance.GetData (3));
		this.skillList.Add (SkillTypeList.instance.GetData (4));
		this.skillList.Add (SkillTypeList.instance.GetData (5));
		this.skillList.Add (null);

        for (int i = 0; i < this.skillList.Count; i++) {
            if (i > this.skillCategoryTraget.Count) break;
			if (skillList [i] == null)
				continue;
            AddingEventTrigger(skillCategoryTraget[i], skillList[i].id, new ClickedEvent(OnClickSkill));
        }
    }

    public void CloseWindow()
    {
        Deactivate();
        this.CloseAgentList();
        this.targetCreature = null;
        this.selectedSkillId = -1;
        this.selectedAgent = null;
        this.attachedNode = null;
        if (this.specialSkill != null) {
            this.skillList.Remove(this.specialSkill);
            this.specialSkill = null;
        }
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
        foreach (WorkAllocateSlot slot in this.slotList) {
            slot.Reset();
        }
    }

    public void GetAgentList() {
        this.agentList.Clear();
        int i = 0;
        foreach (AgentModel model in this.currentSefira.agentList) {
            this.agentList.Add(model);
            this.slotList[i].SetModel(model);
            i++;
        }

        /*
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
         */
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
            if (info == null) {

                continue;
            }
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

    public void Activate()
    {
		if (targetCreature.script != null && targetCreature.script.GetSpecialSkill () != null)
		{
			specialSkillTarget.gameObject.SetActive (true);
			InitSpecialSkillEventTrigger (targetCreature.script.GetSpecialSkill ().id);
			skillList [5] = targetCreature.script.GetSpecialSkill ();
		}
		else
		{
			specialSkillTarget.gameObject.SetActive (false);
		}

        UIActivateManager.instance.Activate(this, this.windowPos);
    }

    public void Deactivate()
    {
        UIActivateManager.instance.Deactivate(this.windowPos);
    }

    public void OnEnter()
    {
        UIActivateManager.instance.OnEnter(this);
    }

    public void OnExit()
    {
        UIActivateManager.instance.OnExit();
    }

    public void UIActivateInit()
    {
        activatableObjectInitiated = true;
        EventTrigger eventTrigger = this.eventTriggerTarget.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = this.eventTriggerTarget.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enter = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();

        enter.eventID = EventTriggerType.PointerEnter;
        exit.eventID = EventTriggerType.PointerExit;

        enter.callback.AddListener((eventData) => { OnEnter(); });
        exit.callback.AddListener((eventData) => { OnExit(); });

        eventTrigger.triggers.Add(enter);
        eventTrigger.triggers.Add(exit);
    }

    public void Close()
    {
        this.CloseWindow();
    }
}
