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

    private bool init = false;

    private float defaultZ = -0.01f;

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
            nodePoint.transform.localPosition = new Vector3(node.GetPosition().x, node.GetPosition().y, defaultZ);
        }

        foreach (MapEdge e in edges)
        {
            // 게임 뷰에 위치 표시
            GameObject edgeLine = Prefab.LoadPrefab("EdgeLine");
            edgeLine.transform.SetParent(gameObject.transform, false);
            edgeLine.transform.localPosition = new Vector3(0, 0, 0);
            edgeLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(e.node1.GetPosition().x, e.node1.GetPosition().y, defaultZ));
            edgeLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(e.node2.GetPosition().x, e.node2.GetPosition().y, defaultZ));
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
