using UnityEngine;
using System.Collections.Generic;

public class AgentModel {

    public int instanceId { get;  private set; }

    // game data

    //public TraitTypeInfo metaTraitInfo;

    public AgentTypeInfo metadata;
    public long metadataId;
    public string name;
    public int hp;

    public Animator anim;

    //public TraitTypeInfo[] traitList;

    public List<TraitTypeInfo> traitList;

    public string gender;
    public int level;
    public int workDays;

    public int expFail = 0;
    public int expSuccess = 0;
    public int expHpDamage = 0;
    public int expMentalDamage = 0;

    public int maxHp;
    public int maxMental;

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

    public string imgsrc {private set; get;}

    public Dictionary<string, string> speechTable = new Dictionary<string, string>();

    public string panicType;
    //

    public string currentSefira { private set; get; }

    private AgentCmdState state = AgentCmdState.IDLE;
    public CreatureModel target; // state; MOVE, WORKING

    private PanicAction currentPanicAction;

    // 

    // path finding2
    private MapNode currentNode;

    private MapEdge currentEdge;
    public float edgePosRate; // 0~1

    public int edgeDirection; //?

    //	private GraphPosition graphPosition;

    private MapEdge[] pathList;
    private int pathIndex;
   

    public AgentModel(int instanceId, string area)
    {
        traitList = new List<TraitTypeInfo>();
        this.instanceId = instanceId;
        //currentSefira = area;
        SetCurrentSefira(area);
        currentNode = MapGraph.instance.GetSepiraNodeByRandom(area);
    }

    private bool visible = true;
    private float oldZ;

    ////

    private float waitTimer = 0;


    // 현재 AgentUnit에서 호출됨
    public void FixedUpdate()
    {
        ProcessAction();

        ProcessMoveNode();
    }

    public Vector2 GetCurrentViewPosition()
    {
        Vector2 output = new Vector2(0,0);

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

    // if map is destroyed....?

    public void applyTrait(TraitTypeInfo addTrait)
    {
        maxHp += addTrait.hp;
        maxMental += addTrait.mental;
        movement += addTrait.moveSpeed;
        work += addTrait.workSpeed;
    }

    private void ProcessAction()
    {
        if (currentPanicAction != null)
        {
            currentPanicAction.Execute();
        }
        else if (state == AgentCmdState.IDLE)
        {
            if (waitTimer <= 0)
            {

                MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));

                waitTimer = 1.5f + Random.value;
            }
        }
        else if (state == AgentCmdState.WORKING)
        {
            if (pathList == null && currentNode != target.GetWorkspaceNode())
            {
                MoveToCreture(target);
            }
        }
        waitTimer -= Time.deltaTime;
    }
    private void ProcessMoveNode()
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
    public AgentCmdState GetState()
    {
        return state;
    }
    public void ReturnToSefira()
    {
        SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
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

    public void MoveToNode(string targetNodeId)
    {
        MoveToNode(MapGraph.instance.GetNodeById(targetNodeId));
    }

    public void MoveToCreture(CreatureModel target)
    {
        //MoveToGlobalPos ((Vector2)target.transform.position);
        //MoveToNode(target.GetNode());
        MoveToNode(target.GetWorkspaceNode());
    }
    public void Working(CreatureModel target)
    {
        state = AgentCmdState.WORKING;
        this.target = target;
        MoveToCreture(target);
    }
    public void FinishWorking()
    {
        state = AgentCmdState.IDLE;
        this.target = null;
    }
    void MoveOnPath(MapEdge[] path)
    {
        if (path.Length == 0)
            return;

        pathList = path;
        pathIndex = 0;
    }

    public void TakePhysicalDamage(int damage)
    {
        Debug.Log(name + " takes PHYSICAL dmg " + damage);
        hp -= damage;
    }

    public void TakeMentalDamage(int damage)
    {
        Debug.Log(name + " takes MENTAL dmg " + damage);
        mental -= damage;
    }

    public bool HasTrait(long id)
    {
        foreach (TraitTypeInfo info in traitList)
        {
            if (info.id == id)
                return true;
        }
        return false;
    }

    public void SetCurrentSefira(string sefira)
    {
        currentSefira = sefira;
        switch (currentSefira)
        {
            case "1": imgsrc = "Agent/Malkuth/0"; break;
            case "2": imgsrc = "Agent/Nezzach/00"; break;
            case "3": imgsrc = "Agent/Hodd/00"; break;
            case "4": imgsrc = "Agent/Yessod/00"; break;
        }
        waitTimer = 0;
    }

    public void Panic()
    {
        /*
        if (panicType == "default")
        {
            currentPanicAction = new PanicDefaultAction();
            string narration = this.name + " (이)가 공황에 빠져 우두커니 서있습니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
        else if (panicType == "roaming")
        {
            currentPanicAction = new PanicRoaming(this);
            string narration = this.name + " (이)가 공황에 빠져 방향을 잃고 배회합니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
         * */
    }

    public void Die()
    {
        string narration = this.name + " (이)가 사망했습니다.";

        Notice.instance.Send("AddSystemLog", narration);
        Notice.instance.Send("AgentDie", this);

        // temp?
        AgentManager.instance.RemoveAgent(this);
    }
}
