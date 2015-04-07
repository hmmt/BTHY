using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {

	private string id;

	private List<MapEdge> edges;
	private Vector2 pos;

	public MapNode(string id, Vector2 pos)
	{
		this.id = id;
		this.pos = pos;

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

	public MapEdge[] GetEdges()
	{
		return edges.ToArray ();
	}
}
