using UnityEngine;
using System.Collections.Generic;

/*
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
*/

public enum MoveDirection { 
    IDLE,
    UP,
    DOWN
}

public class ElevatorUsable{
    public object model;
    public MovableObjectNode movableObject;
    public MoveDirection dir;
    public MapNode dest;

    public ElevatorUsable(object model, MovableObjectNode movableItem) {
        this.model = model;
        this.movableObject = movableItem;
        dir = MoveDirection.IDLE;
        dest = null;
    }

    public object GetTarget<T>(){
        return (T)model;
    }

    public object GetTarget() {
        return model;
    }

    public MovableObjectNode GetMovableObject() {
        return movableObject;
    }

    public void SetDestination() {
        dir = MoveDirection.UP;
        dest = movableObject.GetCurrentEdge().node2;
    }

    public void ReStartMoving() {
        this.dest = null;
    }
}

public class Elevator : MonoBehaviour {
    //[HideInInspector]
    public List<MapNode> nodeList;

    private List<int> moveList;
    public List<ElevatorUsable>[] waitList;
    public List<ElevatorUsable> loaded;
    public int current;//current Position;
    public MoveDirection dir;
    public bool isFull;
    public int limit;//limit of passengers

    private int Max, Min;//Min is highest floor, Max is lowest floor
    private int moveLimit;
    private int weight;

    public void Awake() {
        Init();
    }

    public void Init() {
        nodeList = new List<MapNode>();
        //initialize list;
        dir = MoveDirection.IDLE;
        moveList = new List<int>();
        waitList = new List<ElevatorUsable>[nodeList.Count];
        loaded = new List<ElevatorUsable>();
        current = nodeList.Count - 1;
        isFull = false;
        Max = nodeList.Count - 1;
        Min = 0;
        moveLimit = Min;
        
    }

    /// <summary>
    /// Call Elevator to User's position. User will be added to waitList
    /// </summary>
    /// <param name="i"></param>
    /// <param name="model"></param>
    public void Call(int i, ElevatorUsable model) {
        List<ElevatorUsable> wait = waitList[i];
        wait.Add(model);
    }

    public void LoadPassenger() {
        List<ElevatorUsable> canLoad = new List<ElevatorUsable>();
        List<int> dest = new List<int>();

        foreach (ElevatorUsable user in waitList[current])
        {
            if (user.dir.Equals(this.dir)) {
                canLoad.Add(user);
            }
        }

        while (isFull == false) {
            if (canLoad.Count == 0) break;
            ElevatorUsable user = canLoad[0];
            loaded.Add(user);
            waitList[current].Remove(user);
            canLoad.RemoveAt(0);
            if (loaded.Count == limit) {
                isFull = true;
            }
            
            //목적지 계산
            int destInt = nodeList.IndexOf(user.dest);
            if (!moveList.Contains(destInt)) {
                moveList.Add(destInt);
                //Sort 필요?
            }
        }
        
    }

    public void UnloadPassenger() {
        foreach (ElevatorUsable user in loaded) {
            if (user.dest.Equals(nodeList[current])) { 
                //이동재개
                user.ReStartMoving();
                loaded.Remove(user);
                if (isFull) {
                    isFull = false;
                }
            }
        }
    }

    public void AddDest(int i) {
        moveList.Add(i);
        //정렬, 방향에 따라서 정렬값이 바뀌게 만들 것 
    }


    public void CheckMove() { 
        //앞으로 이동할 방향과 리스트를 체크한다. 원래 직원이 대기하지 않던 층에서 새로운 직원이 대기중일 경우 해당
        //층을 리스트에 추가한다
    }

    public void Move() { 
        //이동
        if (moveList.Count == 0) return;

        UnloadPassenger();
        LoadPassenger();

    }
}