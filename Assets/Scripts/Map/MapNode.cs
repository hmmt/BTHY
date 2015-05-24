using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {

	private string id;

    private bool _activate;
    private string areaName;
	private List<MapEdge> edges;
	private Vector2 pos;

    public bool activate
    {
        get { return _activate; }
        set { _activate = value; }
    }



    public MapNode(string id, Vector2 pos, string areaName)
	{
		this.id = id;
		this.pos = pos;
        this.areaName = areaName;

        _activate = true;
		edges = new List<MapEdge>();
	}


	public void AddEdge(MapEdge edge)
	{
		edges.Add (edge);
	}

	public Vector2 GetPosition()
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

	public MapEdge[] GetEdges()
	{
		return edges.ToArray ();
	}
}
