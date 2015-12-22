using UnityEngine;
using System.Collections.Generic;

public class MapGraphDebugView : MonoBehaviour, IObserver {
    private static MapGraphDebugView _instance;
    public static MapGraphDebugView instance
    {
        get
        {
            return _instance;
        }
    }

    private bool debugOn = true;
    private bool init = false;

    private float defaultZ = -10;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        if (MapGraph.instance.loaded && init == false)
            UpdateView();
    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.LoadMapGraphComplete, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.LoadMapGraphComplete, this);
    }

    public void UpdateView()
    {
        if (debugOn == false)
        {
            init = true;
            return;
        }
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        MapNode[] nodes = MapGraph.instance.GetGraphNodes();
        MapEdge[] edges = MapGraph.instance.GetGraphEdges();
        foreach (MapNode node in nodes)
        {
            // 게임 뷰에 위치 표시
            GameObject nodePoint = Prefab.LoadPrefab("NodePoint");

            nodePoint.transform.SetParent(gameObject.transform, false);
            nodePoint.transform.localPosition = new Vector3(node.GetPosition().x, node.GetPosition().y, defaultZ + node.GetPosition().z);
        }

        foreach (MapEdge e in edges)
        {
            // 게임 뷰에 위치 표시
            GameObject edgeLine = Prefab.LoadPrefab("EdgeLine");
            
            edgeLine.transform.SetParent(gameObject.transform, false);
            edgeLine.transform.localPosition = new Vector3(0, 0, 0);
            edgeLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(e.node1.GetPosition().x, e.node1.GetPosition().y, defaultZ + e.node1.GetPosition().z));
            edgeLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(e.node2.GetPosition().x, e.node2.GetPosition().y, defaultZ + e.node2.GetPosition().z));
            if (e.type == "door") {
                edgeLine.GetComponent<LineRenderer>().SetColors(Color.green, Color.green);
            }
        }

        init = true;
    }

    public void OnNotice(string name, params object[] param)
    {
        if (name == NoticeName.LoadMapGraphComplete)
        {
            UpdateView();
        }
    }
}
