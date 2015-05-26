using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<AgentModel> agentList;
	
    public AgentManager()
	{
        Init();
	}

    public void Init()
    {
        agentList = new List<AgentModel>();
    }

    private int instId = 1;
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

        AgentModel unit = new AgentModel(instId++);

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

        unit.metadata = info;
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

        unit.imgsrc = info.imgsrc;

        unit.speechTable = new Dictionary<string, string>(info.speechTable);

        unit.panicType = info.panicType;


        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
        unit.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        agentList.Add(unit);

        Notice.instance.Send(NoticeName.AddAgent, unit);

        return unit;
    }

    public void RemoveAgent(AgentModel model)
    {
        agentList.Remove(model);
        Notice.instance.Send(NoticeName.RemoveAgent, model);
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
}
