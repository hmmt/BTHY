﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathData
{
    public MapEdge[] pathList;
    public float[] pathList_PosRate;

    public int pathIndex;
}

public class MovableObjectNode {
    private MapNode currentNode;

    private MapEdge currentEdge;
    public float currentZValue;

    public float edgePosRate; // 0~1

    public int edgeDirection; // 1 : node1 에서 node2로 이동중
                              // 0 : node2 에서 node1로 이동중

    //private MapEdge[] pathList;
    private PathResult pathInfo;
    private float edgePosRateGoal; // 목표 edge의 edgePosRate
    public float targetZValue;

    private int pathIndex;

    private PathData pathData = new PathData();

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

    public float GetZValue()
    {
        return 0;
    }

    /**
     * movement의 속도로 목적지까지 Time.delta 시간 동안 이동합니다.
     * 
     * 매 프레임 마다 호출해야 합니다.
     * 
     */
    public void ProcessMoveNode(int movement)
    {
        if (pathInfo != null)
        {
            if (currentNode != null)
            {
                if (pathIndex >= pathInfo.pathEdges.Length)
                {
                    pathInfo = null;
                }
                else
                {
                    if (currentZValue > 0)
                    {
                        if (pathInfo.pathEdges[pathIndex].node1 == currentNode)
                        {
                            ///////// 여기부터
                            //MapNodePlane plane = pathInfo.pathEdges[pathIndex].node2.GetPlane();
                            //if (plane == null)
                            {
                                currentZValue -= Time.deltaTime;
                                if (currentZValue < 0) currentZValue = 0;
                                return;
                            }
                        }
                        else
                        {
                            //MapNodePlane plane = pathInfo.pathEdges[pathIndex].node1.GetPlane();
                            //if (plane == null)
                            {
                                currentZValue -= Time.deltaTime;
                                if (currentZValue < 0) currentZValue = 0;
                                return;
                            }
                        }
                    }

                    currentEdge = pathInfo.pathEdges[pathIndex];
                    if (currentEdge.node1 == currentNode)
                    {
                        //edgeDirection = 1;
                    }
                    else
                    {
                        //edgeDirection = 0;
                    }
                    edgeDirection = pathInfo.edgeDirections[pathIndex];
                    edgePosRate = 0;
                    currentNode = null;
                }
            }
            else if (currentEdge != null)
            {
                if (pathInfo.pathEdges != null)
                {
                    float deltaRate = Time.deltaTime / currentEdge.cost * movement;
                    float oldPosRate = edgePosRate;
                    edgePosRate += deltaRate;

                    if (pathIndex >= pathInfo.pathEdges.Length - 1) // 마지막 노드
                    {
                        if (edgePosRate >= edgePosRateGoal)
                        {
                            edgePosRate = edgePosRateGoal;
                            if (edgePosRateGoal == 1) // edge 중간이 아니라 노드로 이동
                            {
                                if (edgeDirection == 1)
                                    currentNode = currentEdge.node2;
                                else
                                    currentNode = currentEdge.node1;
                            }
                            pathInfo = null; // 이동 종료
                            // z값 이동
                            currentZValue = targetZValue;
                        }
                        else
                        {
                            // z값 이동
                            float remainPosRate = 1 - oldPosRate;

                            if (currentZValue != targetZValue)
                            {
                                float deltaZValue = (targetZValue - currentZValue) * deltaRate / remainPosRate;
                                currentZValue += deltaZValue;
                            }
                        }
                    }
                    else if (edgePosRate >= 1)
                    {
                        if (edgeDirection == 1)
                            currentNode = currentEdge.node2;
                        else
                            currentNode = currentEdge.node1;

                        edgePosRate = 0;
                        currentEdge = null;
                        pathIndex++;
                    }
                }
            }
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
        currentNode = node;
        currentEdge = null;
    }
    public void Assign(MovableObjectNode src)
    {
        currentEdge = src.currentEdge;
        currentNode = src.currentNode;
        if (src.pathInfo != null)
        {
            pathInfo = new PathResult((MapEdge[])src.pathInfo.pathEdges.Clone(), (int[])src.pathInfo.edgeDirections.Clone());
        }
        else
            pathInfo = null;
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
        return pathInfo != null;
    }

    public void StopMoving()
    {
        pathInfo = null;
        if (currentNode != null && currentNode.isTemporary)
        {
            Debug.Log("23");
        }
        else if (currentEdge != null && currentEdge.isTemporary)
        {
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
                currentEdge = baseEdge;
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
        }
    }

    // TODO : 같은 node_group에 있는 애들만 포함해야 함
    public bool CheckInRange(MovableObjectNode other)
    {
        return CheckInRange(other, 3);
    }
    public bool CheckInRange(MovableObjectNode other, float range)
    {
        float distance = -1;
        if (currentNode != null && other.currentNode != null)
        {
            distance = GraphAstar.Distance(currentNode, other.currentNode, range+5);
        }
        else if (currentNode == null && currentEdge != null && other.currentNode != null)
        {
            MapNode tempNode = new MapNode("-1", GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
            MapEdge tempEdge1 = new MapEdge(tempNode, currentEdge.node1, currentEdge.type);
            MapEdge tempEdge2 = new MapEdge(tempNode, currentEdge.node2, currentEdge.type);

            tempNode.AddEdge(tempEdge1);
            tempNode.AddEdge(tempEdge2);

            distance = GraphAstar.Distance(tempNode, other.currentNode, range+5);
        }
        else if (currentNode != null && other.currentNode == null && other.currentEdge != null)
        {
            MapNode tempNode = new MapNode("-1", other.GetCurrentViewPosition(), other.currentEdge.node1.GetAreaName());
            MapEdge tempEdge1 = new MapEdge(tempNode, other.currentEdge.node1, other.currentEdge.type);
            MapEdge tempEdge2 = new MapEdge(tempNode, other.currentEdge.node2, other.currentEdge.type);

            tempNode.AddEdge(tempEdge1);
            tempNode.AddEdge(tempEdge2);

            distance = GraphAstar.Distance(currentNode, tempNode, range+5);
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

            distance = GraphAstar.Distance(leftTempNode, rightTempNode, range+5);

            other.currentEdge.node1.RemoveEdge(rightTempEdge1);
            other.currentEdge.node1.RemoveEdge(rightTempEdge2);
        }
        else
        {
            Debug.Log("Current State invalid");
        }

        return distance != -1 && distance < range;
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
            pathInfo = result;

            MapEdge[] searchedPath = result.pathEdges;
            pathIndex = 0;
            if (searchedPath.Length > 0)
            {
                MapEdge lastEdge = searchedPath[searchedPath.Length-1];
                if (lastEdge.node1 == targetNode.currentEdge.node1) // current
                {
                    searchedPath[searchedPath.Length - 1] = targetNode.currentEdge;

                    if (targetNode.edgeDirection == 1)
                        edgePosRateGoal = targetNode.edgePosRate;
                    else
                        edgePosRateGoal = 1 - targetNode.edgePosRate;
                }
                else if (lastEdge.node1 == targetNode.currentEdge.node2)
                {
                    searchedPath[searchedPath.Length - 1] = targetNode.currentEdge;
                    edgePosRateGoal = targetNode.edgePosRate;

                    if (targetNode.edgeDirection == 1)
                        edgePosRateGoal = 1 - targetNode.edgePosRate;
                    else
                        edgePosRateGoal = targetNode.edgePosRate;
                }
                else
                {
                    Debug.LogError("UNKNOWN ERROR : ???");
                }
            }
            targetNode.currentEdge.node1.RemoveEdge(tempEdge1);
            targetNode.currentEdge.node2.RemoveEdge(tempEdge2);
        }
        else if (currentEdge != null)
        {
            if (currentEdge == targetNode.currentEdge)
            {
                float aPos, bPos;

                pathInfo = new PathResult(new MapEdge[1], new int[1]);
                pathInfo.pathEdges[0] = currentEdge;
                pathInfo.edgeDirections[0] = edgeDirection;

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
                targetZValue = targetNode.currentZValue;
            }
            else
            {
                /*
                // temp
                currentNode = currentEdge.node1;
                currentEdge = null;
                MoveToMovableNode(targetNode);
                */
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

                MovableObjectNode old = new MovableObjectNode();
                old.Assign(this);
                currentNode = tempNode;

                MoveToMovableNode(targetNode);

                tempNode.RemoveEdge(tempEdge1);
                tempNode.RemoveEdge(tempEdge2);

                currentNode = null;
                currentEdge = old.currentEdge;
                pathIndex = 1;

                if (pathInfo != null && pathInfo.pathEdges != null && pathInfo.pathEdges.Length > 0)
                {
                    if (pathInfo.pathEdges[0] == tempEdge1)
                    {
                        pathInfo.pathEdges[0] = old.currentEdge;
                        edgeDirection = 0;
                        if (old.edgeDirection == 1)
                            edgePosRate = 1 - old.edgePosRate;
                        else
                            edgePosRate = old.edgePosRate;
                    }
                    else if (pathInfo.pathEdges[0] == tempEdge2)
                    {
                        pathInfo.pathEdges[0] = old.currentEdge;
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
                    targetZValue = targetNode.currentZValue;
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

            pathIndex = 0;
            edgePosRateGoal = 1;

            targetZValue = targetZ;
        }
        else if (currentEdge != null)
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

            //MapEdge[] searchedPath = GraphAstar.SearchPath(tempNode, targetNode);
            PathResult result = GraphAstar.SearchPath(tempNode, targetNode);
            pathInfo = result;


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
            targetZValue = targetZ;
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
}
