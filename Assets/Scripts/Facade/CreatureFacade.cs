using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class CreatureFacade : MonoBehaviour {

	private static CreatureFacade _instance;

	public static CreatureFacade instance
	{
		get
		{
			//if(_instance == null)
				//_instance = new CreatureFacade();
			return _instance;
		}
	}

	void Awake()
	{
		_instance = this;
		Init ();
	}

	public GameObject creatureListNode;

	private List<CreatureUnit> creatureList;

	public void AddCreature(CreatureTypeInfo typeInfo, string nodeId, float x, float y)
	{
		GameObject newCreature = Prefab.LoadPrefab ("Creature1");

		CreatureUnit unit = newCreature.GetComponent<CreatureUnit> ();

        unit.transform.SetParent(creatureListNode.transform, false);

		Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();
		List<MapEdge> edgeList = new List<MapEdge> ();

		foreach(XmlNode node in typeInfo.nodeInfo)
		{
			string id = nodeId + "@" + node.Attributes.GetNamedItem("id").InnerText;
			float nodeX = x+float.Parse(node.Attributes.GetNamedItem("x").InnerText);
			float nodeY = y+float.Parse(node.Attributes.GetNamedItem("y").InnerText);

			MapNode newNode = new MapNode(id, new Vector2(nodeX, nodeY));

			XmlNode typeNode = node.Attributes.GetNamedItem("type");
			if(typeNode != null && typeNode.InnerText == "workspace")
			{
				unit.SetWorkspaceNode(newNode);
			}
			else if(typeNode != null && typeNode.InnerText == "creature")
			{
				unit.SetNode(newNode);
			}
			else if(typeNode != null && typeNode.InnerText == "entry")
			{
				MapNode entryNode = MapGraph.instance.GetNodeById(nodeId);
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

			MapNode node1 = null , node2 = null;

			if(nodeDic.TryGetValue(node1Id, out node1) == false ||
				nodeDic.TryGetValue(node2Id, out node2) == false)
			{
				Debug.Log("cannot create edge - ("+node1Id + ", " +node2Id+")");
			}

			XmlNode costNode = node.Attributes.GetNamedItem("cost");

			MapEdge edge;
			if(costNode != null)
			{
				//edge = new MapEdge(node1, node2, type);
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

		//unit.SetNode(MapGraph.instance.GetNodeById(nodeId));


		creatureList.Add (unit);

		unit.metaInfo = typeInfo;
		unit.specialSkill = typeInfo.specialSkill;
		unit.AddFeeling(typeInfo.feelingMax);

		unit.script = (CreatureBase)System.Activator.CreateInstance (System.Type.GetType(typeInfo.script));
		
		Texture2D tex = Resources.Load<Texture2D> ("Sprites/"+typeInfo.imgsrc);
		unit.spriteRenderer.sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f, 0.5f));
		unit.spriteRenderer.gameObject.transform.localScale = new Vector3 (150f/tex.width, 150f/tex.height, 1);

		GameObject creatureRoom = Prefab.LoadPrefab ("IsolateRoom");
        creatureRoom.transform.SetParent(creatureListNode.transform, false);
		IsolateRoom room = creatureRoom.GetComponent<IsolateRoom> ();
        tex = Resources.Load<Texture2D> ("Sprites/"+typeInfo.roomsrc);
        
        room.roomSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
		room.targetUnit = unit;

        tex = Resources.Load<Texture2D>("Sprites/" + typeInfo.framesrc);
        room.frameSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        room.Init();
        
		creatureRoom.transform.position = new Vector3(x, y, 0);
		room.UpdateStatus ();

		unit.room = room;
	}

	public void AddCreature(long typeId, string nodeId, float x, float y)
	{
		CreatureTypeInfo info = CreatureTypeList.instance.GetData (typeId);
		//AddCreature (info, x, y);
		AddCreature(info, nodeId, x, y);
	}

	public void Init()
	{
		creatureList = new List<CreatureUnit> ();
	}

	public CreatureUnit[] GetCreatureList()
	{
		return creatureList.ToArray();
	}
}
