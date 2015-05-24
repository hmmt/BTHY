using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour {

	private static AgentManager _instance;
	
	public static AgentManager instance
	{
		get{ return _instance; }
	}
	
	void Awake()
	{
		_instance = this;
	}

	//private List<AgentUnit> agentList = new List<AgentUnit>();

	public AgentUnit NewAgent()
	{
		/*
		GameObject newUnit = new GameObject ("unit");
		//newUnit.AddComponent<RectDrawer> ().texture = Resources.Load<Texture2D> ("Sprites/Unit/unit1");
		newUnit.AddComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Sprites/Unit/unit1");
		
		
		GameObject touchable = new GameObject ("unit_touchable");
		ButtonMsgHandler btn = touchable.AddComponent<ButtonMsgHandler>();
		btn.functionName = "OpenStatusWindow";
		btn.target = newUnit;
		touchable.layer = LayerMask.NameToLayer ("UI");
		touchable.transform.parent = newUnit.transform;

		
		newUnit.transform.parent = GameObject.Find ("Units").transform;
		newUnit.transform.localPosition = new Vector3(0,0,0);

		return newUnit.AddComponent<AgentUnit>();
		*/
		GameObject newUnit = Prefab.LoadPrefab ("unit");
		newUnit.transform.SetParent( GameObject.Find ("Units").transform);
		return newUnit.GetComponent<AgentUnit> ();
	}

	public AgentUnit AddAgent(long typeId)
	{
        int traitHp=0;
        int traitMental=0;
        int traitMoveSpeed=0;
        int traitWorkSpeed=0;

		AgentTypeInfo info = AgentTypeList.instance.GetData (typeId);
        
        if (info == null)
        {
            return null;
        }

        AgentUnit unit = NewAgent();

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

            unit.traitNameList.Add(unit.traitList[i].name);
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

		unit.speechTable = new Dictionary<string, string> (info.speechTable);

		unit.panicType = info.panicType;

		Texture2D tex = Resources.Load<Texture2D> ("Sprites/"+unit.imgsrc);
		//unit.spriteRenderer.sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f, 0.5f));
		unit.spriteRenderer.sprite = null;

		/*
		Vector2 pos = CreatureRoom.instance.TileToWorld (x, y);
		unit.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
*/
		unit.SetMaxHP (unit.maxHp);

		return unit;
	}
	
	public AgentUnit[] GetAgentList()
	{
		GameObject units = GameObject.Find ("Units");
		if(units == null)
		{
			return new AgentUnit[]{};
		}

		List<AgentUnit> output = new List<AgentUnit> ();
		foreach(Transform child in units.transform)
		{
			AgentUnit com = child.GetComponent<AgentUnit>();
			if(com != null)
			{
				output.Add(com);
			}
		}

		return output.ToArray ();
	}

    public bool BuyAgent(long id)
    {
        AgentTypeInfo info = AgentTypeList.instance.GetData(id);

        //info.

        float energy = EnergyModel.instance.GetEnergy();
        //int needEnergy = 1;

        AgentManager.instance.AddAgent(id);

        return true;
    }
}
