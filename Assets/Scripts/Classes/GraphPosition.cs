using UnityEngine;
using System.Collections;

public class GraphPosition{

	public MapNode currentNode;

	public MapEdge currentEdge;
	public int edgeDirection;
	public float edgePosRate; // 0~1

	public GraphPosition(MapNode currentNode)
	{
		this.currentNode = currentNode;
	}

	public GraphPosition(MapEdge currentEdge, int direction, float rate)
	{
		this.currentEdge = currentEdge;
		this.edgeDirection = direction;
		this.edgePosRate = rate;
	}

	private bool GetCurrentViewPosition(out Vector2 output)
	{
		output = new Vector2();
		if(currentNode != null)
		{
			Vector2 pos = currentNode.GetPosition();
			output.x = pos.x;
			output.y = pos.y;
			return true;
		}
		else if(currentEdge != null)
		{
			MapNode node1 = currentEdge.node1;
			MapNode node2 = currentEdge.node2;
			Vector2 pos1 = node1.GetPosition();
			Vector2 pos2 = node2.GetPosition();

			if(edgeDirection == 1)
			{
				output.x = Mathf.Lerp(pos1.x, pos2.x, edgePosRate);
				output.y = Mathf.Lerp(pos1.y, pos2.y, edgePosRate);
			}
			else
			{
				output.x = Mathf.Lerp(pos1.x, pos2.x, 1-edgePosRate);
				output.y = Mathf.Lerp(pos1.y, pos2.y, 1-edgePosRate);
			}
			return true;
		}
		return false;
	}
}
