using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum SefiraState
{
    NORMAL,
    NOT_ENOUGH_AGENT
}

public class CreatureManager : IObserver{

	private static CreatureManager _instance;

	public static CreatureManager instance
	{
		get
		{
			if(_instance == null)
				_instance = new CreatureManager();
			return _instance;
		}
	}

	private CreatureManager()
	{
		Init ();
	}

	public GameObject creatureListNode;

	private List<CreatureModel> creatureList;

    public List<CreatureModel> MalkuthCreature = new List<CreatureModel>();
    public List<CreatureModel> NezzachCreature = new List<CreatureModel>();
    public List<CreatureModel> HodCreature = new List<CreatureModel>();
    public List<CreatureModel> YessodCreature = new List<CreatureModel>();

    public List<SefiraState> sefiraState = new List<SefiraState>();
    
    public SefiraState malkuthState = SefiraState.NOT_ENOUGH_AGENT;
    public SefiraState nezzachState = SefiraState.NOT_ENOUGH_AGENT;
    public SefiraState hodState = SefiraState.NOT_ENOUGH_AGENT;
    public SefiraState yessodState = SefiraState.NOT_ENOUGH_AGENT;

	
    private int nextInstId = 1;

    /**
     * 새 환상체를 추가합니다.
     **/
    public void AddCreature(long metadataId, string nodeId, float x, float y, string sefiraNum)
    {
        CreatureModel model = new CreatureModel(nextInstId++);

		model.sefira = SefiraManager.instance.getSefira(sefiraNum);
		model.sefiraNum = model.sefira.indexString;

		model.position = new Vector2(x, y);

        BuildCreatureModel(model, metadataId, nodeId, x, y);

		model.AddFeeling(model.metaInfo.feelingMax / 2);

        AddCreatureInSepira(model, sefiraNum);

        RegisterCreature(model);

       
    }

    // 환상체 세피라에 배속
    public void AddCreatureInSepira(CreatureModel creature, string sepira)
    {
        if (sepira == "1")
        {
            MalkuthCreature.Add(creature);
            
        }

        else if (sepira == "2")
        {
            NezzachCreature.Add(creature);
        }

        else if (sepira == "3")
        {
            HodCreature.Add(creature);
        }

        else if (sepira == "4")
        {
            YessodCreature.Add(creature);
        }
        //Debug.Log(SefiraManager.instance.getSefira(sepira).creatureList.Count);
        SefiraManager.instance.getSefira(sepira).creatureList.Add(creature);
    }

    public List<CreatureModel> GetSelectedList(string sefira) {
        if (sefira == "1")
        {
            return MalkuthCreature;
        }

        else if (sefira == "2")
        {
            return NezzachCreature;
        }

        else if (sefira == "3")
        {
            return HodCreature;
        }

        else if (sefira == "4")
        {
            return YessodCreature;
        }
        else
            return null;
    }

    public void Update()
    {
        OnChangeAgentSefira();
    }


    
    /**
     * 환상체를 리스트에 추가합니다.
     * 
     */
    private void RegisterCreature(CreatureModel model)
    {
        creatureList.Add(model);

        Notice.instance.Observe(NoticeName.FixedUpdate, model);
        Notice.instance.Observe(NoticeName.CreatureFeelingUpdateTimer, model);
        Notice.instance.Send(NoticeName.AddCreature, model);
    }

