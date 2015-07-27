using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovableObjectNode {

    private MapNode currentNode;

    private MapEdge currentEdge;
    public float edgePosRate; // 0~1

    public int edgeDirection; // 1 : node1 에서 node2로 이동중
                              // 0 : node2 에서 node1로 이동중

    private MapEdge[] pathList;
    private int pathIndex;

    public Vector2 GetCurrentViewPosition()
    {
        Vector2 output = new Vector2(0, 0);

        if (currentNode != null)
        {
            Vector2 pos = currentNode.GetPosition();
            output.x = pos.x;
            output.y = pos.y;
        }
        else if (currentEdge != null)
        {
            MapNode node1 = currentEdge.node1;
            MapNode node2 = currentEdge.node2;
            Vector2 pos1 = node1.GetPosition();
            Vector2 pos2 = node2.GetPosition();

            if (edgeDirection == 1)
            {
                output.x = Mathf.Lerp(pos1.x, pos2.x, edgePosRate);
                output.y = Mathf.Lerp(pos1.y, pos2.y, edgePosRate);
            }
            else
            {
                output.x = Mathf.Lerp(pos1.x, pos2.x, 1 - edgePosRate);
                output.y = Mathf.Lerp(pos1.y, pos2.y, 1 - edgePosRate);
            }
        }
        return output;
    }

    /**
     * movement의 속도로 목적지까지 Time.delta 시간 동안 이동합니다.
     * 
     * 매 프레임 마다 호출해야 합니다.
     * 
     */
    public void ProcessMoveNode(int movement)
    {
        if (pathList != null)
        {
            if (currentNode != null)
            {
                if (pathIndex >= pathList.Length)
                {
                    pathList = null;
                }
                else
                {

                    currentEdge = pathList[pathIndex];
                    if (currentEdge.node1 == currentNode)
                    {
                        edgeDirection = 1;
                    }
                    else
                    {
                        edgeDirection = 0;
                    }
                    currentNode = null;
                }
            }
            else if (currentEdge != null)
            {
                edgePosRate += Time.deltaTime / currentEdge.cost * movement;

                if (edgePosRate >= 1)
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



    // edge 위에 있을 때도 통합할 수 있는 타입 필요
    public MapNode GetCurrentNode()
    {
        return currentNode;
    }
    public void SetCurrentNode(MapNode node)
    {
        pathList = null;
        currentNode = node;
        currentEdge = null;
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
        return pathList != null;
    }

    public void StopMoving()
    {
        pathList = null;
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

    public bool CheckInRange(MovableObjectNode other)
    {
        float distance = -1;
        if (currentNode != null && other.currentNode != null)
        {
            distance = GraphAstar.Distance(currentNode, other.currentNode, 3);
        }
        else if (currentNode == null && currentEdge != null && other.currentNode != null)
        {
            MapNode tempNode = new MapNode("-1", GetCurrentViewPosition(), currentEdge.node1.GetAreaName());
            MapEdge tempEdge1 = new MapEdge(tempNode, currentEdge.node1, currentEdge.type);
            MapEdge tempEdge2 = new MapEdge(tempNode, currentEdge.node2, currentEdge.type);

            tempNode.isTemporary = true;
            tempEdge1.isTemporary = true;
            tempEdge2.isTemporary = true;

            tempNode.AddEdge(tempEdge1);
            tempNode.AddEdge(tempEdge2);

            distance = GraphAstar.Distance(tempNode, other.currentNode, 3);
        }
        else if (currentNode != null && other.currentNode == null && other.currentEdge != null)
        {
            MapNode tempNode = new MapNode("-1", other.GetCurrentViewPosition(), other.currentEdge.node1.GetAreaName());
            MapEdge tempEdge1 = new MapEdge(tempNode, other.currentEdge.node1, other.currentEdge.type);
            MapEdge tempEdge2 = new MapEdge(tempNode, other.currentEdge.node2, other.currentEdge.type);

            tempNode.AddEdge(tempEdge1);
            tempNode.AddEdge(tempEdge2);

            distance = GraphAstar.Distance(currentNode, tempNode, 3);
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

            distance = GraphAstar.Distance(leftTempNode, rightTempNode, 3);

            other.currentEdge.node1.RemoveEdge(rightTempEdge1);
            other.currentEdge.node1.RemoveEdge(rightTempEdge2);
        }
        else
        {
            Debug.Log("Current State invalid");
        }

        return distance != -1;
    }

    public void MoveToNode(MapNode targetNode)
    {
        if (currentNode != null)
        {
            MapEdge[] searchedPath = GraphAstar.SearchPath(currentNode, targetNode);

            pathList = searchedPath;
            pathIndex = 0;
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

            MapEdge[] searchedPath = GraphAstar.SearchPath(tempNode, targetNode);

            pathList = searchedPath;
            pathIndex = 0;
            if (searchedPath.Length > 0)
            {
                currentEdge = searchedPath[0];
                edgePosRate = 0;
                edgeDirection = 1;
            }
        }
        else
        {
            Debug.Log("Current State invalid");
        }
    }
}
