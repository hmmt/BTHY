﻿using UnityEngine;
using System.Collections;

public class MapEdge {
	public MapNode node1;
	public MapNode node2;

	public float cost;

	public string type;
    public string name;
	public bool activated;

    // temp edge
    public bool isTemporary = false;
    public MapEdge baseEdge = null;

	public MapEdge(MapNode node1, MapNode node2, string type)
	{
		this.node1 = node1;
		this.node2 = node2;
		this.type = type;


		this.cost = Vector3.Distance(node1.GetPosition(), node2.GetPosition());
	}

	public MapEdge(MapNode node1, MapNode node2, string type, float cost)
	{
		this.node1 = node1;
		this.node2 = node2;
		this.type = type;

		this.cost = cost;
	}

	public MapNode ConnectedNode(MapNode node)
	{
        if (node1.activate == false || node2.activate == false)
        {
            return null;
        }
		if(node == node1)
		{
			return node2;
		}
		if(node == node2)
		{
			return node1;
		}
		return null;
	}

	public MapNode ConnectedNodeIgoreActivate(MapNode node)
	{
		if(node == node1)
		{
			return node2;
		}
		if(node == node2)
		{
			return node1;
		}
		return null;
	}

    public MapNode GetGoalNode(int direction)
    {
        if (direction == 1)
        {
            return node2;
        }
        else
        {
            return node1;
        }
    }
}
