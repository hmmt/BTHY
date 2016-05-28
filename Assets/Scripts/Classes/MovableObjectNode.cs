using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathMoveBy
{
	public UnitDirection direction;
	public float value;
}

public enum MovableState
{
    MOVE,
    STOP,
    WAIT
}

public enum PassType
{
	NONE,
	SHIELDBEARER,
}

public enum EdgeDirection
{
	FORWARD,
	BACKWARD
}

public class MovableObjectNode {
	private UnitModel model;

	private List<PassType> unpassableList;

    private MovableState state;

    private MapNode lastNode; // for debug

    private MapNode _currentNode;
    private MapEdge _currentEdge;

	private UnitDirection unitDirection = UnitDirection.LEFT;

	private ElevatorPassageModel currentElevator;

	public MapNode currentNode
	{
		get{ return _currentNode; }
	}
	public MapEdge currentEdge
	{
		get{ return _currentEdge; }
	}

	private PassageObjectModel currentPassage;

    public float currentZValue;


    public float edgePosRate; // 0~1
	public EdgeDirection edgeDirection; // 1 : node1 에서 node2로 이동중
                              // 0 : node2 에서 node1로 이동중
	// MoveTo
    private PathResult pathInfo;
    private float edgePosRateGoal; // 목표 edge의 edgePosRate
	private MapNode destinationNode = null;
	private MovableObjectNode destinationNode2 = null;

    private int pathIndex;

	// MoveBy
	private PathMoveBy pathMoveBy = null;
	private float moveDistance;

	public float currentScale = 1.0f;

    // 지울 예정
    public MovableObjectNode()
    {
		unpassableList = new List<PassType> ();
    }
	public MovableObjectNode(UnitModel model)
    {
        this.model = model;
		unpassableList = new List<PassType> ();
    }

	public static Vector3 GetViewPositionInEdge(MapEdge edge, EdgeDirection edgeDirection, float edgePosRate)
	{
		Vector3 output = new Vector3(0, 0, 0);
		MapNode node1 = edge.node1;
		MapNode node2 = edge.node2;
		Vector3 pos1 = node1.GetPosition();
		Vector3 pos2 = node2.GetPosition();

		if (edgeDirection == EdgeDirection.FORWARD)
		{
			output.x = Mathf.Lerp(pos1.x, pos2.x, edgePosRate);
			output.y = Mathf.Lerp(pos1.y, pos2.y, edgePosRate);
			output.z = Mathf.Lerp(pos1.z, pos2.z, edgePosRate);
		}
		else
		{
			output.x = Mathf.Lerp(pos1.x, pos2.x, 1 - edgePosRate);
			output.y = Mathf.Lerp(pos1.y, pos2.y, 1 - edgePosRate);
			output.z = Mathf.Lerp(pos1.z, pos2.z, 1 - edgePosRate);
		}

		return output;
	}
    public Vector3 GetCurrentViewPosition()
    {
        Vector3 output = new Vector3(0, 0, 0);

        if (currentNode != null)
        {
            Vector3 pos = currentNode.GetPosition();
            output.x = pos.x;
            output.y = pos.y;
            output.z = pos.z;
        }
        else if (currentEdge != null)
        {
			output = GetViewPositionInEdge (currentEdge, edgeDirection, edgePosRate);
        }
        return output;
    }

    // 현재 있는 노드가 passage에 속해 있으면 그 passage 반환
    public PassageObjectModel GetPassage()
    {
		return currentPassage;
    }

	public void AddUnpassableType(PassType pass)
	{
		if(unpassableList.Contains(pass) == false)
			unpassableList.Add (pass);
	}


	bool CheckPassable(MapEdge edge, EdgeDirection edgeDir, float oldEdgePosRate, float newEdgePosRate)
	{
		Vector3 oldPos = GetViewPositionInEdge (edge, edgeDir, Mathf.Clamp01(oldEdgePosRate));
		Vector3 newPos = GetViewPositionInEdge (edge, edgeDir, Mathf.Clamp01(newEdgePosRate));


		foreach(PassType passType in unpassableList)
		{
			if (passType == PassType.SHIELDBEARER)
			{
				AgentModel[] agents = AgentManager.instance.GetAgentList ();
				List<AgentModel> shieldbearers = new List<AgentModel> ();

				foreach (AgentModel agent in agents)
				{
					if (agent.weapon == AgentWeapon.SHIELD)
					{
						shieldbearers.Add (agent);
					}
				}

				foreach (AgentModel agent in shieldbearers)
				{
					Vector3 agentPos = agent.GetCurrentViewPosition ();

					float agentLeft = agentPos.x - 1f;
					float agentRight = agentPos.x + 1f;

					if (agentLeft <= newPos.x && newPos.x <= agentRight)
						return false;
				}
			}
		}
		return true;
	}

