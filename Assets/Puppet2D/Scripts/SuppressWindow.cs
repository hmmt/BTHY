using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SuppressAction {
    public enum Weapon { 
        NONE,
        STICK,
        GUN
    }

	public AgentModel model;
    public Weapon weapon;

    public SuppressAction(AgentModel target) {
        this.model = target;
        this.weapon = Weapon.NONE;
    }

    /*
        정렬용 비교 함수 구역
     */
    public static int CompareByName(SuppressAction a, SuppressAction b){
        return AgentModel.CompareByName(a.model, b.model);
    }

    public static int CompareByLevel(SuppressAction a, SuppressAction b) {
        return AgentModel.CompareByLevel(a.model, b.model);
    }

    public static int CompareByLifestyle(SuppressAction a, SuppressAction b) {
        return AgentModel.CompareByLifestyle(a.model, b.model);
    }
    //정렬용 비교함수 끝
}

public class SuppressWindow : MonoBehaviour, IActivatableObject
{
    public enum TargetType { 
        CREATURE,
        AGENT,
		OFFICER
    }

    [System.Serializable]
    public class SuppressUIAgent :SuppressUI{
        public Slider Health;
        public Text Name;
        public Text Grade;

        public Image face;
        public Image hair;
        
        AgentModel model;

        public override void Init(object target)
        {
            if (!(target is AgentModel)) {
                Debug.Log("Error");
                return;
            }
            base.Init(target);
            this.model = target as AgentModel;

            Health.maxValue = this.model.defaultMaxHp;
            Health.value = this.model.hp;
            this.Name.text = this.model.name;
            this.Grade.text = AgentModel.GetLevelGradeText(this.model);

            AgentModel.SetPortraitSprite(this.model, this.face, this.hair);

        }

        public override void ChangeValue()
        {
            Health.value = this.model.hp;
        }
    }

    [System.Serializable]
    public class SuppressUICreature :SuppressUI{
        public Image Portrait;
        public Text Name;
        public Text Fear;
        public Text Phyisc;
        public Text Mental;

        CreatureModel model;

        public override void Init(object target)
        {
            if (!(target is CreatureModel)) {
                Debug.Log("Error");
                return;
            }
            base.Init(target);
            this.model = target as CreatureModel;
            this.Name.text = model.metaInfo.name;
            this.Fear.text = model.metaInfo.level;
            this.Phyisc.text = model.metaInfo.physicalAttackLevel.ToString();
            this.Mental.text = model.metaInfo.mentalAttackLevel.ToString();
        }

        public override void ChangeValue()
        {
            return;
        }
    }

    public class SuppressUI {
        public GameObject thisObject;
        public virtual void Init(object target) {
            thisObject.gameObject.SetActive(true);
        }

        public virtual void ChangeValue() { 
            
        }
    }

    public GameObject slot;
    
    [System.Serializable]
    public class SuppressWindowUI {
        private TargetType type;
        private object target;
        public Image Portrait;//may be need distribution for agent or creature
        public Text name;
        public Text grade;

        public Text fear;
        public Text physical;
        public Text mental;

        public Text currentHealth;
        public Text currentMental;
        public Text movementSpeed;

        public SuppressUI currentUi;

        public SuppressUIAgent agentUi;
        public SuppressUICreature creatureUi;
        

        public void Init(object target, TargetType type) {
            this.type = type;
            this.target = target;
            switch(type){
                case TargetType.CREATURE:
                    {
                        CreatureModel model = target as CreatureModel;
                        this.currentUi = creatureUi;
                        agentUi.thisObject.SetActive(false);
                        this.currentUi.Init(target);
                        break;
                    }
                case TargetType.AGENT:
                    {
                        AgentModel model = target as AgentModel;
                        this.currentUi = agentUi;
                        creatureUi.thisObject.SetActive(false);
                        this.currentUi.Init(target);
                    }
                    break;
				case TargetType.OFFICER:
					{
						OfficerModel model = target as OfficerModel;
						this.currentUi = agentUi;
						creatureUi.thisObject.SetActive(false);
						this.currentUi.Init(target);
					}
					break;
            }
        }

        public void Update() {
            //Change 적용
        }

    }

    public RectTransform AgentScrollTarget;
    public RectTransform anchor;
    public SuppressWindowUI ui;

    public List<SuppressAction> agentList;//현제 세피라에 배치되었으며, 패닉상태가 아닌 직원들
    public List<SuppressAction> suppressingAgentList;//실제 제압을 하게되는 직원들의 리스트

    //Sort 에 관련된 UI 및 데이터 필요

    [HideInInspector]
    public ActivatableObjectPos windowPos = ActivatableObjectPos.LEFTUPPER;

    public RectTransform eventTriggerTarget;
    bool activatableObjectInitiated = false;

    private object target;

    public object Target {
        get { return GetTarget(); }
    }

    private TargetType targetType;
    private Transform attachedPos;
    private Sefira currentSefira;

    public static SuppressWindow currentWindow = null;
    
