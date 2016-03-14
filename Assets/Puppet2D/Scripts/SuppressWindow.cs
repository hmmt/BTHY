using UnityEngine;
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
}

public class SuppressWindow : MonoBehaviour
{
    public enum TargetType { 
        CREATURE,
        AGENT
    }
    
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

        public void Init(object target, TargetType type) {
            this.type = type;
            this.target = target;
            switch(type){
                case TargetType.CREATURE:
                    {
                        this.currentHealth.gameObject.SetActive(false);
                        this.currentMental.gameObject.SetActive(false);
                        this.movementSpeed.gameObject.SetActive(false);

                        CreatureModel model = target as CreatureModel;
                        this.name.text = model.metaInfo.name;
                        this.grade.text = model.metaInfo.level;
                        this.fear.text = model.metaInfo.level;//?
                        this.physical.text = model.metaInfo.physicsDmg.ToString();
                        this.mental.text = model.metaInfo.mentalDmg.ToString();
                        break;
                    }
                case TargetType.AGENT:
                    {
                        this.fear.gameObject.SetActive(false);
                        this.physical.gameObject.SetActive(false);
                        this.mental.gameObject.SetActive(false);

                        AgentModel model = target as AgentModel;
                        this.name.text = model.name;
                        this.grade.text = model.level.ToString() ;
                        this.currentHealth.text = model.hp.ToString() ;
                        this.currentMental.text = model.mental.ToString();
                        this.movementSpeed.text = model.movement.ToString();
                    }
                    break;
            }
        }

        public void Update() {
            if (type != TargetType.AGENT) return;
            this.currentHealth.text = (target as AgentModel).hp.ToString();
            this.currentMental.text = (target as AgentModel).mental.ToString();
            this.movementSpeed.text = (target as AgentModel).movement.ToString();
        }

    }

    public RectTransform AgentScrollTarget;
    public RectTransform anchor;
    public SuppressWindowUI ui;

    public List<SuppressAction> agentList;

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

        inst.ShowAgentList();

        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(target.instanceId);
        inst.attachedPos = unit.transform;

        inst.InitAgentList();
        inst.ui.Init(target, inst.targetType);

        Debug.Log(target.metaInfo.name);

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
        inst.currentSefira = SefiraManager.instance.getSefira(target.sefira);
        inst.agentList = new List<SuppressAction>();

        inst.ShowAgentList();

        AgentUnit unit = AgentLayer.currentLayer.GetAgent(target.instanceId);
        inst.attachedPos = unit.transform;

        inst.InitAgentList();
        inst.ui.Init(target, inst.targetType);

        Debug.Log(target.name);

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

    void Awake() { 
        //UpdatePosition;
    }

    void FixedUpdate() { 
        //Update
    }
    /// <summary>
    /// Not used
    /// </summary>
    private void UpdatePosition() {
        if (attachedPos != null)
        {
            Vector3 targetPos = attachedPos.position;
            anchor.position = Camera.main.WorldToScreenPoint(targetPos + new Vector3(0, -3, 0));
        }
    }

    private void InitAgentList() {
        List<AgentModel> list = new List<AgentModel>(currentSefira.agentList.ToArray());

        foreach (AgentModel model in list) {
            //패닉 상태인지 확인하는 과정이 필요함
            if (model.panicFlag != true) {
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
    }

    public void CloseWindow() {
        currentWindow = null;
        Destroy(gameObject);
    }

}
