using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;


public class AgentManager : IObserver, ISerializablePlayData {

    public static string[] nameList
        = {
              "Tim", "Jacob", "Mason", "William", "Jayden", "Noah", "Micheal", "Ethan",
              "Paul", "Elijah", "Joshua", "Liam", "Christopher", "Ryan", "Issac", "Isaiah",
              "Susan", "Sophia", "Ava", "Emily", "Chloe", "Grace", "Charlotte", "Lilian", 
              "Alyssa", "Ashley"
          };

	private static AgentManager _instance;
	
	public static AgentManager instance
	{
		get
        {
            if (_instance == null)
                _instance = new AgentManager();
            return _instance;
        }
	}

    private int nextInstId;
    private List<AgentModel> agentList;
    public List<AgentModel> agentListSpare;

    //실험 - 유닛 시체

    public List<AgentModel> agentListDead;

    public int agentCount;
	
    public AgentManager()
	{
        Init();
	}

	private void InitValues()
	{
		nextInstId = 1;
		agentCount = 5;

		agentList = new List<AgentModel>();
		agentListSpare = new List<AgentModel> ();
		agentListDead = new List<AgentModel> ();
	}
    public void Init()
    {
		InitValues ();
		Notice.instance.Observe(NoticeName.ChangeAgentSefira, this);
    }

	public void Clear()
	{
		InitValues ();

		Notice.instance.Send (NoticeName.ClearAgent);
	}

    public AgentModel AddAgentModel()
    {

        AgentTypeInfo info = AgentTypeList.instance.GetData(1);
        
        if (info == null)
        {
            return null;
        }

        AgentModel unit = new AgentModel(nextInstId++);

        TraitTypeInfo RandomEiTrait = TraitTypeList.instance.GetRandomEITrait(unit.traitList);
        TraitTypeInfo RandomNfTrait = TraitTypeList.instance.GetRandomNFTrait(unit.traitList);
        TraitTypeInfo RandomNormalTrait = TraitTypeList.instance.GetRandomInitTrait();


        unit.name = GetRandomName();

        unit.defaultMaxHp = unit.hp = info.hp;
        unit.defaultMaxMental = unit.mental = info.mental;
        unit.defaultMovement = info.movement;
        unit.defaultWork = info.work;

		unit.gender = Random.Range(0, 2) == 1 ? "Female" : "Male";
		//unit.gender = "Female";

        unit.SetModelSprite();

        unit.level = info.level;

        unit.speechTable = new Dictionary<string, string>(info.speechTable);
        /*
        unit.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.imgsrc);
        */

        agentListSpare.Add(unit);

        unit.applyTrait(RandomEiTrait);
        unit.applyTrait(RandomNfTrait);
        unit.applyTrait(RandomNormalTrait);
        AddSpecialSkillToAgent(unit);
        /*
		if(AgentListScript.instance != null)
        	AgentListScript.instance.SetAgentList(unit);
        */

        AgentAllocateWindow.instance.AddAgent(unit);
        /*
        Debug.Log("EI Trait "+RandomEiTrait.name);
        Debug.Log("Nf Trait " + RandomNfTrait.name);
        Debug.Log("가치관 " + unit.agentLifeValue);
        */

        return unit;
    }

    public void AddSpecialSkillToAgent(AgentModel model) {
        model.AddSpecialSkill(SkillTypeList.instance.GetData(40002));
        model.AddSpecialSkill(SkillTypeList.instance.GetData(40003));
        if (model.agentLifeValue == PersonalityType.D || model.agentLifeValue == PersonalityType.C) {
            model.AddSpecialSkill(SkillTypeList.instance.GetData(40004));
        }
    }

    private void ActivateAgent(AgentModel unit)
    {
        unit.activated = true;
		/*
        Sefira targetSefira = SefiraManager.instance.GetSefira(unit.currentSefira);
        if (targetSefira != null) {
            targetSefira.AddAgent(unit);
        }
        */
        
        //unit.SetCurrentSefira(sefira);
        agentListSpare.Remove(unit);

        Notice.instance.Observe(NoticeName.FixedUpdate, unit);
        agentList.Add(unit);
        Notice.instance.Send(NoticeName.AddAgent, unit);
    }

