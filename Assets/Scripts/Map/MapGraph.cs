using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class MapGraph
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

    private Dictionary<string, MapNode> graphNodes;
    private Dictionary<string, List<MapNode>> sefiraNodesTable;
    private Dictionary<string, List<MapNode>> additionalSefiraTable;
    private Dictionary<string, List<List<MapNode>>> deptNodeTable;
    

    private Dictionary<string, List<MapNode>> nodeAreaTable;

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

        if (sefiraNodesTable.TryGetValue(area, out output))
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
        List<MapNode> nodeList;
        if (nodeAreaTable.TryGetValue(name, out nodeList))
        {
            foreach (MapNode node in nodeList)
            {
                node.activate = true;
            }
            Notice.instance.Send(NoticeName.AreaOpenUpdate, name);
        }
    }

    public void DeactivateArea(string name)
    {
        List<MapNode> nodeList;
        if (nodeAreaTable.TryGetValue(name, out nodeList))
        {
            foreach (MapNode node in nodeList)
            {
                node.activate = false;
            }
            Notice.instance.Send(NoticeName.AreaUpdate, name, false);
        }
    }

    public void InitActivates()
    {
        foreach (KeyValuePair<string, List<MapNode>> pair in nodeAreaTable)
        {
            foreach (MapNode node in pair.Value)
            {
                node.activate = false;
            }
            Notice.instance.Send(NoticeName.AreaUpdate, pair.Key, false);
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
        Dictionary<string, List<MapNode>> nodeAreaDic = new Dictionary<string, List<MapNode>>();
        Dictionary<string, List<MapNode>> additionalSefiraDic = new Dictionary<string, List<MapNode>>();

        foreach (XmlNode areaNode in areaNodes)
        {
            List<MapNode> nodesInArea = new List<MapNode>();
            List<MapNode> sefiraNodes = new List<MapNode>();
            List<MapNode> additionalSefira = new List<MapNode>();
            string areaName = areaNode.Attributes.GetNamedItem("name").InnerText;
            
            int max = int.Parse(areaNode.Attributes.GetNamedItem("sub").InnerText);
            Sefira sefira = SefiraManager.instance.getSefira(areaName);
            sefira.initDepartmentNodeList(max);

            foreach (XmlNode nodeGroup in areaNode.ChildNodes)
            {
                if (nodeGroup.Name == "node_group")
                {
                    string groupName = "group@" + groupCount;
                    groupCount++;
                    
                    foreach (XmlNode node in nodeGroup.ChildNodes)
                    {
                        string id = node.Attributes.GetNamedItem("id").InnerText;
                        float x = float.Parse(node.Attributes.GetNamedItem("x").InnerText);
                        float y = float.Parse(node.Attributes.GetNamedItem("y").InnerText);
                        
                        XmlNode typeNode = node.Attributes.GetNamedItem("type");

                        MapNode newMapNode = new MapNode(id, new Vector2(x, y), areaName, groupName);
                       
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
                        
                        nodesInArea.Add(newMapNode);
                    }
                }
                else
                {
                    Debug.Log("this is not node_group");
                }
            }

            nodeAreaDic.Add(areaName, nodesInArea);
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

        nodeAreaTable = nodeAreaDic;
        sefiraNodesTable = sefiraNodesDic;
        additionalSefiraTable = additionalSefiraDic;

        loaded = true;

        Notice.instance.Send(NoticeName.LoadMapGraphComplete);
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
}
