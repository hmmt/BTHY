using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GraphAstar {
	
	public class PathScore : IComparable<PathScore>
	{
		public MapNode node;
		
		public float cost;
		public float h;
		
		public PathScore()
		{
		}
		public PathScore(MapNode node)
		{
			this.node = node;
		}
		
		public int CompareTo(PathScore other)
		{
			return (int)((cost + h - (other.cost + other.h))*10);
		}
	}
	
	public class SearchInfo
	{
		public float cost;
		public MapEdge edge;
		
		public SearchInfo(){}
		public SearchInfo(float cost, MapEdge edge)
		{
			this.cost = cost;
			this.edge = edge;
		}
	}
	
	public static float ComputeHeuristic(Vector2 a, Vector2 b)
	{
		return Vector2.Distance(a, b);
	}


	// point를 노드 뿐만 아니라 edge의 중간도 지정할 수 있도록 바꿔야 함.
	public static MapEdge[] SearchPath(MapNode startPoint, MapNode endPoint)
	{
		PriorityQueue<PathScore> opendset = new PriorityQueue<PathScore> ();
		HashSet<MapNode> closedset = new HashSet<MapNode> ();
		
		Dictionary<MapNode, SearchInfo> dic = new Dictionary<MapNode, SearchInfo> ();

		PathScore cur = new PathScore (startPoint);
		cur.cost = 0;
		cur.h = 0;
		
		opendset.Enqueue (cur);
		
		while(true)
		{
			if(opendset.Count() <= 0)
				break;
			cur = opendset.Dequeue();
			
			if(cur.node == endPoint)
			{
				System.Collections.ArrayList outputDirs = new System.Collections.ArrayList();
				MapNode pathNode = cur.node;
				
				while(true)
				{
					SearchInfo value=null;
					if(!dic.TryGetValue(pathNode, out value))
					{
						break;
					}
					MapEdge edge = value.edge;
					pathNode = edge.ConnectedNode(pathNode);
					//Debug.Log("path : ["+edge.node1.GetId() +", "+ edge.node2.GetId() +"]");
					outputDirs.Add(edge);
				}
				
				outputDirs.Reverse();
				return (MapEdge[])outputDirs.ToArray(typeof(MapEdge));
			}
			
			// Debug.Log("visit : ["+cur.x+", "+cur.y+"]");
			//Debug.Log("visit : ["+cur.node.GetId()+"]");
			
			closedset.Add(cur.node);

			foreach(MapEdge edge in cur.node.GetEdges())
			{
				MapNode nextNode = edge.ConnectedNode(cur.node);
                if (nextNode == null)
                    continue;
				if(closedset.Contains(nextNode))
				{
					continue;
				}
				//Debug.Log("visit : ["+nextNode.GetId()+"]");
				PathScore newPoint = new PathScore(nextNode);
				newPoint.cost = cur.cost + edge.cost;
				newPoint.h = ComputeHeuristic(nextNode.GetPosition(), endPoint.GetPosition());

				SearchInfo oldInfo = null;
				dic.TryGetValue(nextNode, out oldInfo);
				
				if(!dic.TryGetValue(nextNode, out oldInfo) || oldInfo.cost > newPoint.cost)
				{
					opendset.Enqueue(newPoint);
					dic.Add(nextNode, new SearchInfo(newPoint.cost, edge));
				}
			}
		}
		
		return new MapEdge[]{};
	}
}
