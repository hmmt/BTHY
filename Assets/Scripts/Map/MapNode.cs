using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {

	private string id;

    private bool _activate;
    private string areaName;
    private string groupName;
	private List<MapEdge> edges;
    private List<MapNode> zNodes;
	private Vector3 pos;

    public bool isTemporary = false;
    public bool isZBase = false;

    public MapSefiraArea area; // 소속된 area

    public bool activate
    {
        get { return _activate; }
        set { _activate = value; }
    }

    public MapNode(string id, Vector3 pos, string areaName)
    {
        this.id = id;
        this.pos = pos;
        this.areaName = areaName;
        this.groupName = "NoName";

        _activate = true;
        edges = new List<MapEdge>();
        zNodes = new List<MapNode>();
    }

    public MapNode(string id, Vector3 pos, string areaName, string groupName)
	{
		this.id = id;
		this.pos = pos;
        this.areaName = areaName;
        this.groupName = groupName;

        _activate = true;
		edges = new List<MapEdge>();
        zNodes = new List<MapNode>();
	}

    public void AddZNode(MapNode node)
    {
        zNodes.Add(node);
    }

    public MapNode[] GetZNodes()
    {
        return zNodes.ToArray();
    }

	public void AddEdge(MapEdge edge)
	{
		edges.Add (edge);
	}

    public void RemoveEdge(MapEdge edge)
    {
        edges.Remove(edge);
    }

	public Vector3 GetPosition()
	{
		return pos;
	}

	public string GetId()
	{
		return id;
	}

    public string GetAreaName()
    {
        return areaName;
    }

    public string GetGroupName()
    {
        return groupName;
    }

	public MapEdge[] GetEdges()
	{
		return edges.ToArray ();
	}

    public MapEdge GetEdgeByNode(MapNode node)
    {
        if (node == this)
            return null;
        foreach (MapEdge edge in edges)
        {
            if (edge.node1 == node || edge.node2 == node)
                return edge;
        }
        return null;
    }
}
