﻿using UnityEngine;
using UnityEngine.UI;
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

public class SuppressWindow : MonoBehaviour
{
    public enum TargetType { 
        CREATURE,
        AGENT
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

            AgentModel.SetPortraitSprite(this.model, this.face.sprite, this.hair.sprite);

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

    private object target;

    public object Target {
        get { return GetTarget(); }
    }

    private TargetType targetType;
    private Transform attachedPos;
    private Sefira currentSefira;

    public static SuppressWindow currentWindow = null;
    
    public static SuppressWindow CreateWindow(CreatureModel target) {
        if (currentWindow != null) {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Prefab.LoadPrefab("SuppressionWindow");

        SuppressWindow inst = newObj.GetComponent<SuppressWindow>();
        inst.target = target;
        inst.targetType = TargetType.CREATURE;
        inst.currentSefira = target.sefira;
        inst.agentList = new List<SuppressAction>();

        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(target.instanceId);
        inst.attachedPos = unit.transform;
        inst.ui.Init(target, inst.targetType);

        inst.InitAgentList();
        inst.ShowAgentList();
        currentWindow = inst;
        return inst;
    }

    public static SuppressWindow CreateWindow(AgentModel target)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Prefab.LoadPrefab("SuppressionWindow");

        SuppressWindow inst = newObj.GetComponent<SuppressWindow>();
        inst.target = target;
        inst.targetType = TargetType.AGENT;
		inst.currentSefira = SefiraManager.instance.getSefira(target.currentSefira);
		//inst.currentSefira = SefiraManager.instance.getSefira("1");
        inst.agentList = new List<SuppressAction>();

        AgentUnit unit = AgentLayer.currentLayer.GetAgent(target.instanceId);
        inst.attachedPos = unit.transform;
        inst.ui.Init(target, inst.targetType);

        inst.InitAgentList();
        inst.ShowAgentList();
        currentWindow = inst;
        return inst;
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
			SuppressAction sa = new SuppressAction ((AgentModel)target);
			sa.weapon = SuppressAction.Weapon.GUN;

			actor.StartSuppressAgent((AgentModel)target, sa, SuppressType.UNCONTROLLABLE);
		}
	}

    public void CloseWindow() {
        currentWindow = null;
        Destroy(gameObject);
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
}