	public bool SetMovableByPosition(Vector3 position)
	{
		if (currentPassage != null)
		{
			MapNode[] nodeList = currentPassage.GetNodeList ();
		}
		return false;
	}

    /**
     * movement의 속도로 목적지까지 Time.delta 시간 동안 이동합니다.
     * 
     * 매 프레임 마다 호출해야 합니다.
     * 
     */
    public void ProcessMoveNode(float movement)
    {
		ProcessMoveByDistance (Time.deltaTime * movement);
    }

	void ProcessMoveByDistance(float distance)
	{
		float edgeCost = -1;
		float deltaRate = -1;
		float oldPosRate = -1;
		while(true)
		{
			// 문에 대한 처리
			if (currentNode != null)
			{
				if (currentNode.GetDoor() != null)
					currentNode.GetDoor().OnObjectPassed();
			}
			else if (currentEdge != null)
			{
				if (currentEdge.node1.GetDoor() != null)
					currentEdge.node1.GetDoor().OnObjectPassed();

				if (currentEdge.node2.GetDoor() != null)
					currentEdge.node2.GetDoor().OnObjectPassed();
			}
			//if (pathInfo != null)
			if(state == MovableState.MOVE)
			{
				// 현재 node 위에 있는 경우
				if (pathMoveBy != null)
				{
					unitDirection = pathMoveBy.direction;

					if (moveDistance < pathMoveBy.value)
					{
						if(currentNode != null)
						{
							MapEdge selectedEdge = MoveBy_GetNextEdge ();

							if (selectedEdge != null)
							{
								if (selectedEdge.node1 == currentNode)
								{
									edgeDirection = EdgeDirection.FORWARD;
								}
								else
								{
									edgeDirection = EdgeDirection.BACKWARD;
								}
								edgePosRate = 0;
								UpdateNodeEdge (null, selectedEdge);

								continue;
							}
							else
							{
								StopMoving ();
							}
						}
						else if(currentEdge != null)
						{
							edgeCost = currentEdge.cost;
							deltaRate = distance / edgeCost;

							if (CheckPassable (currentEdge, edgeDirection, edgePosRate, edgePosRate + deltaRate) == false)
							{
								StopMoving ();
								return;
							}

							moveDistance += distance;


							edgePosRate += deltaRate;

							//
							float remainRate = 0;
							if (edgePosRate >= 1f)
							{
								remainRate = edgePosRate - 1;

								if (edgeDirection == EdgeDirection.FORWARD)
									UpdateNodeEdge (currentEdge.node2, null);
								else
									UpdateNodeEdge (currentEdge.node1, null);
							}

							if (moveDistance >= pathMoveBy.value)
								StopMoving ();
							else if(remainRate > 0f)
							{
								distance = remainRate * edgeCost;
								continue;
							}
								
						}
						else
						{
							Debug.Log ("invalid");
						}
					}

				}
				else if(pathInfo != null)
				{
					if (currentNode != null)
					{
						if (pathIndex >= pathInfo.pathEdges.Length)
						{
							StopMoving ();
						}
						else
						{
							MapEdge nextEdge = pathInfo.pathEdges[pathIndex];
							EdgeDirection nextDirection = pathInfo.edgeDirections[pathIndex];
							MapNode nextGoalNode = nextEdge.GetGoalNode(nextDirection);

							// 길이 막혀있을 경우
							if (nextGoalNode.closed)
							{
								if (nextGoalNode.GetDoor() != null)
								{
									// null 체크는 임시. 나중에 model이 무조건 null이 아니게 변경 예정.
									if (model != null)
									{
										if (model.CanOpenDoor())
										{
											InteractWithDoor(nextGoalNode.GetDoor());
										}
									}
								}
								else
								{
									StopMoving();
								}
							}
							else if(nextGoalNode.GetElevator() != null)
							{
								if(pathIndex+1 < pathInfo.pathEdges.Length)
								{
									MapEdge destEdge = pathInfo.pathEdges[pathIndex+1];
									EdgeDirection destDirection = pathInfo.edgeDirections[pathIndex+1];
									MapNode destGoalNode = destEdge.GetGoalNode(destDirection);

									EnterElevator (nextGoalNode, destGoalNode);
								}
								else
								{
									Debug.Log ("Elevator.. .....");
								}
							}
							else
							{
								edgeDirection = nextDirection;
								edgePosRate = 0;

								UpdateNodeEdge (null, nextEdge);

								if (float.IsInfinity (deltaRate)) {
									Debug.LogError ("1");
								}

								//continue;

								ProcessMoveByDistance(distance);
							}
						}
					}
					else if (currentEdge != null) // 현재 edge 위에 있는 경우
					{
						if (pathInfo.pathEdges != null)
						{
							edgeCost = currentEdge.cost;
							deltaRate = distance / edgeCost;
							oldPosRate = edgePosRate;

							if (deltaRate < 0) {
								Debug.LogError ("1");
							}

							if (CheckPassable (currentEdge, edgeDirection, edgePosRate, edgePosRate + deltaRate) == false)
							{
								StopMoving ();
								return;
							}

							edgePosRate += deltaRate;

							if (edgeDirection == EdgeDirection.FORWARD)
							{
								if(currentEdge.node1.GetPosition().x < currentEdge.node2.GetPosition().x)
								{
									unitDirection = UnitDirection.RIGHT;
								}
								else if(currentEdge.node1.GetPosition().x > currentEdge.node2.GetPosition().x)
								{
									unitDirection = UnitDirection.LEFT;
								}
							}
							else
							{
								if(currentEdge.node1.GetPosition().x > currentEdge.node2.GetPosition().x)
								{
									unitDirection = UnitDirection.RIGHT;
								}
								else if(currentEdge.node1.GetPosition().x < currentEdge.node2.GetPosition().x)
								{
									unitDirection = UnitDirection.LEFT;
								}
							}


							if (pathIndex >= pathInfo.pathEdges.Length - 1) // 마지막 노드
							{
								// 목표 지점에 도착
								if (edgePosRate >= edgePosRateGoal)
								{
									edgePosRate = edgePosRateGoal;
									if (edgePosRateGoal == 1) // edge 중간이 아니라 노드로 이동
									{
										if (edgeDirection == EdgeDirection.FORWARD)
											UpdateNodeEdge (currentEdge.node2, null);
										else
											UpdateNodeEdge (currentEdge.node1, null);
									}
									StopMoving ();
								}
							}
							else
							{
								// 다음 노드에 도착
								if (edgePosRate >= 1)
								{
									float remainRate = edgePosRate - 1;

									if (edgeDirection == EdgeDirection.FORWARD)
										UpdateNodeEdge (currentEdge.node2, null);
									else
										UpdateNodeEdge (currentEdge.node1, null);

									edgePosRate = 0;
									pathIndex++;

									//distance = remainRate * edgeCost;

									if (float.IsInfinity (deltaRate)) {
										Debug.LogError ("1");
									}
									//continue;
									ProcessMoveByDistance(remainRate * edgeCost);
								}
							}
						}
					}
				}
			}

			/*
			if (currentNode != null)
			{
				currentScale = currentNode.scaleFactor;
				lastNode = currentNode;
			}
			*/
			break;
		}

		if (float.IsNaN (edgePosRate)) {
			Debug.LogError ("aaa");
		}
	}

