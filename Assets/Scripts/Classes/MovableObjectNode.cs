﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathData
{
    public MapEdge[] pathList;
    public float[] pathList_PosRate;

    public int pathIndex;
}

public enum MovableState
{
    MOVE,
    STOP,
    WAIT
}

public class MovableObjectNode {
    private ObjectModelBase model;

    private MovableState state;

    private MapNode lastNode;

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

    public int edgeDirection; // 1 : node1 에서 node2로 이동중
                              // 0 : node2 에서 node1로 이동중

    //private MapEdge[] pathList;
    private PathResult pathInfo;
    private float edgePosRateGoal; // 목표 edge의 edgePosRate
	private MapNode destinationNode = null;
	private MovableObjectNode destinationNode2 = null;

    private int pathIndex;

    private PathData pathData = new PathData();


	public float currentScale = 1.0f;

    // 지울 예정
    public MovableObjectNode()
    {
    }
    public MovableObjectNode(ObjectModelBase model)
    {
        this.model = model;
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
            MapNode node1 = currentEdge.node1;
            MapNode node2 = currentEdge.node2;
            Vector3 pos1 = node1.GetPosition();
            Vector3 pos2 = node2.GetPosition();

            if (edgeDirection == 1)
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
        }
        return output;
    }

    // 현재 있는 노드가 passage에 속해 있으면 그 passage 반환
    public PassageObjectModel GetPassage()
    {
		return currentPassage;
		/*
        if (currentNode != null)
        {
            return currentNode.GetAttachedPassage();
        }
        else if (currentEdge != null)
        {
            PassageObjectModel p1 = currentEdge.node1.GetAttachedPassage();
            PassageObjectModel p2 = currentEdge.node1.GetAttachedPassage();

            if (p1 != null && p2 != null)
            {
                if (p1 == p2)
                    return p1;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        else
        {
            Debug.Log("ERROR : invalid node state");
        }
        return null;
        */
    }

    /**
     * movement의 속도로 목적지까지 Time.delta 시간 동안 이동합니다.
     * 
     * 매 프레임 마다 호출해야 합니다.
     * 
     */
    public void ProcessMoveNode(int movement)
    {
		bool stateChanged = false;

		if (movement <= 0)
			return;

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
            if (currentNode != null)
            {
                if (pathIndex >= pathInfo.pathEdges.Length)
                {
                    pathInfo = null;
                    state = MovableState.STOP;
                }
                else
                {
                    MapEdge nextEdge = pathInfo.pathEdges[pathIndex];
                    int nextDirection = pathInfo.edgeDirections[pathIndex];
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
							int destDirection = pathInfo.edgeDirections[pathIndex+1];
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
                        /*
                        if (currentEdge.node1 == currentNode)
                        {
                            //edgeDirection = 1;
                        }
                        else
                        {
                            //edgeDirection = 0;
                        }
                        */
                        edgeDirection = nextDirection;
                        edgePosRate = 0;

						//currentEdge = nextEdge;
                        //currentNode = null;
						UpdateNodeEdge (null, nextEdge);
                    }
                }
            }
            else if (currentEdge != null) // 현재 edge 위에 있는 경우
            {
                if (pathInfo.pathEdges != null)
                {
                    float deltaRate = Time.deltaTime / currentEdge.cost * movement;
                    float oldPosRate = edgePosRate;
                    edgePosRate += deltaRate;

					if (edgeDirection == 1)
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
                                if (edgeDirection == 1)
									//currentNode = currentEdge.node2;
									UpdateNodeEdge (currentEdge.node2, null);
                                else
                                    //currentNode = currentEdge.node1;
									UpdateNodeEdge (currentEdge.node1, null);
                            }
                            pathInfo = null; // 이동 종료
                            state = MovableState.STOP;
                        }
                    }
                    else
                    {
                        // 다음 노드에 도착
                        if (edgePosRate >= 1)
                        {
                            if (edgeDirection == 1)
                                //currentNode = currentEdge.node2;
								UpdateNodeEdge (currentEdge.node2, null);
                            else
                                //currentNode = currentEdge.node1;
								UpdateNodeEdge (currentEdge.node1, null);

                            edgePosRate = 0;
                            //currentEdge = null;
                            pathIndex++;
                        }
                    }
                }
            }
        }


		if (currentNode != null)
		{
			currentScale = currentNode.scaleFactor;
			lastNode = currentNode;
		}
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
	public UnitDirection GetDirection()
	{
		return unitDirection;
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
				(int[])src.pathInfo.edgeDirections.Clone(),
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
    public int GetEdgeDirection()
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
					result.edgeDirections [searchedPath.Length - 1] = 1;

                    if (targetNode.edgeDirection == 1)
						rateGoal = targetNode.edgePosRate;
                    else
						rateGoal = 1 - targetNode.edgePosRate;
                }
                else if (lastEdge.node1 == targetNode.currentEdge.node2)
                {
                    searchedPath[searchedPath.Length - 1] = targetNode.currentEdge;
					result.edgeDirections [searchedPath.Length - 1] = 0;

                    if (targetNode.edgeDirection == 1)
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

				PathResult pathResult = new PathResult(new MapEdge[1], new int[1], currentEdge.cost); // cost??

                if (edgeDirection == 1)
                    aPos = edgePosRate;
                else
                    aPos = 1 - edgePosRate;

                if (targetNode.edgeDirection == 1)
                    bPos = targetNode.edgePosRate;
                else
                    bPos = 1 - targetNode.edgePosRate;

                if (aPos > bPos)
                {
                    edgeDirection = 0;
                    edgePosRate = 1 - aPos;
                    edgePosRateGoal = 1 - bPos;
                }
				else
                {
                    edgeDirection = 1;
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

                pathIndex = 1;

                if (pathInfo != null && pathInfo.pathEdges != null && pathInfo.pathEdges.Length > 0)
                {
                    if (pathInfo.pathEdges[0] == tempEdge1)
                    {
                        pathInfo.pathEdges[0] = old.currentEdge;
						pathInfo.edgeDirections [0] = 0;
                        edgeDirection = 0;
                        if (old.edgeDirection == 1)
                            edgePosRate = 1 - old.edgePosRate;
                        else
                            edgePosRate = old.edgePosRate;
                    }
                    else if (pathInfo.pathEdges[0] == tempEdge2)
                    {
                        pathInfo.pathEdges[0] = old.currentEdge;
						pathInfo.edgeDirections [0] = 1;
                        edgeDirection = 1;
                        if (old.edgeDirection == 1)
                            edgePosRate = old.edgePosRate;
                        else
                            edgePosRate = 1 - old.edgePosRate;
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
			if (edgeDirection == 1) {
				result1cost = currentEdge.cost * edgePosRate + result1.totalCost;
			} else {
				result1cost = currentEdge.cost * (1 - edgePosRate) + result1.totalCost;
			}

			float result2cost = 0;
			if (edgeDirection == 1) {
				result2cost = currentEdge.cost * (1 - edgePosRate) + result2.totalCost;
			} else {
				result2cost = currentEdge.cost * edgePosRate + result2.totalCost;
			}

			if (result1cost < result2cost) {
				List<int> edgeDirections = new List<int> (result1.edgeDirections);
				List<MapEdge> pathEdges = new List<MapEdge> (result1.pathEdges);
				edgeDirections.Insert (0, 0);
				pathEdges.Insert (0, currentEdge);

				result1.pathEdges = pathEdges.ToArray ();
				result1.edgeDirections = edgeDirections.ToArray ();

				if (edgeDirection == 1) {
					edgeDirection = 0;
					edgePosRate = 1 - edgePosRate; 
				}

				pathInfo = result1;
			} else {
				List<int> edgeDirections = new List<int> (result2.edgeDirections);
				List<MapEdge> pathEdges = new List<MapEdge> (result2.pathEdges);
				edgeDirections.Insert (0, 1);
				pathEdges.Insert (0, currentEdge);

				result1.pathEdges = pathEdges.ToArray ();
				result1.edgeDirections = edgeDirections.ToArray ();

				if (edgeDirection == 0) {
					edgeDirection = 1;
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
            Debug.Log("Current State invalid");
        }
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
			PassageObjectModel p2 = currentEdge.node1.GetAttachedPassage();

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

	public void EnterElevator(MapNode elevatorNode, MapNode nextNode)
	{
		if(model != null && model is WorkerModel)
		{
			if (currentNode != null && elevatorNode.GetElevator () != null)
			{
				if (elevatorNode.GetElevator ().GetCurrentFloorNode () == currentNode)
				{
					elevatorNextDest = destinationNode;
					elevatorNextDest2 = destinationNode2;

					currentElevator = elevatorNode.GetElevator ();
					currentElevator.OnUnitEnter ((WorkerModel)model, nextNode);
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
	}

}
