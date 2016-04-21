using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class MapGraph : IObserver
{
    private static MapGraph _instance;
    public static MapGraph instance
    {
        get
        {
            if (_instance == null)
                _instance = new MapGraph();
            return _instance;
        }
    }

    // 모든 노드
    private Dictionary<string, MapNode> graphNodes;
    
    // 세피라 휴식 공간 노드
    private Dictionary<string, List<MapNode>> sefiraCoreNodesTable;
    private Dictionary<string, List<MapNode>> additionalSefiraTable;
    private Dictionary<string, List<List<MapNode>>> deptNodeTable;

    // 세피라 영역 노드
    private Dictionary<string, MapSefiraArea> mapAreaTable;

    // 통로
    private Dictionary<string, PassageObjectModel> passageTable;

    private List<MapEdge> edges;

	private List<ElevatorPassageModel> elevatorList;

    public bool loaded { private set; get; }

    public MapGraph()
    {
        loaded = false;
        
    }

    public MapNode GetNodeById(string id)
    {
        MapNode output = null;
        graphNodes.TryGetValue(id, out output);
        return output;
    }

    //
    public MapNode GetCreatureRoamingPoint()
    {
        string[] arr = PlayerModel.instance.GetOpenedAreaList();

        string selectedSefira = arr[Random.Range(0, arr.Length)];
        return GetSepiraNodeByRandom(selectedSefira);
    }

    public MapNode GetSepiraNodeByRandom(string area)
    {

        MapNode[] nodes = GetSefiraNodes(area);
        if (nodes.Length == 0)
            return GetNodeById("sefira-malkuth-5");

        return nodes[Random.Range(0, nodes.Length)];
    }

    public MapNode GetSefiraDeptNodes(string area) {
        MapNode[] nodes = GetAdditionalSefira(area);
        if (nodes.Length == 0)
            return GetNodeById("sefira-malkuth-5");
        return nodes[Random.Range(0, nodes.Length)];
    }

    public MapNode[] GetSefiraNodes(string area)
    {
        List<MapNode> output;

        if (sefiraCoreNodesTable.TryGetValue(area, out output))
        {
            return output.ToArray();
        }

        return new MapNode[]{ };
    }

	public PassageObjectModel GetSefiraPassage(string area)
	{
		MapNode[] node = GetSefiraNodes (area);
		return node [0].GetAttachedPassage ();
	}

    public MapNode[] GetAdditionalSefira(string area) {
        List<MapNode> output;
        if (additionalSefiraTable.TryGetValue(area, out output)) {
            return output.ToArray();
        }
        return new MapNode[] { };
    }

    public void ActivateArea(string name)
    {
        MapSefiraArea sefira;
        if (mapAreaTable.TryGetValue(name, out sefira))
        {
            sefira.ActivateArea();
        }
        
    }

    public void DeactivateArea(string name)
    {
        MapSefiraArea sefira;
        if (mapAreaTable.TryGetValue(name, out sefira))
        {
            sefira.DeactivateArea();
        }
    }

    public void InitActivates()
    {
        foreach (MapSefiraArea sefira in mapAreaTable.Values)
        {
            sefira.InitActivates();
        }
    }

	public void LoadMap()
	{
		if (loaded)
			return;
		/*
		TextAsset textAsset = Resources.Load<TextAsset>("xml/MapNodeList");
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(textAsset.text);

		XmlNode nodeXml = doc.SelectSingleNode ("/node_list");

		textAsset = Resources.Load<TextAsset>("xml/MapEdgeList");
		doc = new XmlDocument();
		doc.LoadXml(textAsset.text);

		XmlNode edgeXml = doc.SelectSingleNode ("/edge_list");

		*/

		TextAsset textAsset = Resources.Load<TextAsset>("xml/MapGraph3");
		//TextAsset textAsset = Resources.Load<TextAsset>("xml/TrailerTest4");
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(textAsset.text);

		XmlNode nodeXml = doc.SelectSingleNode ("/map_graph/node_list");
		XmlNode edgeXml = doc.SelectSingleNode ("/map_graph/edge_list");
		LoadMap (nodeXml, edgeXml);
	}
	public void LoadMap(XmlNode nodeRoot, XmlNode edgeRoot)
    {
		int groupCount = 1;

		/*
        XmlDocument doc = new XmlDocument();
		doc.LoadXml(xmlText);
		*/

		XmlNodeList areaNodes = nodeRoot.SelectNodes("area");

        Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();
        Dictionary<string, List<MapNode>> sefiraNodesDic = new Dictionary<string, List<MapNode>>();
        Dictionary<string, List<MapNode>> additionalSefiraDic = new Dictionary<string, List<MapNode>>();

        Dictionary<string, MapSefiraArea> mapAreaDic = new Dictionary<string, MapSefiraArea>();
        Dictionary<string, PassageObjectModel> passageDic = new Dictionary<string, PassageObjectModel>();

		List<MapNode> elevatorNodes = new List<MapNode> ();

        foreach (XmlNode areaNode in areaNodes)
        {
            MapSefiraArea mapArea = new MapSefiraArea();
            List<MapNode> sefiraNodes = new List<MapNode>();
            List<MapNode> additionalSefira = new List<MapNode>();
            string areaName = areaNode.Attributes.GetNamedItem("name").InnerText;
            mapArea.sefiraName = areaName;
            
            int max = int.Parse(areaNode.Attributes.GetNamedItem("sub").InnerText);
            Sefira sefira = SefiraManager.instance.getSefira(areaName);
            sefira.initDepartmentNodeList(max);

            foreach (XmlNode nodeGroup in areaNode.ChildNodes)
            {
                if (nodeGroup.Name == "node_group")
                {
                    string groupName = "group@" + groupCount;
                    groupCount++;

                    XmlAttributeCollection attrs = nodeGroup.Attributes;
                    //XmlNode passageTypeIdNode = attrs.GetNamedItem("typeId");
					XmlNode passageSrcNode = attrs.GetNamedItem("src");
                    XmlNode passageXNode = attrs.GetNamedItem("x");
                    XmlNode passageYNode = attrs.GetNamedItem("y");
                    PassageObjectModel passage = null;
					if (passageSrcNode != null)
                    {
                        //long passageTypeId = long.Parse(passageTypeIdNode.InnerText);
                        float x = 0, y = 0;
                        if (passageXNode != null) x = float.Parse(passageXNode.InnerText);
                        if (passageYNode != null) y = float.Parse(passageYNode.InnerText);
						passage = new PassageObjectModel(groupName, areaName, passageSrcNode.InnerText);
                        passage.position = new Vector3(x, y, 0);
                    }
                    
                    
                    foreach (XmlNode node in nodeGroup.ChildNodes)
                    {
                        string id = node.Attributes.GetNamedItem("id").InnerText;
                        float x = float.Parse(node.Attributes.GetNamedItem("x").InnerText);
                        float y = float.Parse(node.Attributes.GetNamedItem("y").InnerText);
                        
                        XmlNode typeNode = node.Attributes.GetNamedItem("type");

                        MapNode newMapNode = new MapNode(id, new Vector2(x, y), areaName, passage);

						XmlNode scaleAttr = node.Attributes.GetNamedItem ("scale");
						if (scaleAttr != null)
							newMapNode.scaleFactor = float.Parse (scaleAttr.InnerText);

						XmlNode elevatorAttr = node.Attributes.GetNamedItem ("elevator");
						if (elevatorAttr != null) {

							ElevatorPassageModel elevatorModel = new ElevatorPassageModel (newMapNode);

							MapNode eNode1 = new MapNode ("elevator1-" + id, new Vector3 (-1, 0, 0), areaName);
							MapNode eNode2 = new MapNode ("elevator2-" + id, new Vector3 (-0.5f, 0, 0), areaName);
							MapNode eNode3 = new MapNode ("elevator3-" + id, new Vector3 (0, 0, 0), areaName);
							MapNode eNode4 = new MapNode ("elevator4-" + id, new Vector3 (0.5f, 0, 0), areaName);
							MapNode eNode5 = new MapNode ("elevator5-" + id, new Vector3 (1, 0, 0), areaName);
							eNode1.scaleFactor = 0.8f;
							eNode2.scaleFactor = 0.8f;
							eNode3.scaleFactor = 0.8f;
							eNode4.scaleFactor = 0.8f;
							eNode5.scaleFactor = 0.8f;
							elevatorModel.AddNode(eNode1);
							elevatorModel.AddNode(eNode2);
							elevatorModel.AddNode(eNode3);
							elevatorModel.AddNode(eNode4);
							elevatorModel.AddNode(eNode5);

							newMapNode.AttachElevator (elevatorModel);

							elevatorNodes.Add (newMapNode);
							//MapNode node1 = new MapNode ("elevator1", new Vector3 (0, 0, 0), areaName);
						}


                       
                        newMapNode.activate = false;

                        nodeDic.Add(id, newMapNode);
                        if (typeNode != null && typeNode.InnerText == "sefira")
                        {
                            sefiraNodes.Add(newMapNode);
                        }
                        else if(typeNode != null && typeNode.InnerText == "dept"){
                            additionalSefira.Add(newMapNode);

                            string[] totalString;
                            totalString = Regex.Split(id, "-");
                            int index = int.Parse(totalString[1]);
                            sefira.departmentList[index].Add(newMapNode);
                        }

						MapNode optionalNode = null;
                        XmlNodeList optionList = node.SelectNodes("option");
                        int doorCount = 1;
                        foreach (XmlNode optionNode in optionList)
                        {
							if (optionNode.InnerText == "room")
							{
							}
                        }
                        XmlNode doorNode = node.SelectSingleNode("door");
						// TEMP
						if (id.Contains ("sefira-malkuth-1")) {
							string doorId = passage.GetId() + "@" + doorCount;
							newMapNode.SetClosable(true);
							DoorObjectModel door = new DoorObjectModel(doorId, "DoorLeft", passage, newMapNode);
							door.position = new Vector3(newMapNode.GetPosition().x,
								newMapNode.GetPosition().y, -0.01f);
							passage.AddDoor(door);
							newMapNode.SetDoor(door);
							door.Close();
						} else if(id.Contains("sefira-malkuth-9")){
							string doorId = passage.GetId() + "@" + doorCount;
							newMapNode.SetClosable(true);
							DoorObjectModel door = new DoorObjectModel(doorId, "DoorRight", passage, newMapNode);
							door.position = new Vector3(newMapNode.GetPosition().x,
								newMapNode.GetPosition().y, -0.01f);
							passage.AddDoor(door);
							newMapNode.SetDoor(door);
							door.Close();
						}

                        if (doorNode != null)
                        {
                            string doorId = passage.GetId() + "@" + doorCount;
                            newMapNode.SetClosable(true);
                            DoorObjectModel door = new DoorObjectModel(doorId, doorNode.InnerText, passage, newMapNode);
                            door.position = new Vector3(newMapNode.GetPosition().x,
                                newMapNode.GetPosition().y, -0.01f);
                            passage.AddDoor(door);
                            newMapNode.SetDoor(door);
                            door.Close();
                        }

                        if(passage != null)
                            passage.AddNode(newMapNode);
                        mapArea.AddNode(newMapNode);
						if (optionalNode != null)
							mapArea.AddNode (optionalNode);
                    }
                    if (passage != null)
                        passageDic.Add(groupName, passage);
                }
				else if(nodeGroup.Name == "#comment")
				{
					// skip
				}
                else
                {
					Debug.Log("this is not node_group >>> "+nodeGroup.Name);
                }
            }

            mapAreaDic.Add(areaName, mapArea);
            sefiraNodesDic.Add(areaName, sefiraNodes);
            additionalSefiraDic.Add(areaName, additionalSefira);
        }


		XmlNodeList nodes = edgeRoot.SelectNodes("edge");

        List<MapEdge> edgeList = new List<MapEdge>();

        foreach (XmlNode node in nodes)
        {
            string node1Id = node.Attributes.GetNamedItem("node1").InnerText;
            string node2Id = node.Attributes.GetNamedItem("node2").InnerText;

            string type = node.Attributes.GetNamedItem("type").InnerText;

            MapNode node1, node2;

            if (nodeDic.TryGetValue(node1Id, out node1) == false ||
                nodeDic.TryGetValue(node2Id, out node2) == false)
            {
                Debug.Log("cannot create edge - (" + node1Id + ", " + node2Id + ")");
                continue;
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

        graphNodes = nodeDic;
        edges = edgeList;

        mapAreaTable = mapAreaDic;
        sefiraCoreNodesTable = sefiraNodesDic;
        additionalSefiraTable = additionalSefiraDic;

        passageTable = passageDic;

		// elevator

		elevatorList = new List<ElevatorPassageModel> ();

		foreach(MapNode elevatorNode in elevatorNodes)
		{
			MapEdge[] elevatorEdges = elevatorNode.GetEdges ();

			if (elevatorEdges.Length > 1) {
				List<MapNode> upFloorList = new List<MapNode> ();
				List<MapNode> downFloorList = new List<MapNode> ();

				foreach(MapEdge e in elevatorEdges)
				{
					MapNode exitNode = e.ConnectedNodeIgoreActivate (elevatorNode);
					if (exitNode.GetPosition ().y > elevatorNode.GetPosition ().y)
						upFloorList.Add (exitNode);
					else
						downFloorList.Add (exitNode);
				}

				if (upFloorList.Count > 0 && downFloorList.Count > 0)
				{
					ElevatorPassageModel elevator = elevatorNode.GetElevator ();

					elevator.AddFloorInfo (upFloorList.ToArray (), new Vector3 (0, 6, 0) + elevatorNode.GetPosition ());
					elevator.AddFloorInfo (downFloorList.ToArray (), new Vector3 (0, -7, 0) + elevatorNode.GetPosition ());

					elevatorList.Add (elevator);
				}
				else
				{
					elevatorNode.AttachElevator (null);
				}
				/*
				if (elevatorEdges [0].ConnectedNodeIgoreActivate (elevatorNode).GetPosition ().y > elevatorNode.GetPosition ().y)
				{
					ElevatorPassageModel elevator = elevatorNode.GetElevator ();


					elevator.AddFloorInfo (elevatorEdges [1].ConnectedNodeIgoreActivate (elevatorNode), new Vector3 (0, -6, 0) + elevatorNode.GetPosition());
					elevator.AddFloorInfo (elevatorEdges [0].ConnectedNodeIgoreActivate (elevatorNode), new Vector3 (0, 6, 0) + elevatorNode.GetPosition());

					elevatorList.Add (elevator);
				} else {
					ElevatorPassageModel elevator = elevatorNode.GetElevator ();


					elevator.AddFloorInfo (elevatorEdges [0].ConnectedNodeIgoreActivate (elevatorNode), new Vector3 (0, -6, 0) + elevatorNode.GetPosition());
					elevator.AddFloorInfo (elevatorEdges [1].ConnectedNodeIgoreActivate (elevatorNode), new Vector3 (0, 6, 0) + elevatorNode.GetPosition());

					elevatorList.Add (elevator);
				}
				*/

			} else {
				elevatorNode.AttachElevator (null);
			}
		}
        
        ///

        loaded = true;

        // Awake에서 호출되는데 괜찮나?
        Notice.instance.Observe(NoticeName.FixedUpdate, this);
        Notice.instance.Send(NoticeName.LoadMapGraphComplete);
    }
    public void RegisterPassageObject(PassageObjectModel model)
    {
        passageTable.Add(model.GetId(), model);

        Notice.instance.Send(NoticeName.AddPassageObject, model);
    }

    public MapNode[] GetGraphNodes()
    {
        MapNode[] output = new MapNode[graphNodes.Count];
        int i=0;
        foreach (KeyValuePair<string, MapNode> kv in graphNodes)
        {
            output[i++] = kv.Value;
        }
        return output;
    }

    public MapEdge[] GetGraphEdges()
    {
        return edges.ToArray();
    }

	public void RegisterPassage(PassageObjectModel passage)
	{
		passageTable.Add (passage.GetId (), passage);
		Notice.instance.Send (NoticeName.AddPassageObject, passage);
	}

    public PassageObjectModel[] GetPassageObjectList()
    {
        return new List<PassageObjectModel>(passageTable.Values).ToArray();
    }

	public ElevatorPassageModel[] GetElevatorPassageList()
	{
		return elevatorList.ToArray ();
	}


    private void FixedUpdate()
    {
        foreach (PassageObjectModel passage in passageTable.Values)
        {
            passage.FixedUpdate();
        }

		foreach (ElevatorPassageModel elevator in elevatorList)
		{
			elevator.OnFixedUpdate ();
		}
    }

    public void OnNotice(string name, params object[] param)
    {
        if (name == NoticeName.FixedUpdate)
        {
            FixedUpdate();
        }
    }
}