	MapEdge MoveBy_GetNextEdge()
	{
		if (currentNode == null)
			return null;

		List<MapEdge> edgeList = new List<MapEdge> ();

		float nodeX = currentNode.GetPosition ().x;
		foreach (MapEdge edge in currentNode.GetEdges())
		{
			if (edge.ConnectedNode (currentNode).GetPosition ().x < nodeX && pathMoveBy.direction == UnitDirection.LEFT)
			{
				edgeList.Add (edge);
			}
			else if (edge.ConnectedNode (currentNode).GetPosition ().x > nodeX && pathMoveBy.direction == UnitDirection.RIGHT)
			{
				edgeList.Add (edge);
			}
		}

		// 왼쪽 노드 하나만 선정
		MapEdge selectedEdge = null;
		foreach (MapEdge edge in edgeList)
		{
			if (edge.type == "door")
				continue;

			Vector3 dist = edge.ConnectedNode (currentNode).GetPosition () - currentNode.GetPosition ();
			dist.Normalize ();
			if(Mathf.Abs(dist.y / dist.magnitude) < 0.2f)
			{
				selectedEdge = edge;
				break;
			}
		}

		return selectedEdge;
	}

	bool CheckPassInNode()
	{
		return true;
	}

    public bool Equal(MovableObjectNode src)
    {
        if (currentEdge == src.currentEdge && edgePosRate == src.edgePosRate && edgeDirection == src.edgeDirection)
        {
            return true;
        }
        if (currentNode == src.currentNode)
        {
            return true;
        }
        return false;
    }
    // edge 위에 있을 때도 통합할 수 있는 타입 필요
    public MapNode GetCurrentNode()
    {
        return currentNode;
    }
    public void SetCurrentNode(MapNode node)
    {
        pathInfo = null;
        state = MovableState.STOP;
        //currentNode = node;
        //currentEdge = null;
		UpdateNodeEdge (node, null);
    }
	public void SetCurrentEdge(MapEdge srcEdge, float srcEdgePosRate, EdgeDirection srcDirection)
	{
		pathInfo = null;
		state = MovableState.STOP;

		edgePosRate = srcEdgePosRate;
		edgeDirection = srcDirection;
		UpdateNodeEdge (null, srcEdge);
	}

