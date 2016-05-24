using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WorkType
{
    NORMAL,
    ESACAPE
}

public class SelectWorkAgentWindow : MonoBehaviour, AgentSlot.IReceiver {

	public Transform agentScrollTarget;
	public Transform anchor;

	private int state1 = 0;

    private WorkType workType;
	private CreatureModel targetCreature = null;
    private Transform attachedNode = null;

    private SkillTypeInfo specialSkill = null;

    List<AgentModel> selectedAgentList = new List<AgentModel>();

    private List<AgentSlotPanel> agentPanelList = new List<AgentSlotPanel>();
    //public?
    private WorkInventory inventory;
    private WorkListScript workListScript;

    private CreaturePriority priority;
    public WorkRestrictionScript workRestrictionScript;


	public static SelectWorkAgentWindow currentWindow = null;

    public static SelectWorkAgentWindow CreateWindow(CreatureModel creature, WorkType type)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }
        //GameObject newObj = Prefab.LoadPrefab("SelectWorkAgentWindow");
		GameObject newObj = Prefab.LoadPrefab("SelectWorkAgentWindowProto");

        SelectWorkAgentWindow inst = newObj.GetComponent<SelectWorkAgentWindow>();
        //inst.ShowSelectAgent (unit.gameObject);
        inst.targetCreature = creature;

        inst.workType = type;

        inst.inventory = inst.GetComponent<WorkInventory>();
		inst.inventory.targetCreature = creature;
        inst.inventory.Init();

        inst.workListScript = inst.GetComponent<WorkListScript>();
        inst.workListScript.Init(creature);
        
        inst.workRestrictionScript.Init(creature);
        if (type == WorkType.NORMAL)
        {
            CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(creature.instanceId);

            inst.attachedNode = unit.room.transform;
        }
        else if (type == WorkType.ESACAPE)
        {   
            CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(creature.instanceId);

            inst.attachedNode = unit.transform;
        }

        //inst.ShowAgentList();
        inst.priority = inst.gameObject.GetComponent<CreaturePriority>();
        inst.priority.Init(inst.targetCreature);
        currentWindow = inst;

        return inst;
    }
	
	// Use this for initialization
	void Awake () {
		//UpdatePosition ();
	}
	void Start() {
		UpdatePosition ();
	}
	
	// Update is called once per frame
	void Update () {	
	}
	
	void FixedUpdate()
	{
        UpdateButton();
		UpdatePosition ();
	}

    private void UpdateButton()
    {
        if (workType == WorkType.NORMAL)
        {
            UpdateSpecialSkillButton();
        }
    }

    private void UpdateSpecialSkillButton()
    {
        if (targetCreature.script != null &&
            specialSkill != targetCreature.script.GetSpecialSkill())
        {
            specialSkill = targetCreature.script.GetSpecialSkill();
            foreach (AgentSlotPanel panel in agentPanelList)
            {
                SetSkillButton(panel.skillButton4, panel.targetAgent, specialSkill);
            }
        }
    }

	private void UpdatePosition()
	{
        if (attachedNode != null)
        {
            Vector3 targetPos = attachedNode.position;

            anchor.position = Camera.main.WorldToScreenPoint(targetPos + new Vector3(0, -3, 0));
        }
	}
	
	public void OnClickAgentOK()
	{

	}
	public void OnClickClose()
	{
		CloseWindow ();
	}

    public void SelectAgentSkill(AgentModel agent, SkillTypeInfo skillInfo)
	{
		//UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
		CloseWindow ();
	}

    public void SelectEscapeWorkAgent(AgentModel agent)
    {
        AgentAIState agentState = agent.GetState();

        if (agentState != AgentAIState.IDLE)
        {
            Debug.Log("agent's state must be IDLE");
            return;
        }

        //WorkEscapedCreature.Create(agent, targetCreature);
		Debug.LogError("invalid funtion");
        CloseWindow();
    }

    private void SetSkillButton(UnityEngine.UI.Button button, AgentModel agent, SkillTypeInfo skillInfo)
    {
        if (skillInfo != null)
        {
            button.image.sprite = ResourceCache.instance.GetSprite("Sprites/" + skillInfo.imgsrc);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectAgentSkill(agent, skillInfo));
            button.enabled = true;
        }
        else
        {
            button.image.sprite = ResourceCache.instance.GetSprite("Sprites/UI/skill/Work_disable");
            button.enabled = false;
        }
    }

	// 지금 안 씀
	public void OnClickSlot(GameObject slotObject)
	{
		AgentSlot agentSlot = slotObject.GetComponent<AgentSlot> ();

        AgentModel[] agents = AgentManager.instance.GetAgentList();
        AgentModel unit = agents[agentSlot.slotIndex];
		
		if(!selectedAgentList.Contains(unit))
		{
			if(selectedAgentList.Count > 0)
				return;
			selectedAgentList.Add(unit);
			agentSlot.SetSelect(true);
		}
		else
		{
			selectedAgentList.Remove(unit);
			agentSlot.SetSelect(false);
		}
		OnClickAgentOK ();
	}

	public void CloseWindow()
	{
		//gameObject.SetActive (false);
        this.inventory.WindowDestroy();
        //SefiraManager.instance.getSefira(targetCreature.sefiraNum).priority.SetPriority(targetCreature, priority.GetCnt());
		currentWindow = null;
		Destroy(gameObject);
	}

    public string MentalCheck(AgentModel unit)
    {
        if (unit.mental >= unit.maxMental * 2 / 3f)
        {
            return "멘탈 : 건강";
        }

        else if (unit.mental <= unit.maxMental * 2 / 3f && unit.mental >= unit.maxMental * 1 / 3f)
        {
            return "멘탈 : 보통";
        }

        else if (unit.mental >= unit.maxMental * 1 / 3f)
        {
            return "멘탈 : 심각";
        }

        else
        {
            return "멘탈 : ???";
        }

    }

    public string HealthCheck(AgentModel unit)
    {

        if (unit.hp >= unit.hp * 2 / 3f)
        {
            return "신체 : 건강";
        }

        else if (unit.hp <= unit.maxHp * 2 / 3f && unit.hp >= unit.maxHp * 1 / 3f)
        {
            return "신체 : 보통";
        }

        else if (unit.hp >= unit.maxHp * 1 / 3f)
        {
            return "신체 : 심각";
        }

        else
        {
            return "신체 : ???";
        }
    }

    public void OnRestrictionChanged() {

        this.workListScript.OnChanged();
    }
}