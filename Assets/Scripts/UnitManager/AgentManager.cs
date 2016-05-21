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

    private int nextInstId = 1;
    private List<AgentModel> agentList;
    public List<AgentModel> agentListSpare;

    //실험 - 유닛 시체

    public List<AgentModel> agentListDead;

    public int agentCount = 5;
	
    public AgentManager()
	{
        Init();
	}

    public void Init()
    {
        agentList = new List<AgentModel>();
        agentListSpare = new List<AgentModel>();
        agentListDead = new List<AgentModel>();

        Notice.instance.Observe(NoticeName.ChangeAgentSefira, this);
    }

    public AgentModel AddAgentModel()
    {

        AgentTypeInfo info = AgentTypeList.instance.GetData(1);
        
        if (info == null)
        {
            return null;
        }

        AgentModel unit = new AgentModel(nextInstId++, "1");

        TraitTypeInfo RandomEiTrait = TraitTypeList.instance.GetRandomEITrait(unit.traitList);
        TraitTypeInfo RandomNfTrait = TraitTypeList.instance.GetRandomNFTrait(unit.traitList);
        TraitTypeInfo RandomNormalTrait = TraitTypeList.instance.GetRandomInitTrait();


        unit.name = GetRandomName();

        unit.defaultMaxHp = unit.hp = info.hp;
        unit.defaultMaxMental = unit.mental = info.mental;
        unit.defaultMovement = info.movement;
        unit.defaultWork = info.work;

        unit.gender = info.gender;

        unit.SetModelSprite();

        unit.level = info.level;

        unit.speechTable = new Dictionary<string, string>(info.speechTable);
        /*
        unit.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.imgsrc);
        */

        unit.SetCurrentSefira("0");
        unit.activated = false;
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

    public string setRandomSprite(int count)
    {
        int num = Random.Range(0, count);

        if (num == 0)
        {
            return "A";
        }

        else if (num == 1)
        {
            return "B";
        }

        else if (num == 2)
        {
            return "C";
        }

        else if (num == 3)
        {
            return "D";
        }

        else if (num == 4)
        {
            return "E";
        }

        else if (num == 5)
        {
            return "F";
        }

        else if (num == 6)
        {
            return "C";
        }

        else if (num ==7)
        {
            return "H";
        }
        else if (num == 8)
        {
            return "I";
        }

        else if (num == 9)
        {
            return "J";
        }


        else
        {
            Debug.Log("스프라이트 범위 넘어감");
            return "";
        }
    }

    public void activateAgent(AgentModel unit, string sefira)
    {
        unit.activated = true;
        //Debug.Log("activated");
        Sefira targetSefira = SefiraManager.instance.GetSefira(unit.currentSefira);
        if (targetSefira != null) {
            //Debug.Log("AgentActivated");
            targetSefira.AddAgent(unit);
        }
        
        unit.SetCurrentSefira(sefira);
        agentListSpare.Remove(unit);

        Notice.instance.Observe(NoticeName.FixedUpdate, unit);
        agentList.Add(unit);
        Notice.instance.Send(NoticeName.AddAgent, unit);
    }

    public void deactivateAgent(AgentModel unit)
    {
        unit.activated = false;
        //Debug.Log("deactivated");
        Sefira UnitSefira = SefiraManager.instance.GetSefira(unit.currentSefira);
        if (UnitSefira != null)
        {
            UnitSefira.RemoveAgent(unit);
        }
        else {
            return;
        }
        
        Notice.instance.Remove(NoticeName.FixedUpdate, unit);
        agentList.Remove(unit);
        Notice.instance.Send(NoticeName.RemoveAgent, unit);

        agentListSpare.Add(unit);
        unit.SetCurrentSefira("0");
       
    }

    public void RemoveAgent(AgentModel model)
    {/*
        if (model.currentSefira == "1")
        {
            malkuthAgentList.Remove(model);
        }

        else if (model.currentSefira == "2")
        {
            nezzachAgentList.Remove(model);
        }

        else if (model.currentSefira == "3")
        {
            hodAgentList.Remove(model);
        }

        else if (model.currentSefira == "4")
        {
            yesodAgentList.Remove(model);
        }
        */
        Sefira sefira = SefiraManager.instance.GetSefira(model.currentSefira);
        sefira.RemoveAgent(model);

        Notice.instance.Remove(NoticeName.FixedUpdate, model);
        agentList.Remove(model);
        //agentListDead.Add(model);
        Notice.instance.Send(NoticeName.RemoveAgent, model);
    }

    public void ClearAgent()
    {
        foreach (AgentModel model in agentList)
        {
            Notice.instance.Remove(NoticeName.FixedUpdate, model);
            Notice.instance.Send(NoticeName.RemoveAgent, model);
        }
        AgentLayer.currentLayer.ClearAgent();

        agentList = new List<AgentModel>();
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

        dic.Add("agentList", list);

        return dic;
    }
    public void LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file =  File.Open(Application.persistentDataPath + "/test.txt", FileMode.Open);
        Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(file);
        file.Close();

        LoadData(dic);
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


            AgentModel model = new AgentModel(agentId, sefira);
            model.LoadData(data);

            agentList.Add(model);

            Notice.instance.Send(NoticeName.AddAgent, model);
        }
    }

	/*
    public AgentModel[] GetNearAgents(MovableObjectNode node)
    {
        List<AgentModel> output = new List<AgentModel>();
        foreach (AgentModel agent in agentList)
        {
            if (node.CheckInRange(agent.GetMovableNode()))
            {
                output.Add(agent);
                
            }
        }
        return output.ToArray();
    }
    */
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
        /*
        Debug.Log("Change");
        if (oldSefira != "0") {
            old = SefiraManager.instance.getSefira(oldSefira);
            old.RemoveAgent(agentModel);
        }
        */
        old = SefiraManager.instance.GetSefira(oldSefira);
        if (old != null)
        {
            old.RemoveAgent(agentModel);
            //deactivateAgent(agentModel);
        }


        current = SefiraManager.instance.GetSefira(agentModel.currentSefira);
        if (current != null)
        {
            current.AddAgent(agentModel);
            //activateAgent(agentModel, agentModel.currentSefira);
        }
        else {
            deactivateAgent(agentModel);
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
