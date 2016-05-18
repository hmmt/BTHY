using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorkAllocateWindow : MonoBehaviour, IActivatableObject {
    [System.Serializable]
    public class WorkAlloateUI {
        public Sprite bg;
        public Sprite button_Normal;
        public Sprite button_Over;
        public Sprite button_Click;

        public Sprite observeBg;
        public Sprite observeButton_Normal;
        public Sprite observeButton_Over;
        public Sprite observeButton_Click;
    }

    [System.Serializable]
    public class WorkAllocateTarget {
        public Image bg;
        public Button button;


        WorkAlloateUI current;

        public void SetSprite(WorkAlloateUI ui){
            current = ui;
            SetWork();
        }

        public void SetObserve() {
            bg.sprite = current.observeBg;
            button.image.sprite = current.observeButton_Normal;
            SpriteState state = button.spriteState;
            state.highlightedSprite = current.observeButton_Over;
            state.disabledSprite = current.observeButton_Normal;
            state.pressedSprite = current.observeButton_Click;
            button.spriteState = state;
        }

        public void SetWork() {
            bg.sprite = current.bg;
            button.image.sprite = current.button_Normal;
            SpriteState state = button.spriteState;
            state.highlightedSprite = current.button_Over;
            state.disabledSprite = current.button_Normal;
            state.pressedSprite = current.button_Click;
            button.spriteState = state;
        }
    }

    public Transform anchor;
    public RectTransform agentScrollTarget;
    public List<RectTransform> skillCategoryTraget;
	public RectTransform specialSkillTarget;

    public GameObject agentSlot;

    private CreatureModel targetCreature = null;
    private Transform attachedNode = null;
    private WorkType workType;
    public List<WorkAllocateSlot> slotList;
    public List<AgentModel> observingAgent = new List<AgentModel>();
    
    public static WorkAllocateWindow currentWindow;
    bool agentListDisplayed = false;
    Sefira currentSefira = null;

    [HideInInspector]
    public List<SkillTypeInfo> skillList;
    [HideInInspector]
    public List<AgentModel> agentList;

    float posy = 0f;

    public WorkAlloateUI malkuth;
    public WorkAlloateUI yesod;

    public WorkAllocateTarget uiTarget;

    public long selectedSkillId = -1;
    AgentModel selectedAgent;

    public ActivatableObjectPos windowPos = ActivatableObjectPos.ISOLATE;
    public RectTransform eventTriggerTarget;

	public Button observeButton;
    public bool observeState = false;//이 창이 관찰창이 되는가에 대한 정보
    bool activatableObjectInitiated = false;
    SkillTypeInfo specialSkill = null;

    public Sprite currentIcon = null;   

    //public SelectObserveAgentWindow observeWindow;
    
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
        currentWindow.selectedAgent = null;
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

            currentWindow.Change(currentWindow.currentSefira);
        }
        currentWindow.ShowAgentList();
        currentWindow.observeState = false;
        currentWindow.workType = workType;

		// activate observe button
		if (currentWindow.targetCreature.CanObserve ())
			currentWindow.observeButton.gameObject.SetActive (true);
		else
			currentWindow.observeButton.gameObject.SetActive (false);

        Canvas canvas = currentWindow.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        return currentWindow;    
    }

    public void Change(Sefira sefira) {
        if (sefira.name == SefiraName.Malkut) {
            this.uiTarget.SetSprite(this.malkuth);
        }
        else if (sefira.name == SefiraName.Yesod)
        {
            this.uiTarget.SetSprite(this.yesod);
        }
        else {
            this.uiTarget.SetSprite(this.malkuth);
        }
    }

    public CreatureModel GetTargetCreature() {
        return this.targetCreature;
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

        if (CheckIdValidate(this.selectedSkillId))
        {
            ChangeWorkIcon(this.selectedSkillId, ManageWorkIconState.DEFAULT);
        }
        this.CloseAgentList();
        this.targetCreature = null;
        this.selectedSkillId = -1;
        this.selectedAgent = null;
        this.attachedNode = null;
        if (this.specialSkill != null) {
            this.skillList.Remove(this.specialSkill);
            this.specialSkill = null;
        }

        foreach (WorkAllocateSlot slot in this.slotList) {
            slot.Release();
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

        foreach (WorkAllocateSlot slot in slotList)
        {
            slot.SetIconByWindow(null, 10000);
        }

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

        for (; i < 5; i++) {
            this.slotList[i].SetModel(null);
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

    public Sprite ChangeWorkIcon(long id, ManageWorkIconState state) {
        int index = AgentModel.GetWorkIconId(SkillTypeList.instance.GetData(id));
        Sprite s = IconManager.instance.GetWorkIcon(index).GetIcon(state).icon;
        Sprite output = IconManager.instance.GetWorkIcon(index).GetDefault().icon;
        Image target = null;
        if (id > 5)
        {
            target = specialSkillTarget.GetChild(0).GetComponent<Image>();

        }
        else
        {
            Debug.Log((id - 1).ToString());
            target = this.skillCategoryTraget[(int)id - 1].GetChild(0).GetComponent<Image>();
        }
        target.sprite = s;
        return output;
    }

    public bool CheckIdValidate(long id) {
        foreach (SkillTypeInfo skill in this.skillList) {
            if (skill == null) continue;
            if (skill.id == id) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 계열 슬롯을 클릭하였을 때 작동
    /// </summary>
    /// <param name="id"></param>
    public void OnClickSkill(long id)
    {
        //temporary
        if (CheckIdValidate(this.selectedSkillId)) {
            Debug.Log(this.selectedSkillId);
            ChangeWorkIcon(this.selectedSkillId , ManageWorkIconState.DEFAULT);
        }

        
        if (this.selectedAgent != null)
        {
            foreach (WorkAllocateSlot slot in this.slotList)
            {
                if (slot.model == this.selectedAgent)
                {
                    slot.Release();
                    break;
                }
            }
        }

        if (this.selectedSkillId == id)
        {
            ChangeWorkIcon(this.selectedSkillId, ManageWorkIconState.DEFAULT);
            this.selectedSkillId = -1;
            return;
        }


        SkillTypeInfo targetSkill = GetSkill(id);
        if (targetSkill == null) {
            Debug.Log("Getting skill was failed");
            return;
        }
        this.selectedSkillId = targetSkill.id;
		Debug.Log ("Click skill : " + selectedSkillId);
        if (this.observeState) {
            this.CloseObserveWindow();
        }

        Sprite icon = ChangeWorkIcon(this.selectedSkillId, ManageWorkIconState.COLOR);

        foreach (WorkAllocateSlot slot in this.slotList) {
            slot.SetIconByWindow(icon, this.selectedSkillId);
        }
        //ShowAgentList();
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

    public void OnSelectAgent(AgentModel model) {
		/*
        if (this.observeState) {
            this.observingAgent.Add(model);
            return;
        }
        */
        if (this.selectedAgent != null) {
            foreach (WorkAllocateSlot slot in this.slotList) {
                if (slot.model == this.selectedAgent) {
                    slot.Release();
                    break;
                }
            }
        }
        this.selectedAgent = model;
    }

    public void OnDeselectAgent(AgentModel model) {
        if (this.observeState) {
            this.observingAgent.Remove(model);
            return;
        }

        if (this.selectedAgent == model) {

            this.selectedAgent = null;
        }
    }

    public void OnClickStartButton() {
        if (this.selectedAgent == null) return;

        if (this.observeState) { 
            //관찰 시작
            Debug.Log("관찰을 시작한다");
			selectedAgent.ObserveCreature (targetCreature);
            CloseWindow();
            return;
        }

        SkillTypeInfo skillTypeInfo = SkillTypeList.instance.GetData(selectedSkillId);
        if (targetCreature.manageDelay > 0)
        {
            Debug.Log("not ready creature.. remain : " + targetCreature.manageDelay);
            return;
        }

        if (selectedAgent.IsReadyToUseSkill(skillTypeInfo) == false)
        {
            Debug.Log("not ready skill.. remain : " + selectedAgent.GetSkillDelay(skillTypeInfo));
            return;
        }

        //Clicked Event
        selectedAgent.ManageCreature(targetCreature, SkillTypeList.instance.GetData(selectedSkillId));

        CloseWindow();
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

    public void OpenObserveWindow() {
		if (targetCreature.CanObserve () == false) {
			int point = targetCreature.GetObservationConditionPoint ();
			Debug.Log ("point : " + point);
			return;
		}

        if (observeState) {
            CloseObserveWindow();
            return;
        }
        if (CheckIdValidate(this.selectedSkillId)) {
            ChangeWorkIcon(this.selectedSkillId, ManageWorkIconState.DEFAULT);
        }

        foreach (WorkAllocateSlot slot in slotList) {
            slot.SetIconByWindow(null, 10000);
        }

        observingAgent.Clear();
        observeState = true;    
        this.uiTarget.SetObserve();
        //ShowAgentList();
    }

    public void CloseObserveWindow() {
        observeState = false;
        observingAgent.Clear();
        this.uiTarget.SetWork();
    }
}
