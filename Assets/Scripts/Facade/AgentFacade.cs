using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentFacade : MonoBehaviour {

	private static AgentFacade _instance;
	
	public static AgentFacade instance
	{
		get{ return _instance; }
	}
	
	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
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

	public AgentUnit AddAgent(long typeId, int x, int y)
	{
		AgentTypeInfo info = AgentTypeList.instance.GetData (typeId);

        TraitTypeInfo traitInfo = TraitTypeList.instance.GetRandomInitTrait();

		if(info == null)
		{
			return null;
		}

		AgentUnit unit = NewAgent ();

        unit.traitNameList.Add(traitInfo.name);

		unit.metadata = info;
		unit.metadataId = info.id;
		
		unit.name = info.name;

		unit.hp = info.hp + traitInfo.hp;
		unit.mental = info.mental + traitInfo.mental;
		unit.movement = info.movement + traitInfo.moveSpeed;
		unit.work = info.work + traitInfo.workSpeed;

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
		unit.SetMaxHP (info.hp);




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
}
