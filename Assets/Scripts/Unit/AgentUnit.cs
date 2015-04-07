using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentUnit : MonoBehaviour {

	// game data
	public AgentTypeInfo metadata;
	public long metadataId;
	public string name;
	public int hp;

	public int mental;
	public int movement;
	public int work;

	public string prefer;
	public int preferBonus;
	public string reject;
	public int rejectBonus;

	public SkillTypeInfo directSkill;
	public SkillTypeInfo indirectSkill;
	public SkillTypeInfo blockSkill;

	public string imgsrc;

	public Dictionary<string, string> speechTable = new Dictionary<string, string>();

	public string panicType;
	//

	public SpriteRenderer spriteRenderer;
	//

	private AgentCmdState state = AgentCmdState.IDLE;
	public CreatureUnit target; // state; MOVE, WORKING

	private PanicAction currentPanicAction;

	// path finding
	/*
	private bool movingOnPath = false;
	private AIPoint[] shortestPath;
	private int pathCurIndex;
	*/

	// path finding2
	private MapNode currentNode;

	private MapEdge currentEdge;
	private float edgePosRate; // 0~1

	private int edgeDirection; //?

//	private GraphPosition graphPosition;

	private MapEdge[] pathList;
	private int pathIndex;

	private void UpdateDirection()
	{
		if(currentEdge != null)
		{
			MapNode node1 = currentEdge.node1;
			MapNode node2 = currentEdge.node2;
			Vector2 pos1 = node1.GetPosition();
			Vector2 pos2 = node2.GetPosition();

			if(edgeDirection == 1)
			{
				Transform anim = transform.Find("Anim");
				Vector3 scale = anim.localScale;
				if(pos2.x - pos1.x > 0 && scale.x < 0)
				{
					scale.x = -scale.x;
				}
				else if(pos2.x - pos1.x < 0 && scale.x > 0)
				{
					scale.x = -scale.x;
				}
				anim.transform.localScale = scale;
			}
			else
			{
				Transform anim = transform.Find("Anim");
				Vector3 scale = anim.localScale;
				if(pos2.x - pos1.x > 0 && scale.x > 0)
				{
					scale.x = -scale.x;
				}
				else if(pos2.x - pos1.x < 0 && scale.x < 0)
				{
					scale.x = -scale.x;
				}
				anim.transform.localScale = scale;
			}
		}
	}

	private Vector2 GetCurrentViewPosition()
	{
		Vector2 output = transform.localPosition;
		if(currentNode != null)
		{
			Vector2 pos = currentNode.GetPosition();
			output.x = pos.x;
			output.y = pos.y;
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
		}
		return output;
	}

	private bool visible = true;
	private float oldZ;

	private void UpdateViewPosition()
	{
		if(currentEdge != null && currentEdge.type == "door")
		{
			if(visible)
			{
				visible = false;
				Vector3 oldViewPosition = transform.localPosition;
				oldZ = oldViewPosition.z;
				oldViewPosition.z = 100000f;
				transform.localPosition = oldViewPosition;
			}
		}
		else
		{
			if(!visible)
			{
				visible = true;
				Vector3 oldViewPosition = transform.localPosition;
				oldViewPosition.z = oldZ;
				transform.localPosition = oldViewPosition;	
			}
			transform.localPosition = GetCurrentViewPosition();
		}
	}

	////

	private float waitTimer = 0;

	void Start () {

		//currentNode = MapGraph.instance.GetNodeById(100);
		currentNode = MapGraph.instance.GetNodeById("1001002");

//		MoveToNode(MapGraph.instance.GetNodeById(5));
	}

	// if map is destroyed....?

	void FixedUpdate()
	{
		ProcessAction ();

		//ProcessMoving ();
		ProcessMoveNode();
	}

	void Update()
	{
		UpdateViewPosition();
		UpdateDirection();
		SetCurrentHP (hp);
		UpdateMentalView ();
	}

	private void ProcessAction()
	{
		if(currentPanicAction != null)
		{
			currentPanicAction.Execute();
		}
		else if(state == AgentCmdState.IDLE)
		{
			if(waitTimer <= 0)
			{
				int x,y;
				/*
				CreatureRoom.instance.WorldToTile(transform.position, out x, out y);
				Vector2 goalPoint = CreatureRoom.instance.GetNearRoamingPoint(new Vector2(x,y));
				MoveToTilePos((int)goalPoint.x, (int)goalPoint.y);
				*/

				MoveToNode(MapGraph.instance.GetRandomRestPoint());
				
				waitTimer = 1.5f + Random.value;
			}
		}
		waitTimer -= Time.deltaTime;
	}
	/*
	private void ProcessMoving()
	{		
		Vector2 moveDirection = new Vector2(0,0);
		do
		{
			if(movingOnPath)
			{
				int x,y;
				CreatureRoom.instance.WorldToTile(transform.position, out x, out y);
				AIPoint nextPoint = shortestPath[pathCurIndex];
				if(x == nextPoint.x && y == nextPoint.y)
				{
					pathCurIndex++;
					if(pathCurIndex >= shortestPath.Length)
					{
						movingOnPath = false;
						moveDirection = new Vector2(0,0);
						break;
					}
					nextPoint = shortestPath[pathCurIndex];
				}
				Vector2 nextPosition = CreatureRoom.instance.TileToWorld(nextPoint.x, nextPoint.y);
				
				moveDirection = (nextPosition - (Vector2)transform.position).normalized;
			}
		}while(false);
		
		
		// update position
		Vector2 newLocalPos = (Vector2)transform.localPosition +  moveDirection * movement * Time.deltaTime;
		
		transform.localPosition = new Vector3 (newLocalPos.x, newLocalPos.y, transform.localPosition.z);
	}
*/
	private void ProcessMoveNode()
	{
		/*
		private MapNode currentNode;

		private MapEdge currentEdge;
		private int edgeDirection;
		private float edgePosRate; // 0~1

		private MapEdge[] pathList;
		private int pathIndex;
		*/

		if(pathList != null)
		{
			if(currentNode != null)
			{
				if(pathIndex >= pathList.Length)
				{
					pathList = null;
				}
				else
				{
					
					currentEdge = pathList[pathIndex];
					if(currentEdge.node1 == currentNode)
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
			else if(currentEdge != null)
			{
				edgePosRate += Time.deltaTime/currentEdge.cost * movement;

				if(edgePosRate >= 1)
				{
					if(edgeDirection == 1)
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
	public AgentCmdState GetState()
	{
		return state;
	}

	public void SetMaxHP(int maxHP)
	{
		GetComponentInChildren<AgentHPBar> ().SetMaxHP (maxHP);
	}

	public void SetCurrentHP(int hp)
	{
		GetComponentInChildren<AgentHPBar> ().SetCurrentHP (hp);
	}

	public void UpdateMentalView()
	{
		GetComponentInChildren<MentalViewer> ().SetMentalRate ((float)mental / (float)metadata.mental);
	}
/*
	public void MoveToTilePos(int goalx, int goaly)
	{
		int x,y;
		CreatureRoom.instance.WorldToTile(transform.position, out x, out y);
		AIPoint[] path = Astar.SearchPath(CreatureRoom.instance.GetTileMap(), x,y, goalx,goaly);
		MoveOnPath(path);
	}

	public void MoveToGlobalPos(Vector2 pos)
	{
		int x,y;
		CreatureRoom.instance.WorldToTile(pos, out x, out y);
		MoveToTilePos (x, y);
	}
	*/

	/*
		private MapNode currentNode;

		private MapEdge currentEdge;
		private int edgeDirection;
		private float edgePosRate; // 0~1

		private MapEdge[] pathList;
		private int pathIndex;
		*/
	public void MoveToNode(MapNode targetNode)
	{
		if(currentNode != null)
		{
			MapEdge[] searchedPath = GraphAstar.SearchPath(currentNode, targetNode);

			pathList = searchedPath;
			pathIndex = 0;
		}
		else if(currentEdge != null)
		{
			MapNode tempNode = new MapNode("-1", GetCurrentViewPosition());
			MapEdge tempEdge1 = new MapEdge(tempNode, currentEdge.node1, currentEdge.type);
			MapEdge tempEdge2 = new MapEdge(tempNode, currentEdge.node2, currentEdge.type);
			tempNode.AddEdge(tempEdge1);
			tempNode.AddEdge(tempEdge2);

			MapEdge[] searchedPath = GraphAstar.SearchPath(tempNode, targetNode);

			pathList = searchedPath;
			pathIndex = 0;
			if(searchedPath.Length > 0)
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

	public void MoveToNode(string targetNodeId)
	{
		MoveToNode(MapGraph.instance.GetNodeById(targetNodeId));
	}

	public void MoveToCreture(CreatureUnit target)
	{
		//MoveToGlobalPos ((Vector2)target.transform.position);
		//MoveToNode(target.GetNode());
		MoveToNode(target.GetWorkspaceNode());
	}
	public void Working(CreatureUnit target)
	{
		state = AgentCmdState.WORKING;
		this.target = target;
		MoveToCreture (target);
	}
	public void FinishWorking()
	{
		state = AgentCmdState.IDLE;
		this.target = null;
	}
/*
	public bool isMoving()
	{
		return movingOnPath;
	}
	*/

	public void OpenStatusWindow()
	{
		AgentStatusWindow.CreateWindow (this);
	}
/*
	void MoveOnPath(AIPoint[] dir)
	{
		Debug.Log("old call");
		if(dir.Length == 0)
			return;
		shortestPath = (AIPoint[])dir.Clone ();
		pathCurIndex = 0;
		movingOnPath = true;
	}
*/
	void MoveOnPath(MapEdge[] path)
	{
		if(path.Length == 0)
			return;

		pathList = path;
		pathIndex = 0;	
	}

	public void Speech(string speechKey)
	{
		string speech;
		if(speechTable.TryGetValue (speechKey, out speech))
		{
			Notice.instance.Send("AddPlayerLog", name + " : " +  speech);
			TextAppearNormalEffect.Create(GetComponent<DestroyHandler>(), new Vector2(0, 0.5f), 4f, name + " : " +  speech, Color.blue);
		}
	}

	public void Panic()
	{
		if(panicType == "default")
		{
			currentPanicAction = new PanicDefaultAction();
		}
		else if(panicType == "roaming")
		{
			currentPanicAction = new PanicRoaming(this);
		}
	}

	public void Die()
	{
		Notice.instance.Send("AgentDie", this);
		Destroy (gameObject);
	}
}
