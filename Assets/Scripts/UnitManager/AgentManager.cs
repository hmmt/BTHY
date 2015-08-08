using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

public class AgentManager {

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

    public int agentCount = 5;
	
    public AgentManager()
	{
        Init();
	}

    public void Init()
    {
        agentList = new List<AgentModel>();
        agentListSpare = new List<AgentModel>();
    }

    public AgentModel AddAgentModel(long typeId)
    {
        int traitHp = 0;
        int traitMental = 0;
        int traitMoveSpeed = 0;
        int traitWorkSpeed = 0;
        int traitDirect = 0;
        int traitInDirect = 0;
        int traitBlock = 0;

        AgentTypeInfo info = AgentTypeList.instance.GetData(typeId);

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

        unit.traitList.Add(RandomTraitInfo1);
        unit.traitList.Add(RandomTraitInfo2);
        unit.traitList.Add(WorkTrait);

        for (int i = 0; i < unit.traitList.Count; i++)
        {
            traitHp += unit.traitList[i].hp;
            traitMental += unit.traitList[i].mental;
            traitMoveSpeed += unit.traitList[i].moveSpeed;
            traitWorkSpeed += unit.traitList[i].workSpeed;

            traitDirect += (int)unit.traitList[i].directWork;
            traitInDirect += (int)unit.traitList[i].inDirectWork;
            traitBlock += (int)unit.traitList[i].blockWork;

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

        unit.faceSpriteName = setRandomSprite(3);
        unit.hairSpriteName = setRandomSprite(3);
        unit.bodySpriteName = setRandomSprite(1);

        unit.AgentPortrait("hair",unit.hairSpriteName);
        unit.AgentPortrait("face", unit.faceSpriteName);
        unit.AgentPortrait("body", null);

        unit.SetCurrentSefira("0");
        unit.activated = false;
        agentListSpare.Add(unit);

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


    public AgentModel BuyAgent(long id)
    {
        AgentTypeInfo info = AgentTypeList.instance.GetData(id);

        //info.

        float energy = EnergyModel.instance.GetEnergy();
        //int needEnergy = 1;
        return AgentManager.instance.AddAgentModel(id);
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
