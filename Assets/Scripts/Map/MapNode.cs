using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {

	private string id;

    private bool _activate;
    private string areaName;
	private List<MapEdge> edges;
    private List<MapNode> zNodes;
	private Vector3 pos;

    public bool isTemporary = false;
    public bool isZBase = false;

    public bool closed = false;

	public CreatureModel connectedCreature = null;

    private DoorObjectModel door = null;
    private PassageObjectModel attachedPassage;

    private bool closable;

	public float scaleFactor = 1.0f;

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
        this.attachedPassage = null;

        _activate = true;
        edges = new List<MapEdge>();
        zNodes = new List<MapNode>();
    }

    public MapNode(string id, Vector3 pos, string areaName, PassageObjectModel attachedPassage)
	{
		this.id = id;
		this.pos = pos;
        this.areaName = areaName;
        this.attachedPassage = attachedPassage;

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

    public void SetClosable(bool b)
    {
        closable = b;
    }

    public void SetDoor(DoorObjectModel door)
    {
        this.door = door;
    }

    public DoorObjectModel GetDoor()
    {
        return door;
    }

    public bool IsClosable()
    {
        return closable;
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

    public PassageObjectModel GetAttachedPassage()
    {
        return attachedPassage;
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
