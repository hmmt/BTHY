using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GameManager : MonoBehaviour {

	private CurrentUIState currentUIState;

	void Awake()
	{
		Screen.fullScreen = true;
	}

	void Start()
	{
		StartGame ();
	}

	// start managing isolate
	public void StartGame()
	{
		currentUIState = CurrentUIState.DEFAULT;

		GameStaticDataLoader.LoadStaticData ();

		EnergyModel.instance.Init ();
		GetComponent<RootTimer> ().AddTimer ("EnergyTimer", 5);
		Notice.instance.Observe ("EnergyTimer", EnergyModel.instance);
		GetComponent<RootTimer> ().AddTimer ("CreatureFeelingUpdateTimer", 10);

        
		AgentFacade.instance.AddAgent (1, 6, 0);
		AgentFacade.instance.AddAgent (2, 7, 0);
//		AgentFacade.instance.AddAgent (3, 8, 0);
//		AgentFacade.instance.AddAgent (4, 7, 1);

		CreatureFacade.instance.AddCreature (10001, "1002001", -8, -1);
		CreatureFacade.instance.AddCreature (10002, "1003002", -16, -1);
		CreatureFacade.instance.AddCreature (10003, "1004101", 8, -1);
		CreatureFacade.instance.AddCreature (10004, "1004102", 17, -1);
		CreatureFacade.instance.AddCreature (10005, "1003111-left-1", -10, -9);
		CreatureFacade.instance.AddCreature (10006, "1003111-right-1", 10, -9);

        // Na??
        CreatureFacade.instance.AddCreature(20001, "N-way1-point2", -25, -4);
        CreatureFacade.instance.AddCreature(20002, "N-way1-point3", -25, -10);
        CreatureFacade.instance.AddCreature(20003, "N-way2-point1", -25, -30);
        CreatureFacade.instance.AddCreature(20004, "N-way2-point2", -25, -36);

        //CreatureFacade.instance.AddCreature(10001, "N-center-way-point1", -18, -16);
        //CreatureFacade.instance.AddCreature(10001, "N-center-way-point1", -18, -24);

		Notice.instance.Send ("AddPlayerLog", "game start가나다");
	}

	public void EndGame()
	{
		GetComponent<RootTimer> ().RemoveTimer ("EnergyTimer");
	}

	public void SetCurrentUIState(CurrentUIState state)
	{
		currentUIState = state;
	}
	public CurrentUIState GetCurrentUIState()
	{
		return currentUIState;
	}
	/*
	public void LoadMetadata()
	{
		// skills

		StreamReader sr = new StreamReader (Application.dataPath + "/xml/Skills.xml");
		
		string text = sr.ReadToEnd ();
		sr.Close ();


		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (text);

		XmlNodeList nodes = doc.SelectNodes ("/skills/skill");

		List<SkillTypeInfo> skillTypeList = new List<SkillTypeInfo> ();

		foreach(XmlNode node in nodes)
		{
			SkillTypeInfo model = new SkillTypeInfo();

			model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.name = node.Attributes.GetNamedItem("name").InnerText;
			model.type = node.Attributes.GetNamedItem("type").InnerText;
			model.amount = int.Parse(node.Attributes.GetNamedItem("amount").InnerText);

			skillTypeList.Add(model);
		}

		SkillTypeList.instance.Init (skillTypeList.ToArray ());

		// agents

		sr = new StreamReader (Application.dataPath + "/xml/Agents.xml");

		text = sr.ReadToEnd ();
		sr.Close ();

		doc = new XmlDocument ();
		doc.LoadXml (text);

		nodes = doc.SelectNodes ("/agent_list/agent");

		List<AgentTypeInfo> agentTypeList = new List<AgentTypeInfo> ();

		foreach(XmlNode node in nodes)
		{
			AgentTypeInfo model = new AgentTypeInfo();

			model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.name = node.Attributes.GetNamedItem("name").InnerText;
			model.hp = int.Parse(node.Attributes.GetNamedItem("hp").InnerText);
			model.mental = int.Parse(node.Attributes.GetNamedItem("mental").InnerText);
			model.movement = int.Parse(node.Attributes.GetNamedItem("movement").InnerText);
			model.work = int.Parse(node.Attributes.GetNamedItem("work").InnerText);

			XmlNode preferSkillNode = node.SelectSingleNode("preferSkill");
			model.prefer = preferSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.preferBonus = int.Parse(preferSkillNode.Attributes.GetNamedItem("bonus").InnerText);
			
			XmlNode rejectSkillNode = node.SelectSingleNode("rejectSkill");
			model.reject = rejectSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.rejectBonus = int.Parse(rejectSkillNode.Attributes.GetNamedItem("bonus").InnerText);

			long directSkillId = long.Parse(node.Attributes.GetNamedItem("directSkillId").InnerText);
			long indirectSkillId = long.Parse(node.Attributes.GetNamedItem("indirectSkillId").InnerText);
			long blockSkillId = long.Parse(node.Attributes.GetNamedItem("blockSkillId").InnerText);

			model.directSkill = SkillTypeList.instance.GetData(directSkillId);
			model.indirectSkill = SkillTypeList.instance.GetData(indirectSkillId);
			model.blockSkill = SkillTypeList.instance.GetData(blockSkillId);

			XmlNode imgNode = node.SelectSingleNode("img");
			model.imgsrc = imgNode.Attributes.GetNamedItem("src").InnerText;

			XmlNodeList speechList = node.SelectNodes("speech");
			Dictionary<string, string> speechTable = new Dictionary<string, string>();
			foreach(XmlNode speechNode in speechList)
			{
				string action = speechNode.Attributes.GetNamedItem("action").InnerText;
				string speechText = speechNode.Attributes.GetNamedItem("text").InnerText;
				speechTable.Add(action, speechText);
			}

			model.speechTable = speechTable;


			agentTypeList.Add(model);
		}

		AgentTypeList.instance.Init (agentTypeList.ToArray ());

		// creature

		sr = new StreamReader (Application.dataPath + "/xml/Creatures.xml");
		
		text = sr.ReadToEnd ();
		sr.Close ();
		
		doc = new XmlDocument ();
		doc.LoadXml (text);
		
		nodes = doc.SelectNodes ("/creature_list/creature");

		List<CreatureTypeInfo> creatureTypeList = new List<CreatureTypeInfo> ();

		foreach(XmlNode node in nodes)
		{
			CreatureTypeInfo model = new CreatureTypeInfo();

			model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.name = node.Attributes.GetNamedItem("name").InnerText;
			model.codeId = node.Attributes.GetNamedItem("codeId").InnerText;
			model.level = int.Parse(node.Attributes.GetNamedItem("level").InnerText);
			model.attackType = node.Attributes.GetNamedItem("attackType").InnerText;
			model.intelligence = int.Parse(node.Attributes.GetNamedItem("intelligence").InnerText);
			model.horrorProb = int.Parse(node.Attributes.GetNamedItem("horrorProb").InnerText);
			model.horrorDmg = int.Parse(node.Attributes.GetNamedItem("horrorDmg").InnerText);
			
			model.physicsProb = int.Parse(node.Attributes.GetNamedItem("physicsProb").InnerText);
			model.physicsDmg = int.Parse(node.Attributes.GetNamedItem("physicsDmg").InnerText);
			
			model.mentalProb = int.Parse(node.Attributes.GetNamedItem("mentalProb").InnerText);
			model.mentalDmg = int.Parse(node.Attributes.GetNamedItem("mentalDmg").InnerText);
			
			model.feelingMax = int.Parse(node.Attributes.GetNamedItem("feelingMax").InnerText);
			model.feelingDownProb = int.Parse(node.Attributes.GetNamedItem("feelingDownProb").InnerText);
			model.feelingDownValue = int.Parse(node.Attributes.GetNamedItem("feelingDownValue").InnerText);

			XmlNode preferSkillNode = node.SelectSingleNode("preferSkill");
			model.prefer = preferSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.preferBonus = int.Parse(preferSkillNode.Attributes.GetNamedItem("bonus").InnerText);

			XmlNode rejectSkillNode = node.SelectSingleNode("rejectSkill");
			model.reject = rejectSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.rejectBonus = int.Parse(rejectSkillNode.Attributes.GetNamedItem("bonus").InnerText);

			XmlNodeList genEnergy = node.SelectNodes("genEnergy/item");
			List<int> items = new List<int>();
			foreach(XmlNode itemNode in genEnergy)
			{
				items.Add(int.Parse(itemNode.Attributes.GetNamedItem("value").InnerText));
			}
			items.Sort();
			model.genEnergy = items.ToArray();

			XmlNode imgNode = node.SelectSingleNode("img");
			model.imgsrc = imgNode.Attributes.GetNamedItem("src").InnerText;

			creatureTypeList.Add(model);
		}

		CreatureTypeList.instance.Init (creatureTypeList.ToArray ());
	}
	*/
}