	public UnitDirection GetDirection()
	{
		return unitDirection;
	}
	public void SetDirection(UnitDirection direction)
	{
		unitDirection = direction;
	}
    public void Assign(MovableObjectNode src)
    {
        //currentEdge = src.currentEdge;
        //currentNode = src.currentNode;
		UpdateNodeEdge (src.currentNode, src.currentEdge);
        if (src.pathInfo != null)
        {
            pathInfo = new PathResult(
				(MapEdge[])src.pathInfo.pathEdges.Clone(),
				(EdgeDirection[])src.pathInfo.edgeDirections.Clone(),
				src.pathInfo.totalCost);
        }
        else
            pathInfo = null;
        state = src.state;
        pathIndex = src.pathIndex;

        edgePosRate = src.edgePosRate; // 0~1

        edgeDirection = src.edgeDirection;

    }
    public MapEdge GetCurrentEdge()
    {
        return currentEdge;
    }
	public EdgeDirection GetEdgeDirection()
    {
        return edgeDirection;
    }

    public bool IsMoving()
    {
        return state == MovableState.MOVE;
        //return pathInfo != null;
    }

    public void Wait()
    {
        state = MovableState.WAIT;
    }
    public void StopMoving()
    {
        pathInfo = null;
		pathMoveBy = null;

        state = MovableState.STOP;
        if (currentNode != null && currentNode.isTemporary)
        {
            //Debug.Log("23");
        }
        else if (currentEdge != null && currentEdge.isTemporary)
        {
			/*
            MapNode tempNode = null;
            MapNode realNode = null;
            MapEdge baseEdge = currentEdge.baseEdge; // 임시 edge와 겹쳐있는 실제 edge


            if (currentEdge.node1.isTemporary)
            {
                tempNode = currentEdge.node1;
                realNode = currentEdge.node2;

                edgePosRate = ((1 - edgePosRate) * currentEdge.cost) / baseEdge.cost;
                if (baseEdge.node2 == realNode)
                {
                    edgeDirection = 0;
                }
                else
                {
                    edgeDirection = 1;
                }
                //currentEdge = baseEdge;
				UpdateNodeEdge(null, baseEdge);
            }
            else if (currentEdge.node2.isTemporary)
            {
                // 임시 edge 생성 시 node1에서 node2로 이동하게 하고 있으므로 들어올 일 없음.
                Debug.Log("StopMoving() : INVALID state1");
            }
            else
            {
                Debug.Log("StopMoving() : INVALID state2");
            }
            */
        }
    }

    private void InteractWithDoor(DoorObjectModel door)
    {
        model.InteractWithDoor(door);
    }

    // TODO : 같은 node_group에 있는 애들만 포함해야 함
    public bool CheckInRange(MovableObjectNode other)
    {
        return CheckInRange(other, 3);
    }
    public bool CheckInRange(MovableObjectNode other, float range)
    {
		float distance = GetDistance (other, range);

        return distance != -1 && distance < range;
    }