    public static SuppressWindow CreateWindow(CreatureModel target) {
        if (currentWindow.gameObject.activeSelf)
        {
            if (currentWindow.target == target)
            {
                return currentWindow;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.Activate();
        }
        
        SuppressWindow inst = currentWindow;
        inst.target = target;
        inst.targetType = TargetType.CREATURE;
        inst.currentSefira = target.sefira;

        inst.agentList.Clear();

        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(target.instanceId);
        inst.attachedPos = unit.transform;
        inst.ui.Init(target, inst.targetType);

        if (inst.activatableObjectInitiated == false) {
            inst.UIActivateInit();
        }

        inst.InitAgentList();
        inst.ShowAgentList();
        Canvas canvas = inst.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        currentWindow = inst;

        return inst;
    }

    public static SuppressWindow CreateWindow(AgentModel target)
    {
        if (currentWindow.gameObject.activeSelf)
        {
            if (currentWindow.target == target)
            {
                return currentWindow;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.Activate();
        }

        SuppressWindow inst = currentWindow;
        inst.target = target;
        inst.targetType = TargetType.AGENT;
		inst.currentSefira = SefiraManager.instance.getSefira(target.currentSefira);
		//inst.currentSefira = SefiraManager.instance.getSefira("1");
        inst.agentList.Clear();

        AgentUnit unit = AgentLayer.currentLayer.GetAgent(target.instanceId);
        inst.attachedPos = unit.transform;
        inst.ui.Init(target, inst.targetType);

        inst.InitAgentList();
        inst.ShowAgentList();

        Canvas canvas = inst.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        currentWindow = inst;
        return inst;
    }

	public static SuppressWindow CreateWindow(OfficerModel target)
	{
        if (currentWindow.gameObject.activeSelf)
        {
            if (currentWindow.target == target)
            {
                return currentWindow;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.Activate();
        }


        SuppressWindow inst = currentWindow;
		inst.target = target;
		inst.targetType = TargetType.OFFICER;
		inst.currentSefira = SefiraManager.instance.getSefira(target.currentSefira);
		//inst.currentSefira = SefiraManager.instance.getSefira("1");
        inst.agentList.Clear();

		OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer(target.instanceId);
		inst.attachedPos = unit.transform;
		inst.ui.Init(target, inst.targetType);

		inst.InitAgentList();
		inst.ShowAgentList();

        Canvas canvas = inst.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

		currentWindow = inst;
		return inst;
	}

    public void Start()
    {
        currentWindow = this;
        currentWindow.agentList = new List<SuppressAction>();
        currentWindow.gameObject.SetActive(false);
    }

    private object GetTarget() {
        if (currentWindow.targetType == TargetType.CREATURE)
        {
            return currentWindow.target as CreatureModel;
        }
        else if (currentWindow.targetType == TargetType.AGENT)
        {
            return currentWindow.target as AgentModel;
        }
		else if (currentWindow.targetType == TargetType.OFFICER)
		{
			return currentWindow.target as OfficerModel;
		}
        else {
            return null;
        }
    }

    private void InitAgentList() {
        List<AgentModel> list = new List<AgentModel>(currentSefira.agentList.ToArray());

        foreach (AgentModel model in list) {
            //패닉 상태인지 확인하는 과정이 필요함
			if (model.panicFlag != true && model.GetState() != AgentAIState.CANNOT_CONTROLL) {
                SuppressAction action = new SuppressAction(model);
                this.agentList.Add(action);
            }
        }
    }

    public void OnClickClose() { 
        //CloseWindow;

        CloseWindow();
    }

    public void ShowAgentList() { 
        //패닉상태가 아닌 직원들의 리스트가 필요
        //정렬 기능을 구현해야 함
        float posy = 0;
        foreach (Transform child in AgentScrollTarget) {
            Destroy(child.gameObject);
        }
        AgentScrollTarget.sizeDelta = new Vector2(AgentScrollTarget.sizeDelta.x, 0f);

        foreach (SuppressAction agent in this.agentList) {
            GameObject newObj = Instantiate(slot);
            RectTransform rect = newObj.GetComponent<RectTransform>();
            SuppressAgentSlot script = newObj.GetComponent<SuppressAgentSlot>();

            rect.SetParent(this.AgentScrollTarget);
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;

            rect.anchoredPosition = new Vector2(0f, -posy);
            
            script.Init(agent.model);

            posy += script.GetHeight();
        }

        AgentScrollTarget.sizeDelta = new Vector2(AgentScrollTarget.sizeDelta.x, posy);
    }

	public void OnSetSuppression(AgentModel actor)
	{
		
		if(target is AgentModel)
		{
			SuppressAction sa = new SuppressAction (actor);
			sa.weapon = SuppressAction.Weapon.GUN;

			actor.StartSuppressAgent((AgentModel)target, sa, SuppressType.UNCONTROLLABLE);
		}
		else if(target is OfficerModel)
		{
			SuppressAction sa = new SuppressAction (actor);
			sa.weapon = SuppressAction.Weapon.GUN;

			actor.StartSuppressAgent((OfficerModel)target, sa, SuppressType.UNCONTROLLABLE);
		}
		else if(target is CreatureModel)
		{
			SuppressAction sa = new SuppressAction (actor);
			sa.weapon = SuppressAction.Weapon.GUN;

			actor.SuppressCreature((CreatureModel)target, sa);
		}

		/*
		SuppressAction sa = new SuppressAction (actor);
		sa.weapon = SuppressAction.Weapon.GUN;
		AutoCommandManager.instance.SetSuppressAgent(target, sa);
		*/
	}

    public void CloseWindow() {
        Deactivate();
        currentWindow.gameObject.SetActive(false);//ANimations?
    }

    /// <summary>
    /// call agent list who operate suppress actions.
    /// </summary>
    /// <returns></returns>
    public List<SuppressAction> GetSuppressingAgentList()
    {
        this.suppressingAgentList = new List<SuppressAction>();

        foreach (SuppressAction action in this.agentList) {
            if (action.weapon != SuppressAction.Weapon.NONE) {
                suppressingAgentList.Add(action);
            }
        }

        return this.suppressingAgentList;
    }

    public void Activate()
    {
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
