using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GameStaticDataLoader {


	public static void LoadStaticData()
	{
		GameStaticDataLoader loader = new GameStaticDataLoader ();
		loader.LoadSKillData ();
		loader.LoadCreatureData ();
		loader.LoadAgentData ();
	}


	public void LoadSKillData()
	{
		// skills
		StreamReader sr = new StreamReader (Application.dataPath + "/Resources/xml/Skills.xml");
		
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

            model.imgsrc = node.Attributes.GetNamedItem("imgsrc").InnerText;
			
			skillTypeList.Add(model);
		}
		
		SkillTypeList.instance.Init (skillTypeList.ToArray ());
	}

	public void LoadAgentData()
	{
		// agents
        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/xml/Agents.xml");
		
		string text = sr.ReadToEnd ();
		sr.Close ();
		
		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (text);
		
		XmlNodeList nodes = doc.SelectNodes ("/agent_list/agent");
		
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

			XmlNode panicTypeNode = node.SelectSingleNode("panic");
			model.panicType = panicTypeNode.Attributes.GetNamedItem("action").InnerText;
			
			
			agentTypeList.Add(model);
		}
		
		AgentTypeList.instance.Init (agentTypeList.ToArray ());
	}

	public void LoadCreatureData()
	{
		// creature

        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/xml/Creatures.xml");
		
		string text = sr.ReadToEnd ();
		sr.Close ();
		
		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (text);
		
		XmlNodeList nodes = doc.SelectNodes ("/creature_list/creature");
		
		List<CreatureTypeInfo> creatureTypeList = new List<CreatureTypeInfo> ();
		
		foreach(XmlNode node in nodes)
		{
			CreatureTypeInfo model = new CreatureTypeInfo();
			
			model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.name = node.Attributes.GetNamedItem("name").InnerText;
			model.codeId = node.Attributes.GetNamedItem("codeId").InnerText;
			model.level = node.Attributes.GetNamedItem("level").InnerText;
			model.attackType = node.Attributes.GetNamedItem("attackType").InnerText;
			model.intelligence = node.Attributes.GetNamedItem("intelligence").InnerText;


			model.horrorProb = float.Parse(node.Attributes.GetNamedItem("horrorProb").InnerText);
			model.horrorDmg = int.Parse(node.Attributes.GetNamedItem("horrorDmg").InnerText);
			
			model.physicsProb = float.Parse(node.Attributes.GetNamedItem("physicsProb").InnerText);
			model.physicsDmg = int.Parse(node.Attributes.GetNamedItem("physicsDmg").InnerText);
			
			model.mentalProb = float.Parse(node.Attributes.GetNamedItem("mentalProb").InnerText);
			model.mentalDmg = int.Parse(node.Attributes.GetNamedItem("mentalDmg").InnerText);

			model.script = node.Attributes.GetNamedItem("script").InnerText;

			XmlNode feelingNode = node.SelectSingleNode("feeling");
			model.feelingMax = int.Parse(feelingNode.Attributes.GetNamedItem("max").InnerText);
			model.feelingDownProb = float.Parse(feelingNode.Attributes.GetNamedItem("downProb").InnerText);
			model.feelingDownValue = int.Parse(feelingNode.Attributes.GetNamedItem("downValue").InnerText);

			XmlNode skillAttr = node.Attributes.GetNamedItem("specialSkillId");
			if(skillAttr != null)
			{
				model.specialSkill = SkillTypeList.instance.GetData(long.Parse(skillAttr.InnerText));
			}

			List<int> energyItems = new List<int>();
			XmlNodeList genEnergies = feelingNode.SelectNodes("section");

			// temp
			// TODO : must read feeling value
			foreach(XmlNode itemNode in genEnergies)
			{
				energyItems.Add(int.Parse(itemNode.Attributes.GetNamedItem("energy").InnerText));
			}
			energyItems.Sort();
			model.genEnergy = energyItems.ToArray();

			
			XmlNode preferSkillNode = node.SelectSingleNode("preferSkill");
			model.prefer = preferSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.preferBonus = float.Parse(preferSkillNode.Attributes.GetNamedItem("bonus").InnerText);
			
			XmlNode rejectSkillNode = node.SelectSingleNode("rejectSkill");
			model.reject = rejectSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.rejectBonus = float.Parse(rejectSkillNode.Attributes.GetNamedItem("bonus").InnerText);
			/*
			XmlNodeList genEnergy = node.SelectNodes("genEnergy/item");
			List<int> items = new List<int>();
			foreach(XmlNode itemNode in genEnergy)
			{
				items.Add(int.Parse(itemNode.Attributes.GetNamedItem("value").InnerText));
			}
			items.Sort();
			model.genEnergy = items.ToArray();
			*/
			
			XmlNode imgNode = node.SelectSingleNode("img");
			model.imgsrc = imgNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode roomNode = node.SelectSingleNode("room");
            model.roomsrc = roomNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode frameNode = node.SelectSingleNode("frame");
            model.framesrc = frameNode.Attributes.GetNamedItem("src").InnerText;

			Dictionary<string, string> typoTable = new Dictionary<string, string>();
			XmlNodeList typoNodeList = node.SelectNodes("typo");
			foreach(XmlNode typoNode in typoNodeList)
			{
				string key = typoNode.Attributes.GetNamedItem("action").InnerText;
				string ttext = typoNode.InnerText;
				
				typoTable.Add(key, ttext);
			}
			model.typoTable = typoTable;

			Dictionary<string, string> narrationTable = new Dictionary<string, string>();
			XmlNodeList narrationNodeList = node.SelectNodes("narration");
			foreach(XmlNode narrationNode in narrationNodeList)
			{
				string key = narrationNode.Attributes.GetNamedItem("action").InnerText;
				string ntext = narrationNode.InnerText.Trim();

				narrationTable.Add(key, ntext);
			}
			model.narrationTable = narrationTable;

            Dictionary<string, string> soundTable = new Dictionary<string, string>();
            XmlNodeList soundNodeList = node.SelectNodes("sound");
            foreach (XmlNode soundNode in soundNodeList)
            {
                string key = soundNode.Attributes.GetNamedItem("action").InnerText;
                string stext = soundNode.Attributes.GetNamedItem("src").InnerText;

                soundTable.Add(key, stext);
            }
            model.soundTable = soundTable;

            XmlNode descNode = node.SelectSingleNode("desc");
            string descData = descNode.InnerText;
            model.desc = TextConverter.TranslateDescData(descData);

            XmlNode observeNode = node.SelectSingleNode("observe");
            string observeData = observeNode.InnerText;
            model.observe = TextConverter.TranslateDescData(observeData);

            // inner graph
            model.nodeInfo = node.SelectNodes("graph/node");
            model.edgeInfo = node.SelectNodes("graph/edge");
			creatureTypeList.Add(model);
		}
		
		CreatureTypeList.instance.Init (creatureTypeList.ToArray ());
	}
}
