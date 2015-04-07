using UnityEngine;
//using System.Collections;
using System;
using System.Collections.Generic;

public class Astar : MonoBehaviour {

	public class PathScore : IComparable<PathScore>
	{
		public int x;
		public int y;

		public int cost;
		public int h;

		public PathScore()
		{
		}
		public PathScore(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public int CompareTo(PathScore other)
		{
			return cost + h - (other.cost + other.h);
		}
	}

	public class SearchInfo
	{
		public int cost;
		public int direction;

		public SearchInfo(){}
		public SearchInfo(int cost, int direction)
		{
			this.cost = cost;
			this.direction = direction;
		}
	}

	// Use this for initialization
	void Start () {
		/*
		int[,] map = new int[,]{
			{1,0,1,1,1},
			{1,0,1,0,1},
			{1,0,1,0,1},
			{1,1,1,0,1},
			{1,0,1,0,1}
		};

		SearchPath (map, 0, 0, 4, 4);
		*/
	}

	public static bool IsPathable(int type)
	{
		if (type == 1)
			return true;
		return false;
	}

	public static int ComputeHeuristic(int curx, int cury, int goalx, int goaly)
	{
		return (Mathf.Abs (curx - goalx) + Mathf.Abs (cury - goaly))*10;
	}

	public static AIPoint[] SearchPath(int[,] map, int startx, int starty, int endx, int endy)
	{
		PriorityQueue<PathScore> opendset = new PriorityQueue<PathScore> ();
		HashSet<int> closedset = new HashSet<int> ();

		Dictionary<int, SearchInfo> dic = new Dictionary<int, SearchInfo> ();


		int height = map.GetLength(0);

		int curx = startx;
		int cury = starty;


		PathScore cur = new PathScore (startx, starty);
		cur.cost = 0;
		cur.h = 0;

		opendset.Enqueue (cur);

		while(true)
		{
			if(opendset.Count() <= 0)
				break;
			cur = opendset.Dequeue();

			if(cur.x == endx && cur.y == endy)
			{
				System.Collections.ArrayList outputDirs = new System.Collections.ArrayList();
				int x=cur.x,y=cur.y;

				while(true)
				{
					SearchInfo value=null;
					// Debug.Log("path : ["+x+", "+y+"]");
					if(!dic.TryGetValue(y*height + x, out value))
					{
						break;
					}

					switch(value.direction)
					{
					case 0: x+=1; outputDirs.Add(new AIPoint(x, y)); break;
					case 1: x-=1; outputDirs.Add(new AIPoint(x, y)); break;
					case 2: y+=1; outputDirs.Add(new AIPoint(x, y)); break;
					case 3: y-=1; outputDirs.Add(new AIPoint(x, y)); break;
					}
				}

				outputDirs.Reverse();
				return (AIPoint[])outputDirs.ToArray(typeof(AIPoint));
			}

			// Debug.Log("visit : ["+cur.x+", "+cur.y+"]");

			closedset.Add(cur.y*height + cur.x);

			if(cur.x-1 >= 0 &&
				!closedset.Contains(cur.y*height + cur.x-1) &&
			   IsPathable(map[cur.y, cur.x-1]))
			{
				// must add compare routine
				PathScore newPoint = new PathScore(cur.x-1, cur.y);
				newPoint.cost = cur.cost + 10;
				newPoint.h = ComputeHeuristic(cur.x-1, cur.y, endx, endy);

				int key = newPoint.y*height + newPoint.x;
				SearchInfo oldInfo = null;
				dic.TryGetValue(key, out oldInfo);

				if(!dic.TryGetValue(key, out oldInfo) || oldInfo.cost > newPoint.cost)
				{
					opendset.Enqueue(newPoint);
					dic.Add(key, new SearchInfo(newPoint.cost, 0));
				}
			}
			if(cur.x+1 < map.GetLength(1) &&
				!closedset.Contains(cur.y*height + cur.x+1) &&
			   IsPathable(map[cur.y, cur.x+1]))
			{
				PathScore newPoint = new PathScore(cur.x+1, cur.y);
				newPoint.cost = cur.cost + 10;
				newPoint.h = ComputeHeuristic(cur.x+1, cur.y, endx, endy);

				int key = newPoint.y*height + newPoint.x;
				SearchInfo oldInfo = null;
				dic.TryGetValue(key, out oldInfo);

				if(!dic.TryGetValue(key, out oldInfo) || oldInfo.cost > newPoint.cost)
				{
					opendset.Enqueue(newPoint);
					dic.Add(key, new SearchInfo(newPoint.cost, 1));
				}
			}
			if(cur.y-1 >= 0 &&
				!closedset.Contains((cur.y-1)*height + cur.x) &&
				IsPathable(map[cur.y-1,  cur.x]))
			{
				PathScore newPoint = new PathScore(cur.x, cur.y-1);
				newPoint.cost = cur.cost + 10;
				newPoint.h = ComputeHeuristic(cur.x, cur.y-1, endx, endy);

				int key = newPoint.y*height + newPoint.x;
				SearchInfo oldInfo = null;
				dic.TryGetValue(key, out oldInfo);

				if(!dic.TryGetValue(key, out oldInfo) || oldInfo.cost > newPoint.cost)
				{
					opendset.Enqueue(newPoint);
					dic.Add(key, new SearchInfo(newPoint.cost, 2));
				}
			}

			if(cur.y+1 < map.GetLength(0) &&
				!closedset.Contains((cur.y+1)*height + cur.x) &&
			   IsPathable(map[cur.y+1, cur.x]))
			{
				PathScore newPoint = new PathScore(cur.x, cur.y+1);
				newPoint.cost = cur.cost + 10;
				newPoint.h = ComputeHeuristic(cur.x, cur.y+1, endx, endy);

				int key = newPoint.y*height + newPoint.x;
				SearchInfo oldInfo = null;
				if(!dic.TryGetValue(key, out oldInfo) || oldInfo.cost > newPoint.cost)
				{
					opendset.Enqueue(newPoint);
					dic.Add(key, new SearchInfo(newPoint.cost, 3));
				}
			}
		}

		return new AIPoint[]{};
	}
}
