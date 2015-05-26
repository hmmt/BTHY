using UnityEngine;
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

	public void AddCreature(long typeId, string nodeId, float x, float y)
	{
		CreatureTypeInfo info = CreatureTypeList.instance.GetData (typeId);
        AddCreature(info, nodeId, x, y);
	}

    private int instId = 1;
    public void AddCreature(CreatureTypeInfo typeInfo, string nodeId, float x, float y)
    {
        CreatureModel unit = new CreatureModel(instId++);

        MapNode entryNode = MapGraph.instance.GetNodeById(nodeId);
        Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();
        List<MapEdge> edgeList = new List<MapEdge>();

        foreach (XmlNode node in typeInfo.nodeInfo)
        {
            string id = nodeId + "@" + node.Attributes.GetNamedItem("id").InnerText;
            float nodeX = x + float.Parse(node.Attributes.GetNamedItem("x").InnerText);
            float nodeY = y + float.Parse(node.Attributes.GetNamedItem("y").InnerText);

            MapNode newNode = new MapNode(id, new Vector2(nodeX, nodeY), entryNode.GetAreaName());

            XmlNode typeNode = node.Attributes.GetNamedItem("type");
            if (typeNode != null && typeNode.InnerText == "workspace")
            {
                unit.SetWorkspaceNode(newNode);
            }
            else if (typeNode != null && typeNode.InnerText == "creature")
            {
                unit.SetNode(newNode);
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

        unit.metaInfo = typeInfo;
        unit.specialSkill = typeInfo.specialSkill;
        unit.AddFeeling(typeInfo.feelingMax);

        unit.script = (CreatureBase)System.Activator.CreateInstance(System.Type.GetType(typeInfo.script));

        unit.position = new Vector2(x, y);

        creatureList.Add(unit);

        Notice.instance.Remove(NoticeName.FixedUpdate, unit);
        Notice.instance.Observe(NoticeName.CreatureFeelingUpdateTimer, unit);
        Notice.instance.Send(NoticeName.AddCreature, unit);
    }

	public void Init()
	{
		creatureList = new List<CreatureModel> ();
	}

	public CreatureModel[] GetCreatureList()
	{
		return creatureList.ToArray();
	}
}
