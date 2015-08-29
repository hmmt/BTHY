﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class CreatureManager {

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

	
    private int nextInstId = 1;

    /**
     * 새 환상체를 추가합니다.
     **/
    public void AddCreature(long metadataId, string nodeId, float x, float y, string sefiraNum)
    {
        CreatureModel model = new CreatureModel(nextInstId++);

        BuildCreatureModel(model, metadataId, nodeId, x, y);

        model.sefiraNum = sefiraNum;
        model.AddFeeling(model.metaInfo.feelingMax / 2);

        model.position = new Vector2(x, y);

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

        model.metadataId = metadataId;
        model.metaInfo = typeInfo;
        model.specialSkill = typeInfo.specialSkill;
        model.basePosition = new Vector2(x, y);
        //Debug.Log(typeInfo.script);
        model.script = (CreatureBase)System.Activator.CreateInstance(System.Type.GetType(typeInfo.script));
        if(model.script != null)
            model.script.SetModel(model);
        model.baseNodeId = nodeId;

        MapNode entryNode = MapGraph.instance.GetNodeById(nodeId);
        Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();
        List<MapEdge> edgeList = new List<MapEdge>();

        foreach (XmlNode node in typeInfo.nodeInfo)
        {
            string id = nodeId + "@" + node.Attributes.GetNamedItem("id").InnerText;
            float nodeX = model.basePosition.x + float.Parse(node.Attributes.GetNamedItem("x").InnerText);
            float nodeY = model.basePosition.y + float.Parse(node.Attributes.GetNamedItem("y").InnerText);

            MapNode newNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName());

            XmlNode typeNode = node.Attributes.GetNamedItem("type");
            if (typeNode != null && typeNode.InnerText == "workspace")
            {
                model.SetWorkspaceNode(newNode);
            }
            else if (typeNode != null && typeNode.InnerText == "creature")
            {
                model.SetRoomNode(newNode);
                model.SetCurrentNode(newNode);
            }
            else if (typeNode != null && typeNode.InnerText == "entry")
            {
                MapEdge edge = new MapEdge(newNode, entryNode, "door");

                edgeList.Add(edge);

                newNode.AddEdge(edge);
                entryNode.AddEdge(edge);
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
    }

	public void Init()
	{
		creatureList = new List<CreatureModel> ();
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

            BuildCreatureModel(model, model.metadataId, model.baseNodeId, model.basePosition.x, model.basePosition.y);

            RegisterCreature(model);
        }
    }
}
