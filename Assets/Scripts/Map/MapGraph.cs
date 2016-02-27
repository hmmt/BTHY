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
        int groupCount = 1;

        if (loaded)
            return;
        TextAsset textAsset = Resources.Load<TextAsset>("xml/MapNodeList");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList areaNodes = doc.SelectNodes("/node_list/area");

        Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();
        Dictionary<string, List<MapNode>> sefiraNodesDic = new Dictionary<string, List<MapNode>>();
        Dictionary<string, List<MapNode>> additionalSefiraDic = new Dictionary<string, List<MapNode>>();

        Dictionary<string, MapSefiraArea> mapAreaDic = new Dictionary<string, MapSefiraArea>();
        Dictionary<string, PassageObjectModel> passageDic = new Dictionary<string, PassageObjectModel>();

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
                    XmlNode passageTypeIdNode = attrs.GetNamedItem("typeId");
                    XmlNode passageXNode = attrs.GetNamedItem("x");
                    XmlNode passageYNode = attrs.GetNamedItem("y");
                    PassageObjectModel passage = null;
                    if (passageTypeIdNode != null)
                    {
                        long passageTypeId = long.Parse(passageTypeIdNode.InnerText);
                        float x = 0, y = 0;
                        if (passageXNode != null) x = float.Parse(passageXNode.InnerText);
                        if (passageYNode != null) y = float.Parse(passageYNode.InnerText);
                        passage = new PassageObjectModel(groupName, areaName, PassageObjectTypeList.instance.GetData(passageTypeId));
                        passage.position = new Vector3(x, y, 0);
                    }
                    
                    
                    foreach (XmlNode node in nodeGroup.ChildNodes)
                    {
                        string id = node.Attributes.GetNamedItem("id").InnerText;
                        float x = float.Parse(node.Attributes.GetNamedItem("x").InnerText);
                        float y = float.Parse(node.Attributes.GetNamedItem("y").InnerText);
                        
                        XmlNode typeNode = node.Attributes.GetNamedItem("type");

                        MapNode newMapNode = new MapNode(id, new Vector2(x, y), areaName, passage);
                       
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

        textAsset = Resources.Load<TextAsset>("xml/MapEdgeList");

        doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList nodes = doc.SelectNodes("/edge_list/edge");

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


    private void FixedUpdate()
    {
        foreach (PassageObjectModel passage in passageTable.Values)
        {
            passage.FixedUpdate();
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