	public float GetDistance(MapNode other, float limit)
	{
		MovableObjectNode node = new MovableObjectNode ();
		node.SetCurrentNode (other);
		return GetDistance (node, limit);
	}
	public float GetDistance(MovableObjectNode other, float limit)
	{
		float distance = -1;
		if (currentNode != null && other.currentNode != null)
		{
			distance = GraphAstar.Distance(currentNode, other.currentNode, limit+5);
		}
		else if (currentNode == null && currentEdge != null && other.currentNode != null)
		{
			MapNode tempNode = new MapNode("-1", GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
			MapEdge tempEdge1 = new MapEdge(tempNode, currentEdge.node1, currentEdge.type);
			MapEdge tempEdge2 = new MapEdge(tempNode, currentEdge.node2, currentEdge.type);

			tempNode.AddEdge(tempEdge1);
			tempNode.AddEdge(tempEdge2);

			distance = GraphAstar.Distance(tempNode, other.currentNode, limit+5);
		}
		else if (currentNode != null && other.currentNode == null && other.currentEdge != null)
		{
			MapNode tempNode = new MapNode("-1", other.GetCurrentViewPosition(), other.currentEdge.node1.GetAreaName());
			MapEdge tempEdge1 = new MapEdge(tempNode, other.currentEdge.node1, other.currentEdge.type);
			MapEdge tempEdge2 = new MapEdge(tempNode, other.currentEdge.node2, other.currentEdge.type);

			tempNode.AddEdge(tempEdge1);
			tempNode.AddEdge(tempEdge2);

			distance = GraphAstar.Distance(currentNode, tempNode, limit+5);
		}
		else if (currentNode == null && currentEdge != null
			&& other.currentNode == null && other.currentEdge != null)
		{
			MapNode leftTempNode = new MapNode("-1", GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
			MapEdge leftTempEdge1 = new MapEdge(leftTempNode, currentEdge.node1, currentEdge.type);
			MapEdge leftTempEdge2 = new MapEdge(leftTempNode, currentEdge.node2, currentEdge.type);

			leftTempNode.AddEdge(leftTempEdge1);
			leftTempNode.AddEdge(leftTempEdge2);

			MapNode rightTempNode = new MapNode("-1", other.GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
			MapEdge rightTempEdge1 = new MapEdge(other.currentEdge.node1, rightTempNode, currentEdge.type);
			MapEdge rightTempEdge2 = new MapEdge(other.currentEdge.node2, rightTempNode, currentEdge.type);

			other.currentEdge.node1.AddEdge(rightTempEdge1);
			other.currentEdge.node1.AddEdge(rightTempEdge2);

			distance = GraphAstar.Distance(leftTempNode, rightTempNode, limit+5);

			other.currentEdge.node1.RemoveEdge(rightTempEdge1);
			other.currentEdge.node1.RemoveEdge(rightTempEdge2);
		}
		else
		{
			Debug.Log("Current State invalid");
		}

		return distance;
	}

    public void MoveToMovableNode(MovableObjectNode targetNode)
    {
		StopMoving ();
        if (targetNode.currentNode != null)
        {
            MoveToNode(targetNode.currentNode);
            return;
        }

        if (currentNode != null)
        {
            MapNode tempNode = new MapNode("-1", GetCurrentViewPosition(), targetNode.currentEdge.node1.GetAreaName());
            MapEdge tempEdge1 = new MapEdge(targetNode.currentEdge.node1, tempNode, targetNode.currentEdge.type);
            MapEdge tempEdge2 = new MapEdge(targetNode.currentEdge.node2, tempNode, targetNode.currentEdge.type);

            tempNode.isTemporary = true;
            tempEdge1.isTemporary = true;
            tempEdge1.baseEdge = targetNode.currentEdge;
            tempEdge2.isTemporary = true;
            tempEdge2.baseEdge = targetNode.currentEdge;


            //tempNode.AddEdge(tempEdge1);
            //tempNode.AddEdge(tempEdge2);
            targetNode.currentEdge.node1.AddEdge(tempEdge1);
            targetNode.currentEdge.node2.AddEdge(tempEdge2);

            PathResult result = GraphAstar.SearchPath(currentNode, tempNode);

            MapEdge[] searchedPath = result.pathEdges;
			float rateGoal = 0;

            if (searchedPath.Length > 0)
            {
                MapEdge lastEdge = searchedPath[searchedPath.Length-1];
                if (lastEdge.node1 == targetNode.currentEdge.node1) // current
                {
                    searchedPath[searchedPath.Length - 1] = targetNode.currentEdge;
					result.edgeDirections [searchedPath.Length - 1] = EdgeDirection.FORWARD;

					if (targetNode.edgeDirection == EdgeDirection.FORWARD)
						rateGoal = targetNode.edgePosRate;
                    else
						rateGoal = 1 - targetNode.edgePosRate;
                }
                else if (lastEdge.node1 == targetNode.currentEdge.node2)
                {
                    searchedPath[searchedPath.Length - 1] = targetNode.currentEdge;
					result.edgeDirections [searchedPath.Length - 1] = EdgeDirection.BACKWARD;

					if (targetNode.edgeDirection == EdgeDirection.FORWARD)
						rateGoal = 1 - targetNode.edgePosRate;
                    else
						rateGoal = targetNode.edgePosRate;
                }
                else
                {
                    Debug.LogError("UNKNOWN ERROR : ???");
                }
            }
            targetNode.currentEdge.node1.RemoveEdge(tempEdge1);
            targetNode.currentEdge.node2.RemoveEdge(tempEdge2);

			edgePosRateGoal = rateGoal;
			pathInfo = result;
			state = MovableState.MOVE;
			pathIndex = 0;
        }
        else if (currentEdge != null)
        {
            if (currentEdge == targetNode.currentEdge)
            {
                float aPos, bPos;

				PathResult pathResult = new PathResult(new MapEdge[1], new EdgeDirection[1], currentEdge.cost); // cost??

				if (edgeDirection == EdgeDirection.FORWARD)
                    aPos = edgePosRate;
                else
                    aPos = 1 - edgePosRate;

				if (targetNode.edgeDirection == EdgeDirection.FORWARD)
                    bPos = targetNode.edgePosRate;
                else
                    bPos = 1 - targetNode.edgePosRate;

                if (aPos > bPos)
                {
					edgeDirection = EdgeDirection.BACKWARD;
                    edgePosRate = 1 - aPos;
                    edgePosRateGoal = 1 - bPos;
                }
				else
                {
					edgeDirection = EdgeDirection.FORWARD;
                    edgePosRate = aPos;
                    edgePosRateGoal = bPos;
                }
				pathInfo = pathResult;

				pathInfo.pathEdges[0] = currentEdge;
				pathInfo.edgeDirections[0] = edgeDirection;
				pathIndex = 0;
				state = MovableState.MOVE;
            }
            else
            {
                MapNode tempNode = new MapNode("-1", GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
                MapEdge tempEdge1 = new MapEdge(tempNode, currentEdge.node1, currentEdge.type);
                MapEdge tempEdge2 = new MapEdge(tempNode, currentEdge.node2, currentEdge.type);

                tempNode.isTemporary = true;
                tempEdge1.isTemporary = true;
                tempEdge1.baseEdge = currentEdge;
                tempEdge2.isTemporary = true;
                tempEdge2.baseEdge = currentEdge;

                tempNode.AddEdge(tempEdge1);
                tempNode.AddEdge(tempEdge2);

                MovableObjectNode old = new MovableObjectNode(model);
                old.Assign(this);
                //currentNode = tempNode;
				UpdateNodeEdge(tempNode, null);

                MoveToMovableNode(targetNode);

                tempNode.RemoveEdge(tempEdge1);
                tempNode.RemoveEdge(tempEdge2);

                //currentNode = null;
                //currentEdge = old.currentEdge;

                pathIndex = 0;

                if (pathInfo != null && pathInfo.pathEdges != null && pathInfo.pathEdges.Length > 0)
                {
                    if (pathInfo.pathEdges[0] == tempEdge1)
                    {
                        pathInfo.pathEdges[0] = old.currentEdge;
						pathInfo.edgeDirections [0] = EdgeDirection.BACKWARD;
						edgeDirection = EdgeDirection.BACKWARD;
						if (old.edgeDirection == EdgeDirection.FORWARD)
                            edgePosRate = 1 - old.edgePosRate;
                        else
                            edgePosRate = old.edgePosRate;

						UpdateNodeEdge (null, old.currentEdge);
                    }
                    else if (pathInfo.pathEdges[0] == tempEdge2)
                    {
                        pathInfo.pathEdges[0] = old.currentEdge;
						pathInfo.edgeDirections [0] = EdgeDirection.FORWARD;
						edgeDirection = EdgeDirection.FORWARD;
						if (old.edgeDirection == EdgeDirection.FORWARD)
                            edgePosRate = old.edgePosRate;
                        else
                            edgePosRate = 1 - old.edgePosRate;
						UpdateNodeEdge (null, old.currentEdge);
                    }
                    else
                    {
                        Debug.LogError("MovableObjectNode : unknown error");
                    }
                }
            }
        }
    }
    public void MoveToNode(MapNode targetNode)
    {
        MoveToNode(targetNode, 0);
    }
    public void MoveToNode(MapNode targetNode, float targetZ)
    {
		StopMoving ();
        if (currentNode != null)
        {
            //MapEdge[] searchedPath = GraphAstar.SearchPath(currentNode, targetNode);
            PathResult result = GraphAstar.SearchPath(currentNode, targetNode);

            pathInfo = result;
            state = MovableState.MOVE;

            pathIndex = 0;
            edgePosRateGoal = 1;
        }
        else if (currentEdge != null)
        {
			PathResult result1 = GraphAstar.SearchPath (currentEdge.node1, targetNode);
			PathResult result2 = GraphAstar.SearchPath (currentEdge.node2, targetNode);

			float result1cost = 0;
			if (edgeDirection == EdgeDirection.FORWARD) {
				result1cost = currentEdge.cost * edgePosRate + result1.totalCost;
			} else {
				result1cost = currentEdge.cost * (1 - edgePosRate) + result1.totalCost;
			}

			float result2cost = 0;
			if (edgeDirection == EdgeDirection.FORWARD) {
				result2cost = currentEdge.cost * (1 - edgePosRate) + result2.totalCost;
			} else {
				result2cost = currentEdge.cost * edgePosRate + result2.totalCost;
			}

			if (result1cost < result2cost) {
				List<EdgeDirection> edgeDirections = new List<EdgeDirection> (result1.edgeDirections);
				List<MapEdge> pathEdges = new List<MapEdge> (result1.pathEdges);
				edgeDirections.Insert (0, EdgeDirection.BACKWARD);
				pathEdges.Insert (0, currentEdge);

				result1.pathEdges = pathEdges.ToArray ();
				result1.edgeDirections = edgeDirections.ToArray ();

				if (edgeDirection == EdgeDirection.FORWARD) {
					edgeDirection = EdgeDirection.BACKWARD;
					edgePosRate = 1 - edgePosRate; 
				}

				pathInfo = result1;
			} else {
				List<EdgeDirection> edgeDirections = new List<EdgeDirection> (result2.edgeDirections);
				List<MapEdge> pathEdges = new List<MapEdge> (result2.pathEdges);
				edgeDirections.Insert (0, EdgeDirection.FORWARD);
				pathEdges.Insert (0, currentEdge);

				result1.pathEdges = pathEdges.ToArray ();
				result1.edgeDirections = edgeDirections.ToArray ();

				if (edgeDirection == EdgeDirection.BACKWARD) {
					edgeDirection = EdgeDirection.FORWARD;
					edgePosRate = 1 - edgePosRate; 
				}

				pathInfo = result2;
			}

			pathIndex = 0;
			state = MovableState.MOVE;
			edgePosRateGoal = 1;
			/*
            MapNode tempNode = new MapNode("-1", GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
            MapEdge tempEdge1 = new MapEdge(tempNode, currentEdge.node1, currentEdge.type);
            MapEdge tempEdge2 = new MapEdge(tempNode, currentEdge.node2, currentEdge.type);

            tempNode.isTemporary = true;
            tempEdge1.isTemporary = true;
            tempEdge1.baseEdge = currentEdge;
            tempEdge2.isTemporary = true;
            tempEdge2.baseEdge = currentEdge;

            tempNode.AddEdge(tempEdge1);
            tempNode.AddEdge(tempEdge2);

            //MapEdge[] searchedPath = GraphAstar.SearchPath(tempNode, targetNode);
            PathResult result = GraphAstar.SearchPath(tempNode, targetNode);
            pathInfo = result;
            state = MovableState.MOVE;


            MapEdge[] searchedPath = pathInfo.pathEdges;
            pathIndex = 0;
            if (searchedPath.Length > 0)
            {
                if (searchedPath[0].node2 == currentEdge.node1)
                {
                    // direction이 1이었으면 방향이 반대이므로 rate를 뒤집는다.
					if(edgeDirection == 1)
					{
                    	edgePosRate = 1 - edgePosRate;
                    	edgeDirection = 0;
					}
                }
				else // searchedPath[0].node2 == currentEdge.node2
                {
                    // direction이 0이었으면 방향이 반대이므로 rate를 뒤집는다.
					if(edgeDirection == 0)
					{
						edgePosRate = 1 - edgePosRate;
                    	edgeDirection = 1;
					}
                }
				searchedPath[0] = currentEdge;
            }

            edgePosRateGoal = 1;
            */
        }
        else
        {
            //Debug.Log("Current State invalid");
        }
    }

	public void MoveBy(UnitDirection direction, float value)
	{
		StopMoving ();
		
		pathMoveBy = new PathMoveBy ();
		pathMoveBy.direction = direction;
		pathMoveBy.value = value;

		unitDirection = direction;
		state = MovableState.MOVE;

		moveDistance = 0;

		if (currentNode != null && MoveBy_GetNextEdge () == null)
			StopMoving ();
	}

    public bool EqualPosition(MapNode node)
    {
        if (GetCurrentNode() != null && GetCurrentNode().GetId() == node.GetId())
        {
            return true;
        }
        return false;
    }

	private void UpdateNodeEdge(MapNode node, MapEdge edge)
	{
		_currentNode = node;
		_currentEdge = edge;
		if (node != null && !node.isTemporary)
		{
			currentScale = node.scaleFactor;
			lastNode = node;
		}
		UpdateCurrentPassage ();
	}

	private void UpdateCurrentPassage()
	{
		PassageObjectModel oldPassage = currentPassage;
		if (currentNode != null)
		{
			currentPassage = currentNode.GetAttachedPassage();
		}
		else if (currentEdge != null)
		{
			PassageObjectModel p1 = currentEdge.node1.GetAttachedPassage();
			PassageObjectModel p2 = currentEdge.node2.GetAttachedPassage();

			if (p1 != null && p2 != null)
			{
				if (p1 == p2)
					currentPassage = p1;
				else
					currentPassage = null;
			}
			else
			{
				currentPassage = null;
			}
		}
		else
		{
			Debug.Log("ERROR : invalid node state");
		}

		if (oldPassage != currentPassage) {
			if (oldPassage != null) {
				oldPassage.ExitUnit (this);
			}
			if (currentPassage != null) {
				currentPassage.EnterUnit (this);
			}
		}
	}

	private MapNode elevatorNextDest = null;
	private MovableObjectNode elevatorNextDest2 = null;
	private bool isEnteredElevator = false;


	public bool InElevator()
	{
		return isEnteredElevator;
	}

	public void EnterElevator(MapNode elevatorNode, MapNode nextNode)
	{
		if(true)//if(model != null && model is WorkerModel)
		{
			if (currentNode != null && elevatorNode.GetElevator () != null)
			{
				bool isExitNode = false;
				foreach (MapNode exitNode in elevatorNode.GetElevator ().GetCurrentFloorNodes ())
				{
					if (exitNode == currentNode)
					{
						isExitNode = true;
						break;
					}
				}
				if (isExitNode)
				{
					elevatorNextDest = destinationNode;
					elevatorNextDest2 = destinationNode2;

					currentElevator = elevatorNode.GetElevator ();
					currentElevator.OnUnitEnter (this, nextNode);
					isEnteredElevator = true;
				}
				else
				{
					elevatorNode.GetElevator ().ClickButton (currentNode);
				}
			}
		}
	}

	public void ExitElevator(MapNode node)
	{
		if (currentElevator != null)
		{
			SetCurrentNode (node);
			/*
			_currentNode = node;
			_currentEdge = null;
			*/
			if (elevatorNextDest != null) {
				MoveToNode (elevatorNextDest);
			} else if (elevatorNextDest2 != null) {
				MoveToMovableNode (elevatorNextDest2);
			} else {
				
			}
		}
		isEnteredElevator = false;
	}

}