    private void DeactivateAgent(AgentModel unit)
    {
        unit.activated = false;
		/*
        Sefira UnitSefira = SefiraManager.instance.GetSefira(unit.currentSefira);
        if (UnitSefira != null)
        {
            UnitSefira.RemoveAgent(unit);
        }
        */
        
        Notice.instance.Remove(NoticeName.FixedUpdate, unit);
        agentList.Remove(unit);
        Notice.instance.Send(NoticeName.RemoveAgent, unit);

        agentListSpare.Add(unit);
        //unit.SetCurrentSefira("0");
       
    }

    public void RemoveAgent(AgentModel model)
    {
        Sefira sefira = SefiraManager.instance.GetSefira(model.currentSefira);
        sefira.RemoveAgent(model);

        Notice.instance.Remove(NoticeName.FixedUpdate, model);
        agentList.Remove(model);
        //agentListDead.Add(model);
        Notice.instance.Send(NoticeName.RemoveAgent, model);
    }
	
    public AgentModel[] GetAgentList()
    {
        return agentList.ToArray();
    }


    public AgentModel BuyAgent()
    {
        float energy = EnergyModel.instance.GetEnergy();
        //int needEnergy = 1;
        return AgentManager.instance.AddAgentModel();
    }

    private static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field)
    {
        object output;
        if (dic.TryGetValue(name, out output))
        {
            field = (T)output;
            return true;
        }
        return false;
    }

    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> dic = new Dictionary<string,object>();

        dic.Add("nextInstId", nextInstId);

        List<Dictionary<string, object>> list = new List<Dictionary<string,object>>();

        foreach (AgentModel agent in agentList)
        {
            Dictionary<string, object> agentData = agent.GetSaveData();
            list.Add(agentData);
        }

		foreach (AgentModel agent in agentListSpare)
		{
			Dictionary<string, object> agentData = agent.GetSaveData();
			list.Add(agentData);
		}

        dic.Add("agentList", list);

        return dic;
    }
    public void LoadData(Dictionary<string, object> dic)
    {
		TryGetValue(dic, "nextInstId", ref nextInstId);

        List<Dictionary<string, object>> agentDataList = new List<Dictionary<string,object>>();
        TryGetValue(dic, "agentList", ref agentDataList);
        foreach (Dictionary<string, object> data in agentDataList)
        {
            int agentId = 0;
            string sefira = "";

            TryGetValue(data, "instanceId", ref agentId);
            TryGetValue(data, "currentSefira", ref sefira);


            AgentModel model = new AgentModel(agentId);
            model.LoadData(data);
			agentListSpare.Add (model);

			model.SetCurrentSefira (sefira);
        }
    }

	public AgentModel[] GetNearAgents(MovableObjectNode node)
	{
		List<AgentModel> output = new List<AgentModel>();
		foreach (AgentModel agent in agentList)
		{
			if (agent.isDead ())
				continue;
			/*
			if (node.CheckInRange(agent.GetMovableNode()))
			{
				output.Add(agent);
			}
*/
			Vector3 dist = node.GetCurrentViewPosition () - agent.GetMovableNode ().GetCurrentViewPosition ();
			if (node.GetPassage () == agent.GetMovableNode ().GetPassage () &&
			   dist.sqrMagnitude <= 25) {
				output.Add(agent);
			}
		}
		return output.ToArray();
	}

    private static string GetRandomName()
    {
        return nameList[Random.Range(0, nameList.Length)];
    }

    private void OnChangeAgentSefira(AgentModel agentModel, string oldSefira)
    {
        Sefira old, current;


        old = SefiraManager.instance.GetSefira(oldSefira);
        if (old != null)
        {
            old.RemoveAgent(agentModel);
        }


        current = SefiraManager.instance.GetSefira(agentModel.currentSefira);
        if (current != null)
        {
            current.AddAgent(agentModel);
        }

		if (agentModel.activated && current == null )
		{
			DeactivateAgent (agentModel);
		}
		else if (!agentModel.activated && current != null)
		{
			ActivateAgent (agentModel);
		}

        /*

        if (agentModel.currentSefira != "0")
        {
            current = SefiraManager.instance.getSefira(agentModel.currentSefira);
            current.AddAgent(agentModel);
        }
        */
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.ChangeAgentSefira)
        {
            AgentModel agent = (AgentModel)param[0];
            string oldSefira = (string)param[1];
            OnChangeAgentSefira(agent, oldSefira);
            Notice.instance.Send(NoticeName.ChangeAgentSefira_Late);
        }
    }
}
