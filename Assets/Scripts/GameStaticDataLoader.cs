using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GameStaticDataLoader {

	public static void LoadStaticData()
	{
		GameStaticDataLoader loader = new GameStaticDataLoader ();
        //if(SkillTypeList.instance.loaded == false)
		    //loader.LoadSKillData ();
        if (CreatureTypeList.instance.loaded == false)
		    loader.LoadCreatureList ();
        if (AgentTypeList.instance.loaded == false)
		    loader.LoadAgentData ();

        if(TraitTypeList.instance.loaded == false)
            loader.LoadTraitData();

        if (PassageObjectTypeList.instance.loaded == false)
            loader.LoadPassageData();

        if (SystemMessageManager.instance.isLoaded() == false)
            loader.LoadSystemMessage();

        if (ConversationManager.instance.isLoaded() == false)
            loader.LoadDayScript();

        if (SkillManager.instance.isLoaded == false) {
            loader.LoadSkillInfo();
        }

        if (AgentLyrics.instance.IsLoaded() == false) {
            loader.LoadLyricData();
        }


	}

    public void LoadTraitData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("xml/Traits2");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList nodes = doc.SelectNodes("/traits/trait");

        List<TraitTypeInfo> traitTypeList = new List<TraitTypeInfo>();
        List<TraitTypeInfo> NFList = new List<TraitTypeInfo>();
        List<TraitTypeInfo> EIList = new List<TraitTypeInfo>();
        List<TraitTypeInfo>[] levelList = new List<TraitTypeInfo>[5];

        for (int i = 0; i < 5; i++)
        {
            levelList[i] = new List<TraitTypeInfo>();
        }

        int r = 0;
        foreach (XmlNode node in nodes)
        {
            TraitTypeInfo model = new TraitTypeInfo();

			model.id = (long)float.Parse(node.Attributes.GetNamedItem("id").InnerText);
            model.name = node.Attributes.GetNamedItem("name").InnerText;

			model.level = (int)float.Parse(node.Attributes.GetNamedItem("level").InnerText);
            //model.randomFlag = int.Parse(node.Attributes.GetNamedItem("randomFlag").InnerText);
			model.randomFlag = r++%2;

			model.hp = (int)float.Parse(node.Attributes.GetNamedItem("hp").InnerText);
			model.mental = (int)float.Parse(node.Attributes.GetNamedItem("mental").InnerText);

			model.move = float.Parse(node.Attributes.GetNamedItem("move").InnerText);
			model.work = float.Parse(node.Attributes.GetNamedItem("work").InnerText);

            model.energy = float.Parse(node.Attributes.GetNamedItem("energy").InnerText);
            model.attack = float.Parse(node.Attributes.GetNamedItem("attack").InnerText);
            model.success = float.Parse(node.Attributes.GetNamedItem("success").InnerText);

            model.upwork = node.Attributes.GetNamedItem("upwork").InnerText;

			model.discType = (int)float.Parse(node.Attributes.GetNamedItem("type").InnerText);

            model.description = node.Attributes.GetNamedItem("description").InnerText;
            model.effect = node.Attributes.GetNamedItem("effect").InnerText;
            model.condition = node.Attributes.GetNamedItem("condition").InnerText;

            int imgFlag = (int)float.Parse(node.Attributes.GetNamedItem("imgflag").InnerText);
            if (imgFlag == 1)
            {
                model.haveImg = true;
                model.imgsrc = node.Attributes.GetNamedItem("imgsrc").InnerText;
                model.imgPos = node.Attributes.GetNamedItem("imgpos").InnerText;
            }
            else {
                model.haveImg = false;
                model.imgsrc = "";
                model.imgPos = "";
            }

            if (model.randomFlag == 1) {
                levelList[model.level - 1].Add(model);
                switch (model.discType) { 
                    case 1:
                    case 2:
                        EIList.Add(model);
                        break;
                    case 3:
                    case 4:
                        NFList.Add(model);
                        break;
                    default:
                        break;
                }
            }
            
            traitTypeList.Add(model);
        }
        TraitTypeList.instance.Init(traitTypeList.ToArray(), EIList.ToArray(), NFList.ToArray(), levelList);

    }

    public void LoadSkillInfo() {
        TextAsset textAsset = Resources.Load<TextAsset>("xml/Work");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNode root = doc.SelectSingleNode("root");

        XmlNodeList skillNodes = root.SelectNodes("skill");

        List<SkillTypeInfo> skillList = new List<SkillTypeInfo>();
        
        foreach (XmlNode node in skillNodes) {
            SkillTypeInfo unit = new SkillTypeInfo();
            unit.name = node.SelectSingleNode("name").InnerText;
            unit.level = int.Parse(node.SelectSingleNode("level").InnerText);

            unit.amount = float.Parse(node.SelectSingleNode("amount").InnerText);
            unit.feeling = float.Parse(node.SelectSingleNode("feeling").InnerText);
            unit.energy = float.Parse(node.SelectSingleNode("energy").InnerText);
            unit.coolTime = float.Parse(node.SelectSingleNode("cooltime").InnerText);

            unit.type = GetCreatureType(node.SelectSingleNode("type").InnerText);

            float min, max;
            min = float.Parse(node.SelectSingleNode("intmin").InnerText);
            max = float.Parse(node.SelectSingleNode("intmax").InnerText);
            unit.intelligence.SetValue(min, max);

            min = float.Parse(node.SelectSingleNode("sizemin").InnerText);
            max = float.Parse(node.SelectSingleNode("sizemax").InnerText);
            unit.size.SetValue(min, max);

            min = float.Parse(node.SelectSingleNode("atkmin").InnerText);
            max = float.Parse(node.SelectSingleNode("atkmax").InnerText);
            unit.attack.SetValue(min, max);

            min = float.Parse(node.SelectSingleNode("gomin").InnerText);
            max = float.Parse(node.SelectSingleNode("gomax").InnerText);
            unit.gorgeous.SetValue(min, max);

            min = float.Parse(node.SelectSingleNode("dmgmin").InnerText);
            max = float.Parse(node.SelectSingleNode("dmgmax").InnerText);
            unit.damage.SetValue(min, max);

            unit.animID = int.Parse(node.SelectSingleNode("anim").InnerText);

            unit.imgsrc = node.SelectSingleNode("imgsrc").InnerText;

            string categoryText = node.SelectSingleNode("category").InnerText;
            SkillCategory category = SkillManager.instance.GetCategoryByName(categoryText);
            if (category == null) {
                int tier =  int.Parse(node.SelectSingleNode("tier").InnerText);
                SkillCategory newItem = new SkillCategory(categoryText, tier);
                SkillManager.instance.AddCategory(newItem);
                category = newItem;
            }

            category.AddSkill(unit);
        }
        SkillManager.instance.SortList();
    }

    private CreatureType GetCreatureType(string type) {
        switch (type) { 
            case "BIO":
                return CreatureType.BIO;
            case "THI":
                return CreatureType.THING;
            case "ALL":
                return CreatureType.ALL;
            case "ABS":
                return CreatureType.ABS;
            default:
                return CreatureType.NONE;
        }
    }

    public void LoadSystemMessage() {
        TextAsset textAsset = Resources.Load<TextAsset>("xml/Sysmessage");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        
        XmlNodeList messageNodes = doc.SelectNodes("system/message");
        XmlNodeList keywordNodes = doc.SelectNodes("system/keyword");
        List<SystemMessage> messageList = new List<SystemMessage>();
        List<KeywordMessage> keywordList = new List<KeywordMessage>();

        foreach (XmlNode node in messageNodes)
        {
            SystemMessage model = new SystemMessage();
            model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
            model.name = node.Attributes.GetNamedItem("name").InnerText;
            messageList.Add(model);
        }

        foreach (XmlNode node in keywordNodes)
        {
            KeywordMessage model = new KeywordMessage();
            model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
            model.name = node.Attributes.GetNamedItem("name").InnerText;
            model.desc = node.Attributes.GetNamedItem("desc").InnerText;
            keywordList.Add(model);
        }

        SystemMessageManager.instance.Init(messageList.ToArray(), keywordList.ToArray());
    }

    private ConversationModel.Description[] LoadDesc(XmlNodeList descList) {
        List<ConversationModel.Description> output = new List<ConversationModel.Description>();
        foreach (XmlNode dNode in descList)
        {
            ConversationModel.Description description = new ConversationModel.Description();
            description.id = long.Parse(dNode.Attributes.GetNamedItem("id").InnerText);
            description.speaker = short.Parse(dNode.Attributes.GetNamedItem("speaker").InnerText);
            description.selectId = long.Parse(dNode.Attributes.GetNamedItem("select").InnerText);
            description.tempdesc = dNode.InnerText.Trim();
            //Debug.Log(description.tempdesc);
            description.loadText();

            XmlNode endNode = dNode.SelectSingleNode("end");
            if (endNode != null)
            {

                description.endId = endNode.Attributes.GetNamedItem("id").InnerText;
                description.isEnd = true;
                //Debug.Log(description.endId + description.tempdesc);
            }

            XmlNodeList sys = dNode.SelectNodes("sys");
            if (sys.Count != 0)
            {
                foreach (XmlNode s in sys)
                {
                    int type = int.Parse(s.Attributes.GetNamedItem("call").InnerText);
                    SystemMessage sm = null;
                    switch (type)
                    {
                        case 1:
                            sm = SystemMessageManager.instance.GetSysMessage(1);
                            break;
                        case 2:
                            int target = int.Parse(s.Attributes.GetNamedItem("target").InnerText);
                            sm = SystemMessageManager.instance.GetKeyword(target);
                            break;
                        default:
                            Debug.Log("worng system type in Desc +" + description.id);
                            break;
                    }
                    description.sys.Add(sm);
                }
            }
            output.Add(description);
        }
        return output.ToArray();
    }

    private ConversationModel.Select[] LoadSelect(XmlNodeList selectList) {
        List<ConversationModel.Select> output = new List<ConversationModel.Select>();
        foreach (XmlNode sNode in selectList)
        {
            ConversationModel.Select select = new ConversationModel.Select();
            select.id = long.Parse(sNode.Attributes.GetNamedItem("id").InnerText);

            XmlNodeList innerNodes = sNode.SelectNodes("node");
            foreach (XmlNode selectNode in innerNodes)
            {
                ConversationModel.Select.SelectNode unit = new ConversationModel.Select.SelectNode();
                unit.id = long.Parse(selectNode.Attributes.GetNamedItem("id").InnerText);
                unit.desc = selectNode.Attributes.GetNamedItem("desc").InnerText;
                unit.descId = long.Parse(selectNode.Attributes.GetNamedItem("target").InnerText);
                unit.favor = int.Parse(selectNode.Attributes.GetNamedItem("favor").InnerText);

                select.list.Add(unit);
            }

            output.Add(select);
        }

        return output.ToArray();
    }

    public void LoadDayScript()
    {
        //LoadSystemMessage();
        TextAsset textAsset = Resources.Load<TextAsset>("xml/Day");
        List<ConversationModel> list = new List<ConversationModel>();
        List<EndingModel> endList = new List<EndingModel>();
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList nodes = doc.SelectNodes("script/day");
        foreach (XmlNode node in nodes) {
            ConversationModel model = new ConversationModel();
            model.date = int.Parse(node.Attributes.GetNamedItem("date").InnerText);

            XmlNodeList descList = node.SelectNodes("desc");
            model.InitDescList(LoadDesc(descList));

            XmlNode endingNode = node.SelectSingleNode("ending");
            if (endingNode != null)
            {
                EndingModel end = new EndingModel();
                end.date = model.date;
                end.target = endingNode.Attributes.GetNamedItem("id").InnerText;
                //Debug.Log(end.date + "일 엔딩 " + end.target);
                    
                end.InitDescList(LoadDesc(endingNode.SelectNodes("desc")));
                end.InitSelectList(LoadSelect(endingNode.SelectNodes("select")));
                endList.Add(end);
            }

            XmlNodeList selectList = node.SelectNodes("select");
            model.InitSelectList(LoadSelect(selectList));
            list.Add(model);
        }
        ConversationManager.instance.Init(list.ToArray(), endList.ToArray());
    }

    public void LoadLyricData() {
        TextAsset textAsset = Resources.Load<TextAsset>("xml/AgentLyrics");
        List<AgentLyrics.LyricList> lyricList = new List<AgentLyrics.LyricList>();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        //XmlNode rootNode = doc.SelectSingleNode("root/chat");
        XmlNodeList typeList = doc.SelectNodes("root/chat/type");
        foreach (XmlNode node in typeList) {
            List<AgentLyrics.AgentLyric> templist = new List<AgentLyrics.AgentLyric>();

            int type = (int)float.Parse(node.Attributes.GetNamedItem("num").InnerText);
            XmlNodeList itemList = node.SelectNodes("item");

            foreach (XmlNode item in itemList) {

                AgentLyrics.AgentLyric unitItem = new AgentLyrics.AgentLyric(
                    (int)float.Parse(item.Attributes.GetNamedItem("id").InnerText),
                    item.Attributes.GetNamedItem("desc").InnerText);
                templist.Add(unitItem);
            }
            AgentLyrics.LyricList lyric = new AgentLyrics.LyricList(GetLyricType(type), templist.ToArray());
            lyricList.Add(lyric);
        }

        AgentLyrics.instance.Init(lyricList);
    }

    private LyricType GetLyricType(int type) {
        switch (type) { 
            case 1:
                return LyricType.DAY1;
            case 2:
                return LyricType.DAY;
            case 3:
                return LyricType.DAYSMALL;
            case 4:
                return LyricType.CHAT;
            case 5:
                return LyricType.MENTALBAD;
            case 6:
                return LyricType.LEVELUP;
            case 7:
                return LyricType.ESCAPE;
            case 8:
                return LyricType.SAD;
            default:
                return LyricType.CHAT;
        }
    }

    /*
	public void LoadSKillData()
	{
        // skills
        TextAsset textAsset = Resources.Load<TextAsset>("xml/Skills");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
		
		XmlNodeList nodes = doc.SelectNodes ("/skills/skill");
		
		List<SkillTypeInfo> skillTypeList = new List<SkillTypeInfo> ();
		
		foreach(XmlNode node in nodes)
		{
			SkillTypeInfo model = new SkillTypeInfo();
			
			model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.name = node.Attributes.GetNamedItem("name").InnerText;
			model.type = node.Attributes.GetNamedItem("type").InnerText;
			model.amount = int.Parse(node.Attributes.GetNamedItem("amount").InnerText);
            model.description = node.Attributes.GetNamedItem("desc").InnerText;
            model.category = node.Attributes.GetNamedItem("category").InnerText;
            model.imgsrc = node.Attributes.GetNamedItem("imgsrc").InnerText;

            XmlNodeList bonusType= node.SelectNodes("bonus");
            foreach (XmlNode bonusNode in bonusType)
            {
                if (bonusNode.Attributes.GetNamedItem("type").InnerText == "D")
                {
                    if (bonusNode.SelectSingleNode("amount") != null)
                        model.amountBonusD = float.Parse(bonusNode.SelectSingleNode("amount").InnerText);
                    else
                        model.amountBonusD = 1;

                    if (bonusNode.SelectSingleNode("feeling") != null)
                        model.feelingBonusD = float.Parse(bonusNode.SelectSingleNode("feeling").InnerText);
                    else
                        model.feelingBonusD = 1;

                    if (bonusNode.SelectSingleNode("mentalReduce") != null)
                        model.mentalReduceD = int.Parse(bonusNode.SelectSingleNode("mentalReduce").InnerText);
                    else
                        model.mentalReduceD = 0;

                    if (bonusNode.SelectSingleNode("mentalTick") != null)
                        model.mentalTickD = int.Parse(bonusNode.SelectSingleNode("mentalTick").InnerText);
                    else
                        model.mentalTickD = 0;
                }

                else if (bonusNode.Attributes.GetNamedItem("type").InnerText == "I")
                {
                    if (bonusNode.SelectSingleNode("amount") != null)
                        model.amountBonusI = float.Parse(bonusNode.SelectSingleNode("amount").InnerText);
                    else
                        model.amountBonusI = 1;

                    if (bonusNode.SelectSingleNode("feeling") != null)
                        model.feelingBonusI = float.Parse(bonusNode.SelectSingleNode("feeling").InnerText);
                    else
                        model.feelingBonusI = 1;

                    if (bonusNode.SelectSingleNode("mentalReduce") != null)
                        model.mentalReduceI = int.Parse(bonusNode.SelectSingleNode("mentalReduce").InnerText);
                    else
                        model.mentalReduceI = 0;

                    if (bonusNode.SelectSingleNode("mentalTick") != null)
                        model.mentalTickI = int.Parse(bonusNode.SelectSingleNode("mentalTick").InnerText);
                    else
                        model.mentalTickI = 0;
                }

                else if (bonusNode.Attributes.GetNamedItem("type").InnerText == "S")
                {
                    if (bonusNode.SelectSingleNode("amount") != null)
                        model.amountBonusS = float.Parse(bonusNode.SelectSingleNode("amount").InnerText);
                    else
                        model.amountBonusS = 1;

                    if (bonusNode.SelectSingleNode("feeling") != null)
                        model.feelingBonusS = float.Parse(bonusNode.SelectSingleNode("feeling").InnerText);
                    else
                        model.feelingBonusS = 1;

                    if (bonusNode.SelectSingleNode("mentalReduce") != null)
                        model.mentalReduceS = int.Parse(bonusNode.SelectSingleNode("mentalReduce").InnerText);
                    else
                        model.mentalReduceS = 0;

                    if (bonusNode.SelectSingleNode("mentalTick") != null)
                        model.mentalTickS = int.Parse(bonusNode.SelectSingleNode("mentalTick").InnerText);
                    else
                        model.mentalTickS = 0;
                }

                else if (bonusNode.Attributes.GetNamedItem("type").InnerText == "C")
                {
                    if (bonusNode.SelectSingleNode("amount") != null)
                        model.amountBonusC = float.Parse(bonusNode.SelectSingleNode("amount").InnerText);
                    else
                        model.amountBonusC = 1;

                    if (bonusNode.SelectSingleNode("feeling") != null)
                        model.feelingBonusC = float.Parse(bonusNode.SelectSingleNode("feeling").InnerText);
                    else
                        model.feelingBonusC = 1;

                    if (bonusNode.SelectSingleNode("mentalReduce") != null)
                        model.mentalReduceC = int.Parse(bonusNode.SelectSingleNode("mentalReduce").InnerText);
                    else
                        model.mentalReduceC = 0;

                    if (bonusNode.SelectSingleNode("mentalTick") != null)
                        model.mentalTickC = int.Parse(bonusNode.SelectSingleNode("mentalTick").InnerText);
                    else
                        model.mentalTickC = 0;
                }
            }

            List<long> nextSkillList = new List<long>();
            XmlNodeList nextSkills = node.SelectNodes("nextSkill");
            foreach(XmlNode nextSkillNode in nextSkills)
            {
                long nextSkillId = long.Parse(nextSkillNode.InnerText);
                nextSkillList.Add(nextSkillId);
            }

            model.nextSkillIdList = nextSkillList.ToArray();
			
			skillTypeList.Add(model);
		}
		
		SkillTypeList.instance.Init (skillTypeList.ToArray ());
	}*/

	public void LoadAgentData()
	{
        // agents
        TextAsset textAsset = Resources.Load<TextAsset>("xml/Agents");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
		
		XmlNodeList nodes = doc.SelectNodes ("/agent_list/agent");
		
		List<AgentTypeInfo> agentTypeList = new List<AgentTypeInfo> ();
		
		foreach(XmlNode node in nodes)
		{
			AgentTypeInfo model = new AgentTypeInfo();
			
            /*
			model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.name = node.Attributes.GetNamedItem("name").InnerText;
            */

            model.id = int.Parse(node.Attributes.GetNamedItem("id").InnerText);
            model.name = node.Attributes.GetNamedItem("name").InnerText;

           
			model.hp = int.Parse(node.Attributes.GetNamedItem("hp").InnerText);
			model.mental = int.Parse(node.Attributes.GetNamedItem("mental").InnerText);
			model.movement = int.Parse(node.Attributes.GetNamedItem("movement").InnerText);
			model.work = int.Parse(node.Attributes.GetNamedItem("work").InnerText);

			model.gender = node.Attributes.GetNamedItem("gender").InnerText;
			model.level = int.Parse(node.Attributes.GetNamedItem("level").InnerText);
			model.workDays = int.Parse(node.Attributes.GetNamedItem("workDays").InnerText);
			
			XmlNode preferSkillNode = node.SelectSingleNode("preferSkill");
			model.prefer = preferSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.preferBonus = int.Parse(preferSkillNode.Attributes.GetNamedItem("bonus").InnerText);
			
			XmlNode rejectSkillNode = node.SelectSingleNode("rejectSkill");
			model.reject = rejectSkillNode.Attributes.GetNamedItem("type").InnerText;
			model.rejectBonus = int.Parse(rejectSkillNode.Attributes.GetNamedItem("bonus").InnerText);
			
			long directSkillId = long.Parse(node.Attributes.GetNamedItem("directSkillId").InnerText);
			long indirectSkillId = long.Parse(node.Attributes.GetNamedItem("indirectSkillId").InnerText);
			long blockSkillId = long.Parse(node.Attributes.GetNamedItem("blockSkillId").InnerText);
			/*
			model.directSkill = SkillTypeList.instance.GetData(directSkillId);
			model.indirectSkill = SkillTypeList.instance.GetData(indirectSkillId);
			model.blockSkill = SkillTypeList.instance.GetData(blockSkillId);
			*/
            /*
			XmlNode imgNode = node.SelectSingleNode("img");
			model.imgsrc = imgNode.Attributes.GetNamedItem("src").InnerText;
            */
			
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

	public void LoadCreatureList()
	{
		// creature

        TextAsset textAsset = Resources.Load<TextAsset>("xml/CreatureList");

        XmlDocument list_doc = new XmlDocument();
        list_doc.LoadXml(textAsset.text);
        XmlNodeList creature_list = list_doc.SelectNodes("/creature_list/creature");

        List<CreatureTypeInfo> creatureTypeList = new List<CreatureTypeInfo>();

        foreach (XmlNode pathInfoNode in creature_list)
        {
            string src = pathInfoNode.Attributes.GetNamedItem("src").InnerText;

            
            TextAsset creatureTextAsset = Resources.Load<TextAsset>("xml/creatures/" + src + "_"+ GlobalGameManager.instance.language);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(creatureTextAsset.text);

            XmlNode info = doc.SelectSingleNode("/creature/info");

            XmlNode observe = doc.SelectSingleNode("/creature/observe");

            CreatureTypeInfo model = new CreatureTypeInfo();

            model.id = long.Parse(info.Attributes.GetNamedItem("id").InnerText);
            model.script = info.Attributes.GetNamedItem("script").InnerText;

            /*
            if (info.Attributes.GetNamedItem("animatorScript") != null)
            {
                model.animatorScript = info.Attributes.GetNamedItem("animatorScript").InnerText;
            }*/

            XmlNode stat = info.SelectSingleNode("stat");
            LoadCreatureStat(stat, model);

            Dictionary<string, string> soundTable = new Dictionary<string, string>();
            XmlNodeList soundNodeList = info.SelectNodes("sound");
            foreach (XmlNode soundNode in soundNodeList)
            {
                string key = soundNode.Attributes.GetNamedItem("action").InnerText;
                string stext = soundNode.Attributes.GetNamedItem("src").InnerText;

                soundTable.Add(key, stext);
            }
            model.soundTable = soundTable;

            Dictionary<string, string> narrationTable = new Dictionary<string, string>();
            XmlNodeList narrationNodeList = info.SelectNodes("narration");
            foreach (XmlNode narrationNode in narrationNodeList)
            {
                string key = narrationNode.Attributes.GetNamedItem("action").InnerText;
                string ntext = narrationNode.InnerText.Trim();

                narrationTable.Add(key, ntext);
            }
            model.narrationTable = narrationTable;

            model.nodeInfo = info.SelectNodes("graph/node");
            model.edgeInfo = info.SelectNodes("graph/edge");

            XmlNode imgNode = info.SelectSingleNode("img");
            if (imgNode != null)
                model.imgsrc = imgNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode roomNode = info.SelectSingleNode("room");
            model.roomsrc = roomNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode frameNode = info.SelectSingleNode("frame");
            model.framesrc = frameNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode animNode = info.SelectSingleNode("anim");
            if (animNode != null)
            {
                model.animSrc = animNode.Attributes.GetNamedItem("prefab").InnerText;
            }

            XmlNode roomReturnSrcNode = info.SelectSingleNode("returnImg");
            if (roomReturnSrcNode != null)
            {
                model.roomReturnSrc = roomReturnSrcNode.Attributes.GetNamedItem("src").InnerText;
            }
            else
            {
                model.roomReturnSrc = "";
            }

            /*
                anim, img, portrait, room, frame, graph 읽어오기
             */

            model.CurrentObserveLevel = 0;
            model.MaxObserveLevel = (int)float.Parse(observe.Attributes.GetNamedItem("level").InnerText);

            XmlNode collection = observe.SelectSingleNode("collection");
            LoadCreatureCollectionInfo(collection, model);

            /*
            XmlNode descNode = observe.SelectSingleNode("desc");
            string descData = descNode.InnerText;
            model.desc = TextConverter.TranslateDescData(descData);
            */

            XmlNodeList descList = observe.SelectNodes("desc");

            foreach (XmlNode descNode in descList) {
                int level = (int)float.Parse(descNode.Attributes.GetNamedItem("openLevel").InnerText);
                string text = descNode.InnerText.Trim();

                string input = TextConverter.GetTextFromFormatAlter(text);

                model.desc.Add(input);
                model.observeTable.desc.Add(level);
            }

            XmlNodeList recordList = observe.SelectNodes("record");

            foreach (XmlNode record in recordList) {
                string text = record.InnerText.Trim();
                int openLevel = (int)float.Parse(record.Attributes.GetNamedItem("openLevel").InnerText);

                string input = TextConverter.GetTextFromFormatAlter(text);

                model.observeRecord.Add(input);
                model.observeTable.record.Add(openLevel);
            }

            creatureTypeList.Add(model);
            /*
             XmlNodeList narrationNodeList = node.SelectNodes("narration");
             foreach (XmlNode narrationNode in narrationNodeList)
             {
                 string key = narrationNode.Attributes.GetNamedItem("action").InnerText;
                 string ntext = narrationNode.InnerText.Trim();

                 narrationTable.Add(key, ntext);
             }
             model.narrationTable = narrationTable;
             */

            /*
            //model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
			model.id = long.Parse(pathInfoNode.Attributes.GetNamedItem("id").InnerText);
            //model.name = node.Attributes.GetNamedItem("name").InnerText;
            model.codeId = node.Attributes.GetNamedItem("codeId").InnerText;
            //model.level = node.Attributes.GetNamedItem("level").InnerText;
            //model.attackType = node.Attributes.GetNamedItem("attackType").InnerText;
            model.intelligence = node.Attributes.GetNamedItem("intelligence").InnerText;
            */
           // model.stackLevel = int.Parse(node.Attributes.GetNamedItem("stackLevel").InnerText);
          //  model.observeLevel = int.Parse(node.Attributes.GetNamedItem("observeLevel").InnerText);
            /*
            model.horrorProb = float.Parse(node.Attributes.GetNamedItem("horrorProb").InnerText);
            model.horrorDmg = int.Parse(node.Attributes.GetNamedItem("horrorDmg").InnerText);

            model.physicsProb = float.Parse(node.Attributes.GetNamedItem("physicsProb").InnerText);
            model.physicsDmg = int.Parse(node.Attributes.GetNamedItem("physicsDmg").InnerText);

            model.mentalProb = float.Parse(node.Attributes.GetNamedItem("mentalProb").InnerText);
            model.mentalDmg = int.Parse(node.Attributes.GetNamedItem("mentalDmg").InnerText);
            */
            //model.script = node.Attributes.GetNamedItem("script").InnerText;
            /*
            if (node.Attributes.GetNamedItem("animatorScript") != null)
            {
                model.animatorScript = node.Attributes.GetNamedItem("animatorScript").InnerText;
            }*/
            /*
            XmlNode feelingNode = node.SelectSingleNode("feeling");
            model.feelingMax = int.Parse(feelingNode.Attributes.GetNamedItem("max").InnerText);
            model.feelingDownProb = float.Parse(feelingNode.Attributes.GetNamedItem("downProb").InnerText);
            model.feelingDownValue = float.Parse(feelingNode.Attributes.GetNamedItem("downValue").InnerText);

            List<float> energyItems = new List<float>();
            XmlNodeList genEnergies = feelingNode.SelectNodes("section");
            */
            /*

            // 
            XmlNodeList preferSkillNodeList = node.SelectNodes("preferSkill");
            List<SkillBonusInfo> preferSkillList = new List<SkillBonusInfo>();
            foreach (XmlNode preferSkillNode in preferSkillNodeList)
            {
                SkillBonusInfo preferInfo = new SkillBonusInfo();
                preferInfo.skillType = preferSkillNode.Attributes.GetNamedItem("type").InnerText;
                preferInfo.bonus = float.Parse(preferSkillNode.Attributes.GetNamedItem("bonus").InnerText);
                preferSkillList.Add(preferInfo);
            }
            model.preferList = preferSkillList.ToArray();

            XmlNodeList rejectSkillNodeList = node.SelectNodes("rejectSkill");
            List<SkillBonusInfo> rejectSkillList = new List<SkillBonusInfo>();
            foreach (XmlNode rejectSkillNode in rejectSkillNodeList)
            {
                SkillBonusInfo rejectInfo = new SkillBonusInfo();
                rejectInfo.skillType = rejectSkillNode.Attributes.GetNamedItem("type").InnerText;
                rejectInfo.bonus = float.Parse(rejectSkillNode.Attributes.GetNamedItem("bonus").InnerText);
                rejectSkillList.Add(rejectInfo);
            }
            model.rejectList = preferSkillList.ToArray();
            */
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
            /*
            XmlNode portrait = node.SelectSingleNode("portrait");
            if (portrait != null) {
                model.tempPortrait = ResourceCache.instance.GetSprite("Sprites/" + portrait.Attributes.GetNamedItem("src").InnerText);
            }

            XmlNode imgNode = node.SelectSingleNode("img");
            if(imgNode != null)
                model.imgsrc = imgNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode roomNode = node.SelectSingleNode("room");
            model.roomsrc = roomNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode frameNode = node.SelectSingleNode("frame");
            model.framesrc = frameNode.Attributes.GetNamedItem("src").InnerText;
            XmlNode animNode = node.SelectSingleNode("anim");
            if (animNode != null)
            {
                model.animSrc = animNode.Attributes.GetNamedItem("prefab").InnerText;
            }

            XmlNode roomReturnSrcNode = node.SelectSingleNode("returnImg");
            if (roomReturnSrcNode != null)
            {
                model.roomReturnSrc = roomReturnSrcNode.Attributes.GetNamedItem("src").InnerText;
            }
            else
            {
                model.roomReturnSrc = "";
            }

            Dictionary<string, string> narrationTable = new Dictionary<string, string>();
            XmlNodeList narrationNodeList = node.SelectNodes("narration");
            foreach (XmlNode narrationNode in narrationNodeList)
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
            XmlNodeList observeSubList = observeNode.SelectNodes("observe_sub");
            List<string> observeTexts = new List<string>();
            foreach (XmlNode observeSub in observeSubList)
            {
                observeTexts.Add(TextConverter.TranslateDescData(observeSub.InnerText));
            }

            string observeData = observeNode.InnerText;
            model.observe = TextConverter.TranslateDescData(observeData);
            model.observeList = observeTexts.ToArray();

            // inner graph
            model.nodeInfo = node.SelectNodes("graph/node");
            model.edgeInfo = node.SelectNodes("graph/edge");

            LoadCreatueStatData(model, pathInfoNode.SelectSingleNode("stat").InnerText);

            creatureTypeList.Add(model);
        }
		*/
        }
		CreatureTypeList.instance.Init (creatureTypeList.ToArray ());
	}

    public void LoadCreatureStat(XmlNode stat, CreatureTypeInfo model) {

        
        model.horrorProb = float.Parse(stat.SelectSingleNode("horrorProb").InnerText);
        model.horrorMin = float.Parse(stat.SelectSingleNode("horrorMin").InnerText);
        model.horrorMax = float.Parse(stat.SelectSingleNode("horrorMax").InnerText);
        model.physicsDmg = (int)float.Parse(stat.SelectSingleNode("physicsDmg").InnerText);
        model.mentalDmg = (int)float.Parse(stat.SelectSingleNode("mentalDmg").InnerText);
        
        model.feelingMax = (int)float.Parse(stat.SelectSingleNode("feelingMax").InnerText);
        model.feelingDownValue = float.Parse(stat.SelectSingleNode("feelingDownValue").InnerText);
        model.energyPointChange = (int)float.Parse(stat.SelectSingleNode("energyPointChange").InnerText);

        List<EnergyGenInfo> energyItems = new List<EnergyGenInfo>();
        XmlNode energyGenSection = stat.SelectSingleNode("energyGenSection");
        XmlNodeList energySections = energyGenSection.SelectNodes("section");
        foreach (XmlNode section in energySections)
        {
            int upperBound;
            float value;

            upperBound = int.Parse(section.Attributes.GetNamedItem("bound").InnerText);
            value = float.Parse(section.Attributes.GetNamedItem("gen").InnerText);

            EnergyGenInfo info = new EnergyGenInfo(upperBound, value);
            energyItems.Add(info);
        }

        model.energyGenInfo = energyItems.ToArray();

    }

    public void LoadCreatureCollectionInfo(XmlNode collection, CreatureTypeInfo model) {
        CreatureTypeInfo.ObserveTable table = model.observeTable;
        model.codeId = LoadCollectionItem(collection, "codeNo", ref table.codeNo);
        model.portraitSrc = LoadCollectionItem(collection, "portrait", ref table.portrait);
        
        model.sizeLevel = (int)float.Parse(LoadCollectionItem(collection, "size", ref table.size));
        model.weightLevel = LoadCollectionItem(collection, "weight", ref table.weight);
        model.name = LoadCollectionItem(collection, "name", ref table.name);
        //model.attackType = LoadCollectionItem(collection, "attackType", ref table.attackType);
        model.creatureType = LoadCollectionItem(collection, "creatureType", ref table.creatureType);
        model.horrorLevel = (int)float.Parse(LoadCollectionItem(collection, "horrorLevel", ref table.horrorLevel));
        model.level = LoadCollectionItem(collection, "riskLevel", ref table.riskLevel);
        model.intelligence = (int)float.Parse(LoadCollectionItem(collection, "intelligence", ref table.intelligence));
        model.physicalAttackLevel = (int)float.Parse(LoadCollectionItem(collection, "physical", ref table.physical));

        model.mentalAttackLevel = (int)float.Parse(LoadCollectionItem(collection, "mental", ref table.mental));
        model.specialSkillName = LoadCollectionItem(collection, "specialName", ref table.specialName);
        model.specialSkillInfo = LoadCollectionItem(collection, "specialInfo", ref table.specialInfo);
        model.aggressionLevel = (int)float.Parse(LoadCollectionItem(collection, "aggression", ref table.aggression));
        model.gorgeousLevel = (int)float.Parse(LoadCollectionItem(collection, "gorgeous", ref table.gorgeous));
        //reject prefer work + solution
        //end of collection info
    }

    private string LoadCollectionItem(XmlNode collection, string target , ref int level) {
        level = (int)float.Parse(collection.SelectSingleNode(target).Attributes.GetNamedItem("openLevel").InnerText);
        return collection.SelectSingleNode(target).InnerText;
    }

    public void LoadCreatueStatData(CreatureTypeInfo model, string src)
    {
		//Debug.Log ("Load stat >> " + src);
        TextAsset creatureTextAsset = Resources.Load<TextAsset>("xml/creatures/stats/" + src);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(creatureTextAsset.text);

        XmlNode node = doc.SelectSingleNode("/creature/stat");

        //model.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);
        model.name = node.SelectSingleNode("name").InnerText;
        //model.codeId = node.Attributes.GetNamedItem("codeId").InnerText;
        model.level = node.SelectSingleNode("level").InnerText;
        
		//model.attackType = node.SelectSingleNode("attackType").InnerText;
		model.attackType = CreatureAttackType.PHYSICS;

        //model.intelligence = node.Attributes.GetNamedItem("intelligence").InnerText;

        //model.stackLevel = int.Parse(node.Attributes.GetNamedItem("stackLevel").InnerText);
        //model.observeLevel = int.Parse(node.Attributes.GetNamedItem("observeLevel").InnerText);

        model.horrorProb = float.Parse(node.SelectSingleNode("horrorProb").InnerText);
        model.horrorDmg = int.Parse(node.SelectSingleNode("horrorMin").InnerText, System.Globalization.NumberStyles.Any);
        //model.horrorDmg = int.Parse(node.SelectSingleNode("horrorMax").InnerText);

		//model.attackProb = float.Parse (node.SelectSingleNode ("attackProb").InnerText);

        //model.physicsProb = float.Parse(node.SelectSingleNode("physicsProb").InnerText);
        //model.physicsDmg = int.Parse(node.SelectSingleNode("physicsMin").InnerText, System.Globalization.NumberStyles.Any);
        //model.physicsDmg = int.Parse(node.SelectSingleNode("physicsMax").InnerText);
		model.physicsDmg = (int)float.Parse(node.SelectSingleNode("physicsDmg").InnerText, System.Globalization.NumberStyles.Any);

        //model.mentalProb = float.Parse(node.SelectSingleNode("mentalProb").InnerText);
        //model.mentalDmg = int.Parse(node.SelectSingleNode("mentalMin").InnerText, System.Globalization.NumberStyles.Any);
        //model.mentalDmg = int.Parse(node.SelectSingleNode("mentalMax").InnerText);
		model.mentalDmg = (int)float.Parse(node.SelectSingleNode("mentalDmg").InnerText, System.Globalization.NumberStyles.Any);

        //model.script = node.Attributes.GetNamedItem("script").InnerText;

        /*
        if (node.Attributes.GetNamedItem("animatorScript") != null)
        {
            model.animatorScript = node.Attributes.GetNamedItem("animatorScript").InnerText;
        }*/

        model.feelingMax = int.Parse(node.SelectSingleNode("feelingMax").InnerText, System.Globalization.NumberStyles.Any);
        //model.feelingDownProb = float.Parse(node.SelectSingleNode("feelingDownProb").InnerText);
        model.feelingDownValue = float.Parse(node.SelectSingleNode("feelingDownValue").InnerText);

		model.energyPointChange = int.Parse(node.SelectSingleNode("energyPointChange").InnerText);

		List<EnergyGenInfo> energyItems = new List<EnergyGenInfo> ();
		XmlNode energyGenSection = node.SelectSingleNode ("energyGenSection");
		XmlNodeList energySections = energyGenSection.SelectNodes ("section");
		foreach (XmlNode section in energySections)
		{
			int upperBound;
			float value;

			upperBound = int.Parse (section.Attributes.GetNamedItem ("bound").InnerText);
			value = float.Parse(section.Attributes.GetNamedItem("gen").InnerText);

			EnergyGenInfo info = new EnergyGenInfo (upperBound, value);
			energyItems.Add (info);
		}

		model.energyGenInfo = energyItems.ToArray ();

		/*
        List<EnergyGenInfo> energyItems = new List<EnergyGenInfo>();

        string sectionGood = node.SelectSingleNode("energySectionGood").InnerText;
        string sectionNorm = node.SelectSingleNode("energySectionNorm").InnerText;
		*/
		/*
        List<FeelingSectionInfo> feelingSectionInfoList = new List<FeelingSectionInfo>();

        // 기분 상태 : 나쁨
        FeelingSectionInfo badFeeling = new FeelingSectionInfo();
        badFeeling.feelingState = CreatureFeelingState.BAD;
        badFeeling.section = 0;
        badFeeling.energyGenValue = float.Parse(node.SelectSingleNode("energyGenBad").InnerText);

        badFeeling.preferList = GetSkillBonusInfo(
            node.SelectSingleNode("preferSkiilsBad"),
            node.SelectSingleNode("preferValuesBad")).ToArray();
        badFeeling.rejectList = GetSkillBonusInfo(
            node.SelectSingleNode("rejectSkiilsBad"),
            node.SelectSingleNode("rejectValuesBad")).ToArray();
        feelingSectionInfoList.Add(badFeeling);

        // 기분 상태 : 보통
        if(sectionNorm != "x")
        {
            FeelingSectionInfo normFeeling = new FeelingSectionInfo();
            badFeeling.feelingState = CreatureFeelingState.NORM;
            normFeeling.section = int.Parse(sectionNorm, System.Globalization.NumberStyles.Any);
            normFeeling.energyGenValue = float.Parse(node.SelectSingleNode("energyGenNorm").InnerText);

            normFeeling.preferList = GetSkillBonusInfo(
                node.SelectSingleNode("preferSkiilsNorm"),
                node.SelectSingleNode("preferValuesNorm")).ToArray();
            normFeeling.rejectList = GetSkillBonusInfo(
                node.SelectSingleNode("rejectSkiilsNorm"),
                node.SelectSingleNode("rejectValuesNorm")).ToArray();
            feelingSectionInfoList.Add(normFeeling);
        }

        // 기분 상태 : 좋음
        FeelingSectionInfo goodFeeling = new FeelingSectionInfo();
        badFeeling.feelingState = CreatureFeelingState.GOOD;
        goodFeeling.section = int.Parse(sectionGood, System.Globalization.NumberStyles.Any);
        goodFeeling.energyGenValue = float.Parse(node.SelectSingleNode("energyGenGood").InnerText);

        goodFeeling.preferList = GetSkillBonusInfo(
            node.SelectSingleNode("preferSkiilsGood"),
            node.SelectSingleNode("preferValuesGood")).ToArray();
        goodFeeling.rejectList = GetSkillBonusInfo(
            node.SelectSingleNode("rejectSkiilsGood"),
            node.SelectSingleNode("rejectValuesGood")).ToArray();
        feelingSectionInfoList.Add(goodFeeling);

        model.feelingSectionInfo = feelingSectionInfoList.ToArray();
		*/
    }

    public List<SkillBonusInfo> GetSkillBonusInfo(XmlNode skillsNode, XmlNode valuesNode)
    {
        List<SkillBonusInfo> output = new List<SkillBonusInfo>();

        string skillsText = skillsNode.InnerText;
        string valuesText = valuesNode.InnerText;

        string[] skillNames =  skillsText.Split(',');
        string[] values = valuesText.Split(',');

        for (int i = 0; i < skillNames.Length; i++)
        {
            SkillBonusInfo info = new SkillBonusInfo();
            if (skillNames[i] == "직접")
            {
                info.attr = SkillBonusAttr.CATEGORY_TYPE;
                info.skillType = "direct";
            }
            else if (skillNames[i] == "간접")
            {
                info.attr = SkillBonusAttr.CATEGORY_TYPE;
                info.skillType = "indirect";
            }
            else if (skillNames[i] == "차단")
            {
                info.attr = SkillBonusAttr.CATEGORY_TYPE;
                info.skillType = "block";
            }
            else
            {
                info.attr = SkillBonusAttr.CATEGORY_ID;
                SkillTypeInfo skillInfo = SkillTypeList.instance.GetDataByName(skillNames[i]);
                if (skillInfo == null)
                {
                    //Debug.Log("skill not found : " + skillNames[i]);
                    continue;
                }
                info.skillId = skillInfo.id;
            }

            output.Add(info);
        }

        return output;
    }

    public void LoadPassageData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("xml/PassageList");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList nodes = doc.SelectNodes("/passage_list/passage");

        List<PassageObjectTypeInfo> passsageTypeList = new List<PassageObjectTypeInfo>();

        foreach (XmlNode node in nodes)
        {
            PassageObjectTypeInfo typeInfo = new PassageObjectTypeInfo();

            typeInfo.id = long.Parse(node.Attributes.GetNamedItem("id").InnerText);

            typeInfo.prefabSrc = node.SelectSingleNode("src").InnerText;

            passsageTypeList.Add(typeInfo);
        }

        PassageObjectTypeList.instance.Init(passsageTypeList.ToArray());
    }
    
    public void LoadCreatureResourceData()
    {
    }

    public void LoadCreatureTextData()
    {
    }

}
