﻿using UnityEngine;
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

    List<AgentModel> selectedAgentList = new List<AgentModel>();

	public static SelectWorkAgentWindow currentWindow = null;

    public static SelectWorkAgentWindow CreateWindow(CreatureModel creature, WorkType type)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }
        GameObject newObj = Prefab.LoadPrefab("SelectWorkAgentWindow");

        SelectWorkAgentWindow inst = newObj.GetComponent<SelectWorkAgentWindow>();
        //inst.ShowSelectAgent (unit.gameObject);
        inst.targetCreature = creature;

        inst.workType = type;

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

        inst.ShowAgentList();

        currentWindow = inst;
        return inst;
    }
	
	// Use this for initialization
	void Awake () {
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
		UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
		CloseWindow ();
	}

    public void SelectEscapeWorkAgent(AgentModel agent)
    {
        AgentCmdState agentState = agent.GetState();

        if (agentState != AgentCmdState.IDLE)
        {
            Debug.Log("agent's state must be IDLE");
            return;
        }

        WorkEscapedCreature.Create(agent, targetCreature);
        CloseWindow();
    }

    private void AddAgentSlotWork(AgentModel unit, ref float posy)
    {
        GameObject slot = Prefab.LoadPrefab ("AgentSlotPanel");

		slot.transform.SetParent (agentScrollTarget, false);

		RectTransform tr = slot.GetComponent<RectTransform>();
		tr.localPosition = new Vector3(0,posy,0);
		AgentSlotPanel slotPanel = slot.GetComponent<AgentSlotPanel>();

        slotPanel.skillButton1.image.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.directSkill.imgsrc);
        slotPanel.skillButton2.image.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.indirectSkill.imgsrc);
        slotPanel.skillButton3.image.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.blockSkill.imgsrc);

        slotPanel.agentName.text = unit.name;
        slotPanel.agentHealth.text = HealthCheck(unit);
        slotPanel.agentMental.text = MentalCheck(unit);
        slotPanel.agentLevel.text = "등급 : "+unit.level;

        AgentModel copied = unit;
		slotPanel.skillButton1.onClick.AddListener(()=>SelectAgentSkill(copied, copied.directSkill));
		slotPanel.skillButton2.onClick.AddListener(()=>SelectAgentSkill(copied, copied.indirectSkill));
		slotPanel.skillButton3.onClick.AddListener(()=>SelectAgentSkill(copied, copied.blockSkill));

		if(targetCreature.specialSkill != null)
            slotPanel.skillButton4.onClick.AddListener(() => SelectAgentSkill(copied, targetCreature.specialSkill));
		else
			slotPanel.skillButton4.gameObject.SetActive(false);

        slotPanel.agentIcon.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.imgsrc);

        slotPanel.agentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
        slotPanel.agentFace.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
        slotPanel.agentHair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
        posy -= 100f;
    }

    private void AddAgentSlotEscape(AgentModel unit, ref float posy)
    {
        GameObject slot = Prefab.LoadPrefab("AgentSlotPanel");

        slot.transform.SetParent(agentScrollTarget, false);

        RectTransform tr = slot.GetComponent<RectTransform>();
        tr.localPosition = new Vector3(0, posy, 0);
        AgentSlotPanel slotPanel = slot.GetComponent<AgentSlotPanel>();

        slotPanel.skillButton1.image.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.directSkill.imgsrc);
        slotPanel.skillButton2.gameObject.SetActive(false);
        slotPanel.skillButton3.gameObject.SetActive(false);
        slotPanel.skillButton4.gameObject.SetActive(false);

        slotPanel.agentName.text = unit.name;
        slotPanel.agentHealth.text = HealthCheck(unit);
        slotPanel.agentMental.text = MentalCheck(unit);
        slotPanel.agentLevel.text = "등급 : " + unit.level;

        AgentModel copied = unit;
        slotPanel.skillButton1.onClick.AddListener(() => SelectEscapeWorkAgent(copied));


        slotPanel.agentIcon.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.imgsrc);

        posy -= 100f;
    }

	public void ShowAgentList()
	{
        AgentModel[] agents = AgentManager.instance.GetAgentList();

		float posy = 0;
        foreach (AgentModel unit in agents)
		{
            if (unit.GetState() == AgentCmdState.WORKING)
                continue;

            if (workType == WorkType.NORMAL)
            {
                AddAgentSlotWork(unit, ref posy);
            }
            else if (workType == WorkType.ESACAPE)
            {
                AddAgentSlotEscape(unit, ref posy);
            }
		}

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy + 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

		UpdatePosition ();
	}

	
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
		currentWindow = null;
		Destroy (gameObject);
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
}