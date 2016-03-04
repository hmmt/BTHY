using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorkerModel: ObjectModelBase, IObserver {
    public int instanceId;

	protected WorkerCommandQueue commandQueue;
    
    public string name;
    public int hp;
    public int mental;

    public string gender;
    public int maxHp;
    public int maxMental;

    public int movement;

    public string sefira;//assigned
    public string currentSefira;//now position

    public string panicType;

    public string imgsrc;

    public string faceSpriteName;
    public string hairSpriteName;
    public string bodySpriteName;
    public string panicSpriteName;

    public string hairImgSrc;
    public string faceImgSrc;
    public string bodyImgSrc;

    //public AgentHistory history;

    public Dictionary<string, string> speechTable = new Dictionary<string, string>();

    //CmdState will be needed

    public CreatureModel target;
    public WorkerModel targetWorker;
    private PanicAction currentPanicAction;
    public PanicAction CurrentPanicAction { 
        get {
            return currentPanicAction;
        }
        set {
            currentPanicAction = value;
        }
    }
	private MovableObjectNode _movableNode;
    public MovableObjectNode movableNode { 
        get{
            return _movableNode;
        }
        set {
            _movableNode = value;
        }
    }


    public bool visible = true;
    public float oldZ;
    public float waitTimer = 0;
    public bool panicFlag = false;
    
    public WorkerModel() { }

    public WorkerModel(int instanceId, string area) {
        this.currentSefira = area;
        this.sefira = area;
        this.instanceId = instanceId;
        this._movableNode = new MovableObjectNode();
        this._movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area));
    }

    public WorkerModel(int instanceId, Sefira area) {
        this.sefira = area.indexString;
        this.instanceId = instanceId;
        this._movableNode = new MovableObjectNode();
        this._movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area.indexString));
        this.currentSefira = area.indexString;
    }

    /*
    public WorkerModel(int instanceId, string area)
    {
        this.sefira = area;
        this.instanceId = instanceId;
        this.movableNode = new MovableObjectNode();
        this.movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area.indexString));
        this.currentSefira = area.indexString;
    }*/

    public virtual Dictionary<string, object> GetSaveData() {
        Dictionary<string, object> output = new Dictionary<string, object>();
        output.Add("instanceId", instanceId);
        output.Add("currentSefira", currentSefira);
        output.Add("name", name);
        output.Add("hp", hp);
        output.Add("movement", movement);
        output.Add("gender", gender);
        output.Add("mental", mental);
        output.Add("maxHp", maxHp);
        output.Add("maxMental", maxMental);
        output.Add("sefira", sefira);
        
        output.Add("imgsrc", imgsrc);
        output.Add("speechTable", speechTable);
        output.Add("panicType", panicType);

        return output;
    }

    public static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field){
        object output;
        if (dic.TryGetValue(name, out output)){
            field = (T)output;
            return true;
        }
        return false;
    }

    public virtual void LoadData(Dictionary<string, object> dic) {

        TryGetValue(dic, "instanceId", ref instanceId);
        TryGetValue(dic, "name", ref name);
        TryGetValue(dic, "hp", ref hp);
        TryGetValue(dic, "mental", ref mental);
        TryGetValue(dic, "gender", ref gender);
        TryGetValue(dic, "movement", ref movement);
        TryGetValue(dic, "imgsrc", ref imgsrc);
        TryGetValue(dic, "speechTable", ref speechTable);
        TryGetValue(dic, "panicType", ref panicType);
        TryGetValue(dic, "currentSefira", ref currentSefira);
        
        TryGetValue(dic, "maxHp", ref maxHp);
        TryGetValue(dic, "maxMental", ref maxMental);
        TryGetValue(dic, "sefira", ref sefira);
        
    }

    public virtual void GetPortrait(string parts, string key)
    {
        switch (parts)
        {
            case "hair":
                hairImgSrc = "Sprites/Agent/Hair/Hair_M_" + key + "_00";
                break;
            case "face":
                faceImgSrc = "Sprites/Agent/Face/Face_" + key + "_00";
                break;
            case "body":
                if (currentSefira == "0")
                {
                    bodyImgSrc = "Sprites/Agent/Body/Body_1_S_00";
                }
                else bodyImgSrc = "Sprites/Agent/Body/Body_" + currentSefira + "_S_00";
                break;
            default:
                throw new Exception("Make Portrait Part parameter exception");
        }
    }

    public virtual void OnFixedUpdate() { 
        ProcessAction();
        _movableNode.ProcessMoveNode(movement);
    }

    public virtual void ProcessAction() { 
        
    }

    public virtual MovableObjectNode GetMovableNode()
    {
        return _movableNode;
    }

    public virtual Vector3 GetCurrentViewPosition()
    {
        return _movableNode.GetCurrentViewPosition();
    }

    public virtual MapNode GetCurrentNode()
    {
        return _movableNode.GetCurrentNode();
    }

    public virtual void SetCurrentNode(MapNode node)
    {
        _movableNode.SetCurrentNode(node);
    }

    public virtual MapEdge GetCurrentEdge()
    {
        return _movableNode.GetCurrentEdge();
    }

    public virtual int GetEdgeDirection()
    {
        return _movableNode.GetEdgeDirection();
    }

    //Get Command State

    public virtual void ReturnToSefira() {
        SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
    }

	public void MoveToNode(MapNode targetNode)
	{
		commandQueue.SetAgentCommand(WorkerCommand.MakeMove(targetNode));
	}

    public void MoveToNode(string targetNodeID)
    {
        //movableNode.MoveToNode(MapGraph.instance.GetNodeById(targetNodeID));
		commandQueue.SetAgentCommand(WorkerCommand.MakeMove(MapGraph.instance.GetNodeById(targetNodeID)));
    }

    public virtual bool isDead()
    {
        return hp <= 0;
    }

    public virtual void Attacked() { 
        
    }

    public virtual void WorkEscape()
    { 
    
    }

    public virtual void Working()
    { 
    
    }

    public virtual void FinishWorking()
    { 
    
    }

    public MapNode GetConnectedNode() {
        MapNode pos = this._movableNode.GetCurrentNode();
        //Debug.Log(pos);
        MapNode connected = null;
        if(pos == null) return null;
        foreach (MapEdge me in pos.GetEdges()) {
            if (me.type == "door") continue;
            connected = me.ConnectedNode(pos);
        }
        return connected;
    }

    public virtual void TakePhysicalDamage(int damage) {
        Debug.Log("TakePhysicalDamage : " + damage);
        hp -= damage;
        if (hp <= 0) { 
            //dead
        }
    }

    public virtual void TakeMentalDamage(int damage) {
        mental -= damage;
        
    }

    public virtual void RecoverHP(int amount){
        hp += amount;
        hp = hp> maxHp? maxHp: hp;
    }

    public virtual void RecoverMental(int amount){
        mental += amount;
        mental = mental> maxMental? maxMental: mental;
    }

    public virtual void Panic() { 
        
    }

    public virtual void Die()
    {
        this.hp = 0;
        //state setting
    }

	public override void InteractWithDoor(DoorObjectModel door)
	{
		base.InteractWithDoor(door);

		commandQueue.AddFirst(WorkerCommand.MakeOpenDoor(door));
	}

    public static int CompareByName(WorkerModel x, WorkerModel y)
    {
        if (x == null || y == null)
        {
            Debug.Log("Errror in comparison by name");
            return 0;
        }
        if (x.name == null)
        {
            if (y.name == null) return 0;
            else return -1;
        }
        else
        {
            if (y.name == null) return 1;
            else
            {
                return x.name.CompareTo(y.name);
            }
        }
    }

    public virtual void SetPanicState() {
        this.panicFlag = true;
    }

    public virtual void PanicReadyComplete() {
        return;
    }

    public virtual bool IsInSefira() {
        MapNode node = movableNode.GetCurrentNode();
        MapEdge edge = movableNode.GetCurrentEdge();
        //MapNode[] sefiraNodes = SefiraManager.instance.getSefira(currentSefira)
        if (node != null)
        {
            string name = node.GetId();
            int index = name.IndexOf("sefira");
            //Debug.Log(index);
            if (index != -1)
            {
                return true;
            }
        }
        else {
            string edge1, edge2;
            edge1 = edge.node1.GetId();
            edge2 = edge.node2.GetId();
            //Debug.Log(edge1 + " " + edge2);

            if ((edge1.IndexOf("sefira") != -1) && (edge2.IndexOf("sefira") != -1))
            {
                //Debug.Log("맞지롱");
                return true;
            }
        }

        return false;
    }


	public virtual void OnNotice(string notice, params object[] param){
		if(notice == NoticeName.FixedUpdate){
			OnFixedUpdate();
		}
	}


	//// static methods

	public static int CompareByID(WorkerModel x, WorkerModel y){
		if (x == null || y == null){
			Debug.Log("Error to compare");
			return 0;
		}
		return x.instanceId.CompareTo(y.instanceId);
	}

	public static int CompareBySefira(WorkerModel x, WorkerModel y){
		if (x == null || y == null){
			Debug.Log("Error to compare");
			return 0;
		}
		int xIndex = SefiraManager.instance.getSefira(x.sefira).index;
		int yIndex = SefiraManager.instance.getSefira(y.sefira).index;
		return xIndex.CompareTo(yIndex);
	}
}
