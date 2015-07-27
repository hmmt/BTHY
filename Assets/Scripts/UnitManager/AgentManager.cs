using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

public class AgentManager {

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
	
    public AgentManager()
	{
        Init();
	}

    public void Init()
    {
        agentList = new List<AgentModel>();
    }

    public AgentModel AddAgentModel(long typeId)
    {
        int traitHp = 0;
        int traitMental = 0;
        int traitMoveSpeed = 0;
        int traitWorkSpeed = 0;

        AgentTypeInfo info = AgentTypeList.instance.GetData(typeId);

        if (info == null)
        {
            return null;
        }

        AgentModel unit = new AgentModel(nextInstId++, "1");

        TraitTypeInfo RandomTraitInfo1 = TraitTypeList.instance.GetRandomInitTrait();
        TraitTypeInfo RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();

        if (RandomTraitInfo1.id == RandomTraitInfo2.id)
        {
            while (true)
            {
                RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();
                if (RandomTraitInfo1.id != RandomTraitInfo2.id)
                    break;
            }
        }

        unit.traitList.Add(RandomTraitInfo1);
        unit.traitList.Add(RandomTraitInfo2);

        for (int i = 0; i < unit.traitList.Count; i++)
        {
            traitHp += unit.traitList[i].hp;
            traitMental += unit.traitList[i].mental;
            traitMoveSpeed += unit.traitList[i].moveSpeed;
            traitWorkSpeed += unit.traitList[i].workSpeed;

            //unit.traitNameList.Add(unit.traitList[i].name);
        }

        //unit.metadata = info;
        unit.metadataId = info.id;

        unit.name = info.name;

        unit.maxHp = unit.hp = info.hp + traitHp;
        unit.maxMental = unit.mental = info.mental + traitMental;
        unit.movement = info.movement + traitMoveSpeed;
        unit.work = info.work + traitWorkSpeed;

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
        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
        unit.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        */

        Notice.instance.Observe(NoticeName.FixedUpdate, unit);
        agentList.Add(unit);

        Notice.instance.Send(NoticeName.AddAgent, unit);

        return unit;
    }

    public void RemoveAgent(AgentModel model)
    {
        Notice.instance.Remove(NoticeName.FixedUpdate, model);
        agentList.Remove(model);
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


    public bool BuyAgent(long id)
    {
        AgentTypeInfo info = AgentTypeList.instance.GetData(id);

        //info.

        float energy = EnergyModel.instance.GetEnergy();
        //int needEnergy = 1;

        AgentManager.instance.AddAgentModel(id);

        return true;
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
}