    /**
     * 1. 환상체에 메타데이터(CreatureTypeInfo) 를 적용합니다.
     * 2. 환상체의 MapNode를 생성합니다.
     * 
     **/
    private void BuildCreatureModel(CreatureModel model, long metadataId, string nodeId, float x, float y)
    {
        CreatureTypeInfo typeInfo = CreatureTypeList.instance.GetData(metadataId);

        //Debug.Log(metadataId);

        model.metadataId = metadataId;
        model.metaInfo = typeInfo;
        model.basePosition = new Vector2(x, y);

        model.script = (CreatureBase)System.Activator.CreateInstance(System.Type.GetType(typeInfo.script));
        if(model.script != null)
            model.script.SetModel(model);
        model.entryNodeId = nodeId;

        MapNode entryNode = MapGraph.instance.GetNodeById(nodeId);
		entryNode.connectedCreature = model;

        Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();
        List<MapEdge> edgeList = new List<MapEdge>();

		MapNode outterNode = null;
		MapNode innerNode = null;

		PassageObjectModel passage = null;

		passage = new PassageObjectModel(nodeId+"@creature", entryNode.GetAreaName(), "Map/Passage/PassageEmpty");
		passage.position = new Vector3(x, y, 0);
		int doorCount = 1;

        foreach (XmlNode node in typeInfo.nodeInfo)
        {
            string id = nodeId + "@" + node.Attributes.GetNamedItem("id").InnerText;
            float nodeX = model.basePosition.x + float.Parse(node.Attributes.GetNamedItem("x").InnerText);
            float nodeY = model.basePosition.y + float.Parse(node.Attributes.GetNamedItem("y").InnerText);

			MapNode newNode = null;

            XmlNode typeNode = node.Attributes.GetNamedItem("type");
            if (typeNode != null && typeNode.InnerText == "workspace")
            {
				newNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName());
                model.SetWorkspaceNode(newNode);
            }
			else if (typeNode != null && typeNode.InnerText == "custom")
			{
				newNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName());
				model.SetCustomNode(newNode);
			}
            else if (typeNode != null && typeNode.InnerText == "creature")
            {
				newNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName());
                model.SetRoomNode(newNode);
                model.SetCurrentNode(newNode);
            }
            else if (typeNode != null && typeNode.InnerText == "entry")
            {
				newNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName());
				//string entryNodeId = id + "@entry_door";

                MapEdge edge = new MapEdge(newNode, entryNode, "door");

                edgeList.Add(edge);

                newNode.AddEdge(edge);
                entryNode.AddEdge(edge);
            }
			else if(typeNode != null && typeNode.InnerText == "outterDoor")
			{
				newNode = outterNode = new MapNode(id, new Vector2(entryNode.GetPosition().x, entryNode.GetPosition().y), entryNode.GetAreaName(), passage);

				string roomDoorId = passage.GetId() + "@" + doorCount;
				outterNode.SetClosable(true);
				DoorObjectModel outterDoor = new DoorObjectModel(roomDoorId, "DoorMiddle", passage, outterNode);
				outterDoor.position = new Vector3(outterNode.GetPosition().x, outterNode.GetPosition().y, -0.01f);
				passage.AddDoor(outterDoor);
				outterNode.SetDoor(outterDoor);
				outterDoor.Close();


				MapEdge edge = new MapEdge(newNode, entryNode, "road");

				edgeList.Add(edge);

				newNode.AddEdge(edge);
				entryNode.AddEdge(edge);

				if(innerNode != null)
				{
					MapEdge doorEdge = new MapEdge(outterNode, innerNode, "door", 0);

					outterDoor.Connect (innerNode.GetDoor ());

					edgeList.Add(doorEdge);

					outterNode.AddEdge(doorEdge);
					innerNode.AddEdge(doorEdge);
				}
			}
			else if(typeNode != null && typeNode.InnerText == "innerDoor")
			{
				newNode = innerNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName(), passage);

				string roomDoorId = passage.GetId() + "@" + doorCount;
				innerNode.SetClosable(true);
				DoorObjectModel innerDoor = new DoorObjectModel(roomDoorId, "DoorIsolate", passage, innerNode);
				innerDoor.position = new Vector3(innerNode.GetPosition().x, innerNode.GetPosition().y, -0.01f);
				passage.AddDoor(innerDoor);
				innerNode.SetDoor(innerDoor);
				innerDoor.Close();

				if(outterNode != null)
				{
					MapEdge doorEdge = new MapEdge(innerNode, outterNode, "door", 0);

					innerDoor.Connect (outterNode.GetDoor ());

					edgeList.Add(doorEdge);

					innerNode.AddEdge(doorEdge);
					outterNode.AddEdge(doorEdge);
				}
					
			}

            nodeDic.Add(id, newNode);
        }

        foreach (XmlNode node in typeInfo.edgeInfo)
        {
            string node1Id = nodeId + "@" + node.Attributes.GetNamedItem("node1").InnerText;
            string node2Id = nodeId + "@" + node.Attributes.GetNamedItem("node2").InnerText;

            string type = node.Attributes.GetNamedItem("type").InnerText;

            MapNode node1 = null, node2 = null;

            if (nodeDic.TryGetValue(node1Id, out node1) == false ||
                nodeDic.TryGetValue(node2Id, out node2) == false)
            {
                Debug.Log("cannot create edge - (" + node1Id + ", " + node2Id + ")");
            }

            XmlNode costNode = node.Attributes.GetNamedItem("cost");

            MapEdge edge;
            if (costNode != null)
            {
                edge = new MapEdge(node1, node2, type, float.Parse(costNode.InnerText));
            }
            else
            {
                edge = new MapEdge(node1, node2, type);
            }
            edgeList.Add(edge);

            node1.AddEdge(edge);
            node2.AddEdge(edge);
        }

		MapGraph.instance.RegisterPassage (passage);

        if (model.script != null)
            model.script.OnInit();
    }

	public void Init()
	{
		creatureList = new List<CreatureModel> ();

        for (int i = 0; i < SefiraManager.instance.sefiraList.Count; i++) {
            SefiraState state = new SefiraState();
            state = SefiraState.NOT_ENOUGH_AGENT;
            this.sefiraState.Add(state);
        }

        Notice.instance.Observe(NoticeName.ChangeAgentSefira_Late, this);
	}

	public CreatureModel[] GetCreatureList()
	{
		return creatureList.ToArray();
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

    public void ClearCreatue()
    {
        foreach (CreatureModel model in creatureList)
        {
            Notice.instance.Remove(NoticeName.FixedUpdate, model);
            Notice.instance.Remove(NoticeName.CreatureFeelingUpdateTimer, model);
        }
        CreatureLayer.currentLayer.ClearAgent();

        creatureList = new List<CreatureModel>();
    }

    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        dic.Add("nextInstId", nextInstId);

        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

        foreach (CreatureModel creature in creatureList)
        {
            Dictionary<string, object> creatureData = creature.GetSaveData();
            list.Add(creatureData);
        }

        dic.Add("creatureList", list);

        return dic;
    }

    public void LoadData(Dictionary<string, object> dic)
    {
        ClearCreatue();

        TryGetValue(dic, "nextInstId", ref nextInstId);

        List<Dictionary<string, object>> agentDataList = new List<Dictionary<string, object>>();
        TryGetValue(dic, "creatureList", ref agentDataList);
        foreach (Dictionary<string, object> data in agentDataList)
        {
            int creatureId = 0;

            TryGetValue(data, "instanceId", ref creatureId);

            CreatureModel model = new CreatureModel(creatureId);
            model.LoadData(data);

            BuildCreatureModel(model, model.metadataId, model.entryNodeId, model.basePosition.x, model.basePosition.y);

            RegisterCreature(model);
        }
    }

    public void creatureStateWorse(string sepiraName)
    {
        if(sepiraName == "Malkuth")
        {
            foreach(CreatureModel creature in MalkuthCreature)
            {
                creature.sefiraEmpty = true;
            }
        }

        else if(sepiraName == "Nezzach")
        {
            foreach (CreatureModel creature in NezzachCreature)
            {
                creature.sefiraEmpty = true;
            }
        }

        else if (sepiraName == "Yessod")
        {
            foreach (CreatureModel creature in YessodCreature)
            {
                creature.sefiraEmpty = true;
            }
        }

        else if (sepiraName == "Hod")
        {
            foreach (CreatureModel creature in HodCreature)
            {
                creature.sefiraEmpty = true;
            }
        }
    }

    public void creatureStateNormal(string sepiraName)
    {
        if (sepiraName == "Malkuth")
        {
            foreach (CreatureModel creature in MalkuthCreature)
            {
                creature.sefiraEmpty = false;
            }
        }

        else if (sepiraName == "Nezzach")
        {
            foreach (CreatureModel creature in NezzachCreature)
            {
                creature.sefiraEmpty = false;
            }
        }

        else if (sepiraName == "Yessod")
        {
            foreach (CreatureModel creature in YessodCreature)
            {
                creature.sefiraEmpty = false;
            }
        }

        else if (sepiraName == "Hod")
        {
            foreach (CreatureModel creature in HodCreature)
            {
                creature.sefiraEmpty = false;
            }
        }
    }

    private void OnChangeAgentSefira()
    {
        foreach (Sefira sefira in SefiraManager.instance.sefiraList) {
            SefiraState target = sefiraState[sefira.index-1];
            if (sefira.agentList.Count == 0)
            {
                target = SefiraState.NOT_ENOUGH_AGENT;
                creatureStateWorse(sefira.name);
            }
            else {
                target = SefiraState.NORMAL;
                creatureStateNormal(sefira.name);
            }
        }
        /*
        if (AgentManager.instance.malkuthAgentList.Count == 0)
        {
            malkuthState = SefiraState.NOT_ENOUGH_AGENT;
            creatureStateWorse("Malkuth");
        }
        else
        {
            malkuthState = SefiraState.NORMAL;
            creatureStateNormal("Malkuth");
        }

        if (AgentManager.instance.nezzachAgentList.Count == 0)
        {
            nezzachState = SefiraState.NOT_ENOUGH_AGENT;
            creatureStateWorse("Nezzach");
        }
        else
        {
            nezzachState = SefiraState.NORMAL;
            creatureStateNormal("Nezzach");
        }

        if (AgentManager.instance.hodAgentList.Count == 0)
        {
            hodState = SefiraState.NOT_ENOUGH_AGENT;
            creatureStateWorse("Hod");
        }
        else
        {
            hodState = SefiraState.NORMAL;
            creatureStateNormal("Hod");
        }

        if (AgentManager.instance.yesodAgentList.Count == 0)
        {
            yessodState = SefiraState.NOT_ENOUGH_AGENT;
            creatureStateWorse("Yessod");
        }
        else
        {
            yessodState = SefiraState.NORMAL;
            creatureStateNormal("Yessod");
        }
         */
    }

	public CreatureModel[] GetNearSuppressedCreatures(MovableObjectNode node)
	{
		List<CreatureModel> output = new List<CreatureModel>();
		foreach (CreatureModel creature in creatureList)
		{
			if (creature.state != CreatureState.SUPPRESSED)
				continue;
			
			Vector3 dist = node.GetCurrentViewPosition () - creature.GetMovableNode ().GetCurrentViewPosition ();
			if (node.GetPassage () == creature.GetMovableNode ().GetPassage () &&
				dist.sqrMagnitude <= 3) {
				output.Add(creature);
			}
		}
		return output.ToArray();
	}

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.ChangeAgentSefira_Late)
        {
            OnChangeAgentSefira();
        }
    }

    public void OnStageStart() {
        foreach (CreatureModel model in this.creatureList) {
            if (model != null && model.script != null && model.script.skill != null) {
                model.script.skill.OnStageStart();
            }
        }
    }
}
