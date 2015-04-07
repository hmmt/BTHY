using UnityEngine;
using System.Collections.Generic;

public class Elevator : MonoBehaviour {

	public class ElevatorRequest
	{
		int nodeId;
		int direction; // 0 : head to last, 1 : head to first, 2 : both

		public ElevatorRequest(int nodeId, int direiction)
		{
			this.nodeId = nodeId;
			this.direction = direction;
		}
	}

	private MapNode[] path;

	private List<string> goalNodes;
	private bool direction;

	// path finding2
	private MapNode currentNode;

	private MapEdge currentEdge;
	private int edgeDirection;
	private float edgePosRate; // 0~1


	private float waitingTime = 0;

	private int GetPathIndex(string nodeId)
	{
		for(int i=0; i<path.Length; i++)
		{
			if(path[i].GetId() == nodeId)
			{
				return i; 
			}
		}
		return -1;
	}

	private bool IsGoalNode(string nodeId)
	{
		for(int i=0; i<goalNodes.Count; i++)
		{
			if(goalNodes[i] == nodeId)
				return true;
		}
		return true;
	}

	private bool ExistNextGoalNode(int index, bool direction)
	{
		if(direction)
		{
			for(int i=0; i<goalNodes.Count; i++)
			{
				if(GetPathIndex(goalNodes[i]) > index)
					return true;
			}
		}
		else
		{
			for(int i=0; i<goalNodes.Count; i++)
			{
				if(GetPathIndex(goalNodes[i]) < index)
					return true;
			}
		}
		return false;
	}

	public bool DoorOpened()
	{
		return false;
	}

	void Start () {
	
	}

	void FixedUpdate()
	{
		if(goalNodes.Count > 0)
		{
			if(currentNode != null)
			{
				if(waitingTime > 0)
				{
					waitingTime -= Time.deltaTime;
				}
				else
				{
					int currentIndex = GetPathIndex(currentNode.GetId());

					if(direction)
					{
						bool found = false;
						for(int i=0; i<goalNodes.Count; i++)
						{
							if(GetPathIndex(goalNodes[i]) > currentIndex)
							{
								found = true;
								break;
							}
							found = false;
						}
					}
					else
					{
						bool found = false;
						for(int i=0; i<goalNodes.Count; i++)
						{
							if(GetPathIndex(goalNodes[i]) > currentIndex)
							{
								found = true;
								break;
							}
							found = false;
						}
					}
				}
			}
			else if(currentEdge != null)
			{
				do
				{
					int nextNodeIndex;
					if(direction) // head to last index
					{

						int nodeIndex1 = GetPathIndex(currentEdge.node1.GetId());
						int nodeIndex2 = GetPathIndex(currentEdge.node2.GetId());
						
						if(nodeIndex1 == -1 && nodeIndex2 == -1)
							break;
						nextNodeIndex = Mathf.Max(nodeIndex1, nodeIndex2);

						bool found = false;
						for(int i=0; i<goalNodes.Count; i++)
						{
							if(GetPathIndex(goalNodes[i]) > nextNodeIndex)
							{
								found = true;
								break;
							}
						}

						if(!found && goalNodes.Count > 0)
						{
							direction = !direction;
						}
					}
					else // head to first index
					{
						int nodeIndex1 = GetPathIndex(currentEdge.node1.GetId());
						int nodeIndex2 = GetPathIndex(currentEdge.node2.GetId());

						if(nodeIndex1 == -1 && nodeIndex2 == -1)
							break;
						if(nodeIndex1 == -1 || nodeIndex2 == -1)
							nextNodeIndex = Mathf.Max(nodeIndex1, nodeIndex2);
						else
							nextNodeIndex = Mathf.Min(nodeIndex1, nodeIndex2);
						
						bool found = false;
						for(int i=0; i<goalNodes.Count; i++)
						{
							if(GetPathIndex(goalNodes[i]) < nextNodeIndex)
							{
								found = true;
								break;
							}
						}

						if(!found && goalNodes.Count > 0)
						{
							direction = !direction;
						}
					}
					// move

					edgePosRate = Mathf.Clamp(edgePosRate + Time.deltaTime / currentEdge.cost * (direction ? 1 : -1), 0f, 1f);

					if(direction && edgePosRate >= 1)
					{
						currentNode = currentEdge.node2;
						currentEdge = null;

						if(IsGoalNode(currentNode.GetId()))
						{
							goalNodes.Remove(currentNode.GetId());
							waitingTime = 0.5f;
						}
					}
					else if(!direction && edgePosRate <= 0)
					{
						currentNode = currentEdge.node1;
						currentEdge = null;

						if(IsGoalNode(currentNode.GetId()))
						{
							goalNodes.Remove(currentNode.GetId());
							waitingTime = 0.5f;
						}
					}
				} while(false);

			}
		}
	}
	
	void Update () {
	
	}
/*
	private int GetNextGoalNode()
	{
	}
*/
	public void MoveTo(string nodeId)
	{
		if(goalNodes.Contains(nodeId))
			return;
		foreach(MapNode node in path)
		{
			if(node.GetId() == nodeId)
			{
				goalNodes.Add(nodeId);
				break;
			}
		}
	}
}
