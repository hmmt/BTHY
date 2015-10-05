using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

public class AgentManager : IObserver {

    public static string[] nameList
        = {
              "one",
              "two"
          };

	public static AgentManager _instance;
	
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
    public List<AgentModel> malkuthAgentList;
    public List<AgentModel> hodAgentList;
    public List<AgentModel> nezzachAgentList;
    public List<AgentModel> yesodAgentList;

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

        malkuthAgentList = new List<AgentModel>();
        hodAgentList = new List<AgentModel>();
        nezzachAgentList = new List<AgentModel>();
        yesodAgentList = new List<AgentModel>();

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

        TraitTypeInfo RandomTraitInfo1 = TraitTypeList.instance.GetRandomInitTrait();
        TraitTypeInfo RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();
        TraitTypeInfo WorkTrait = TraitTypeList.instance.GetRandomLevelWorkTrait(1);

        if (RandomTraitInfo1.id == RandomTraitInfo2.id)
        {
            while (true)
            {
                RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();
                if (RandomTraitInfo1.id != RandomTraitInfo2.id)
                    break;
            }
        }



        unit.name = GetRandomName();

        unit.defaultMaxHp = unit.hp = info.hp;
        unit.defaultMaxMental = unit.mental = info.mental;
        unit.defaultMovement = info.movement;
        unit.defaultWork = info.work;

        unit.gender = info.gender;
        unit.level = info.level;
        unit.workDays = info.workDays;

        unit.prefer = info.prefer;
        unit.preferBonus = info.preferBonus;
        unit.reject = info.reject;
        unit.rejectBonus = info.rejectBonus;

        unit.directSkill = info.directSkill;
        unit.indirectSkill = info.indirectSkill;
        unit.blockSkill = info.blockSkill;

        unit.speechTable = new Dictionary<string, string>(info.speechTable);

        unit.panicType = info.panicType;
        /*
        unit.sprite = ResourceCache.instance.GetSprite("Sprites/" + unit.imgsrc);
        */

        unit.faceSpriteName = setRandomSprite(8);
        unit.hairSpriteName = setRandomSprite(9);
        unit.bodySpriteName = setRandomSprite(1);
        unit.panicSpriteName = setRandomSprite(3);

        unit.AgentPortrait("hair",unit.hairSpriteName);
        unit.AgentPortrait("face", unit.faceSpriteName);
        unit.AgentPortrait("body", null);

        unit.SetCurrentSefira("0");
        unit.activated = false;
        agentListSpare.Add(unit);

        unit.applyTrait(RandomTraitInfo1);
        unit.applyTrait(RandomTraitInfo2);
        unit.applyTrait(WorkTrait);

        return unit;
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

        unit.SetCurrentSefira(sefira);
        agentListSpare.Remove(unit);

        Notice.instance.Observe(NoticeName.FixedUpdate, unit);
        agentList.Add(unit);
        Notice.instance.Send(NoticeName.AddAgent, unit);
    }

    public void deactivateAgent(AgentModel unit)
    {
        unit.activated = false;

        Notice.instance.Remove(NoticeName.FixedUpdate, unit);
        agentList.Remove(unit);
        Notice.instance.Send(NoticeName.RemoveAgent, unit);

        agentListSpare.Add(unit);
        unit.SetCurrentSefira("0");
    }

    public void RemoveAgent(AgentModel model)
    {
        if (model.currentSefira == "1")
        {
            malkuthAgentList.Remove(model);
        }

        else if (model.currentSefira == "2")
        {
            nezzachAgentList.Remove(model);
        }

        else  if (model.currentSefira == "3")
        {
            hodAgentList.Remove(model);
        }

        else if (model.currentSefira == "4")
        {
            yesodAgentList.Remove(model);
        }

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
        /*
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/test.txt");
        bf.Serialize(file, dic);
        file.Close();
        */
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

    private static string GetRandomName()
    {
        return nameList[Random.Range(0, nameList.Length)];
    }

    private void OnChangeAgentSefira(AgentModel agentModel, string oldSefira)
    {
        switch (oldSefira)
        {
        case "1":
            malkuthAgentList.Remove(agentModel);
            break;
        case "2":
            nezzachAgentList.Remove(agentModel);
            break;
        case "3":
            hodAgentList.Remove(agentModel);
            break;
        case "4":
            yesodAgentList.Remove(agentModel);
            break;
        }

        switch (agentModel.currentSefira)
        {
        case "1":
            malkuthAgentList.Add(agentModel);
            break;
        case "2":
            nezzachAgentList.Add(agentModel);
            break;
        case "3":
            hodAgentList.Add(agentModel);
            break;
        case "4":
            yesodAgentList.Add(agentModel);
            break;
        }
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
