using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectObserveAgentWindow : MonoBehaviour, IActivatableObject
{
    [System.Serializable]
    public class ObserveWindowUI {
        public Image portrait;
        public Text name;
        public Text grade;
        public Text physical;
        public Text mental;

        int currentLevel;
        CreatureTypeInfo info;
        CreatureTypeInfo.ObserveTable table;

        public void Init(CreatureModel model) {
            this.info = model.metaInfo;
            this.table = model.metaInfo.observeTable;
            this.currentLevel = model.metaInfo.CurrentObserveLevel;

            if (currentLevel >= table.portrait)
            {
                this.portrait.sprite = ResourceCache.instance.GetSprite("Sprites/" + info.portraitSrc);
            }
            else {
                this.portrait.sprite = ResourceCache.instance.GetSprite("Sprites/Unit/creature/dummy");
            }


            DisplayText(this.name, table.name, info.name);
            DisplayText(this.grade, table.riskLevel, info.level);
            DisplayText(this.physical, table.physical, info.physicalAttackLevel.ToString());
            DisplayText(this.mental, table.mental, info.mentalAttackLevel.ToString());

        }

        public void DisplayText(Text target, int level, string desc) {
            if (currentLevel >= level)
            {
                target.text = desc;
            }
            else {
                target.text = "Unknown";
            }
        }
    }

    [System.Serializable]
    public class ConditionUI {
        public Text NeedOfficer;
        public Text NeedAgent;
        public Text ExpectSuccesPercent;

        public void Init(SelectObserveAgentWindow window) {
            this.NeedAgent.text = window.needAgentCnt.ToString();
            this.NeedOfficer.text = window.needOfficerCnt.ToString();
            this.ExpectSuccesPercent.text = window.expectSuccessPercent.ToString();
        }
    }
    
    public Transform agentScrollTarget;
    
    public ObserveWindowUI ui;
    public ConditionUI conditionUI;

    [HideInInspector]
    public ActivatableObjectPos windowPos = ActivatableObjectPos.LEFTUPPER;

    public RectTransform eventTriggerTarget;
    bool activatableObjectInitiated = false;

    private int state1 = 0;
    private CreatureModel targetCreature = null;
    List<GameObject> selectedAgentList = new List<GameObject>();

    private int needOfficerCnt;
    private int needAgentCnt;
    private float expectSuccessPercent;

    private int currentOfficerCnt;
    private int currentAgentCnt;

    private List<OfficerModel> officerList;
    private List<AgentModel> agentList;
    private List<AgentModel> sefiraAgent;//agentList for sefria display. may not need
    
    public static SelectObserveAgentWindow currentWindow = null;

    public static SelectObserveAgentWindow CreateWindow(CreatureModel unit)
    {
        if (currentWindow.gameObject.activeSelf)
        {
            if (currentWindow.targetCreature == unit)
            {
                return currentWindow;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.Activate();
        }
        
        SelectObserveAgentWindow inst = currentWindow;
        
        inst.targetCreature = unit;
        //Initialize attributes
        inst.currentAgentCnt = inst.currentOfficerCnt = 0;
        inst.expectSuccessPercent = 0f;
        inst.needAgentCnt = 1;
        inst.needOfficerCnt = 2;

        inst.officerList.Clear();
        inst.agentList.Clear();
        inst.GetRandomOfficer(inst.needOfficerCnt);

        //Initialize UI
        inst.ui.Init(inst.targetCreature);
        inst.conditionUI.Init(inst);
        

        Canvas canvas = currentWindow.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        //make AgentSlot
        inst.ShowAgentList();
        currentWindow = inst;

        return inst;
    }

    public void Start() {
        currentWindow = this;
        currentWindow.officerList = new List<OfficerModel>();
        currentWindow.agentList = new List<AgentModel>();

        if (currentWindow.activatableObjectInitiated == false)
        {
            currentWindow.UIActivateInit();
        }
        currentWindow.gameObject.SetActive(false);
    }

    public void GetRandomOfficer(int cnt) {
        int maxOfficerCnt = targetCreature.sefira.GetOfficerCount();
        if (maxOfficerCnt < cnt) return;
        int value = cnt;
        while (value > 0) {
            OfficerModel model = targetCreature.sefira.GetOfficerByRandom();
            if (this.officerList.Contains(model)) {
                continue;
            }
            this.officerList.Add(model);
            value--;
        }
    }

    public void OnClickAgentOK()
    {

    }

    public void OnClickClose()
    {
        CloseWindow();
    }

    public void SelectAgentSkill(AgentModel agent)
    {
        //ObserveCreature.Create(agent,targetCreature);
		agent.ObserveCreature(targetCreature);
        //UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
        CloseWindow();
    }

    public bool AddAgentToObserveList(AgentModel model) {
        if (this.agentList.Contains(model) || this.agentList.Count >= 5) {
            return false;   
        }

        this.agentList.Add(model);
        return true;
    }

    public bool RemoveAgentFromObserveList(AgentModel model) {
        if (this.agentList.Contains(model))
        {
            this.agentList.Remove(model);
            return true;
        }
        else
            return false;
    }

    public void ShowAgentList()
    {
        AgentModel[] agents = targetCreature.sefira.agentList.ToArray();
        
        //패닉 사망 등의 상황이 아니라면 리스트에 남기게끔 검사하는 과정을 추가할 것
        /*
            foreach(){
         *  
         *  }
         */

        foreach (Transform child in agentScrollTarget) {
            Destroy(child.gameObject);
        }

        float posy = 0;
        int cnt = 0;
        foreach (AgentModel unit in agents)
        {
            AgentAIState state = unit.GetState();
            
            if (state == AgentAIState.MANAGE || state == AgentAIState.OBSERVE)
                continue;
            

            GameObject slot = Prefab.LoadPrefab("AgentSlotPanelObserve");
           
            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = Vector3.zero;
            tr.anchoredPosition = new Vector2(0f, -posy);
            posy += tr.rect.height;

            AgentSlotPanelObserve slotPanel = slot.GetComponent<AgentSlotPanelObserve>();

            slotPanel.Init(unit);

            //slotPanel.skillButton1.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.directSkill.imgsrc);

            /*
            if (cnt % 2 == 0)
            {
                slotPanel.bg.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi");
            }
            else {
                slotPanel.bg.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
            }

            AgentModel copied = unit;
            slotPanel.skillButton1.onClick.AddListener(() => SelectAgentSkill(copied));


            slotPanel.agentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.agentFace.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.agentHair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
            */
            //Debug.Log(tr.localPosition);

            cnt++;
        }

        RectTransform scrollTarget = agentScrollTarget.GetComponent<RectTransform>();
        Vector2 delta = scrollTarget.sizeDelta;
        delta = new Vector2(delta.x, posy);
        scrollTarget.sizeDelta = delta;

    }

    public void GetAllocatedAgents(ref List<AgentModel> agents, ref List<OfficerModel> officers) {
        if (this.agentList.Count < this.needAgentCnt) {
            return;
        }
        agents = new List<AgentModel>(this.agentList.ToArray());
        officers = new List<OfficerModel>(this.officerList.ToArray());
        //CloseWindow(); -> ButtonClicked
    }

    public void CloseWindow()
    {
        //gameObject.SetActive (false);
        Deactivate();
        currentWindow.gameObject.SetActive(false);
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
