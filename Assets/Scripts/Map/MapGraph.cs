using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class MapGraph : MonoBehaviour
{

    private static MapGraph _instance;
    public static MapGraph instance { get { return _instance; } }

    private Dictionary<string, MapNode> graphNodes;

    private List<MapEdge> edges;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        LoadMap();
    }

    public MapNode GetNodeById(string id)
    {
        MapNode output = null;
        graphNodes.TryGetValue(id, out output);
        return output;
    }

    public string GetRandomRestPoint()
    {
        return Random.Range(1001001, 1001003).ToString();
    }

    public int GetRandomPanicRoamingPoint()
    {
        return 100;
    }

    public void LoadMap()
    {
        //StreamReader sr = new StreamReader (Application.dataPath + "/Resources/xml/MapNodeList.xml");

        TextAsset textAsset = Resources.Load<TextAsset>("xml/MapNodeList");

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList nodes = doc.SelectNodes("/node_list/node");

        Dictionary<string, MapNode> nodeDic = new Dictionary<string, MapNode>();

        foreach (XmlNode node in nodes)
        {
            string id = node.Attributes.GetNamedItem("id").InnerText;
            float x = float.Parse(node.Attributes.GetNamedItem("x").InnerText);
            float y = float.Parse(node.Attributes.GetNamedItem("y").InnerText);

            nodeDic.Add(id, new MapNode(id, new Vector2(x, y)));

            // 게임 뷰에 위치 표시
            GameObject nodePoint = Prefab.LoadPrefab("NodePoint");

            nodePoint.transform.SetParent(gameObject.transform);
            nodePoint.transform.localPosition = new Vector3(x, y, 0);
        }

        textAsset = Resources.Load<TextAsset>("xml/MapEdgeList");

        doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        nodes = doc.SelectNodes("/edge_list/edge");

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


            // 게임 뷰에 위치 표시
            GameObject edgeLine = Prefab.LoadPrefab("EdgeLine");
            edgeLine.transform.SetParent(gameObject.transform);
            edgeLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(node1.GetPosition().x, node1.GetPosition().y, 0));
            edgeLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(node2.GetPosition().x, node2.GetPosition().y, 0));
        }

        graphNodes = nodeDic;
        //edges = edgeList.ToArray();
        edges = edgeList;
    }
}
