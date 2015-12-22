using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class OfficerModel : IObserver {

    public int instanceID;

    public string name;
    public int hp;
    public int mental;
    public int movement;
    public int workSpeed;

    public int maxHp;
    public int maxMental;

    public string gender;
    public string sefira;
    public Dictionary<string, string> speechTable = new Dictionary<string, string>();

    public string currentSefira;
    public string panicType;

    public bool activated;//may be not used
    public string imgsrc;
    public string faceSpriteName;
    public string hairSpriteName;
    public string bodySpriteName;
    public string panicSpriteName;

    public string hairImgSrc;
    public string faceImgSrc;
    public string bodyImgSrc;

    //not saved
    private bool isMoving = false;
    public bool chatWaiting = false;
    public OfficerModel chatTarget;
    private OfficerCmdState state = OfficerCmdState.IDLE;

    public CreatureModel target;
        
    private PanicAction currentPanicAction;

    MovableObjectNode movableNode;

    public OfficerModel(int instanceID, string area) {
        this.movableNode = new MovableObjectNode();

        this.instanceID = instanceID;
        currentSefira = area;
        movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area));

    }

    private bool visible = true;
    private float oldZ;

    private float waitTimer = 0;

    public Dictionary<string, object> GetSaveData() {
        Dictionary<string, object> output = new Dictionary<string, object>();

        output.Add("instanceId", instanceID);
        output.Add("curretnSefira", currentSefira);
        output.Add("name", name);
        output.Add("hp", hp);
        output.Add("movement", movement);
        output.Add("gender", gender);
        output.Add("mental", mental);
        output.Add("maxHp", maxHp);
        output.Add("maxMental", maxMental);
        output.Add("sefira", sefira);
        output.Add("workSpeed", workSpeed);
        output.Add("imgsrc", imgsrc);
        output.Add("speechTable", speechTable);
        output.Add("panicType", panicType);
        return output;
    }

    private static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field) {
        object output;
        if (dic.TryGetValue(name, out output)){
            field = (T)output;
            return true;
        }
        return false;
    }
    
    public void LoadData(Dictionary<string, object> dic) {
        TryGetValue(dic, "instanceID", ref instanceID);
        TryGetValue(dic, "name", ref name);
        TryGetValue(dic, "hp", ref hp);
        TryGetValue(dic, "gender", ref gender);
        TryGetValue(dic, "mental", ref mental);
        TryGetValue(dic, "movement", ref movement);
        TryGetValue(dic, "imgsrc", ref imgsrc);
        TryGetValue(dic, "speechTable", ref speechTable);
        TryGetValue(dic, "panicType", ref panicType);
        TryGetValue(dic, "currentSefira", ref currentSefira);
        TryGetValue(dic, "workSpeed", ref workSpeed);
        TryGetValue(dic, "maxHp", ref maxHp);
        TryGetValue(dic, "maxMental", ref maxMental);
        TryGetValue(dic, "sefira", ref sefira);
    }

    public void OfficerPortrait(string parts, string key) {
        switch (parts) { 
            case "hair":
                hairImgSrc = "Sprites/Agent/Hair/Hair_M_" + key + "_00";
                break;
            case "face":
                faceImgSrc = "Sprites/Agent/Face/Face_" + key + "_00";
                break;
            case "body":
                if (currentSefira == "0") {
                    bodyImgSrc = "Sprites/Agent/Body/Body_1_S_00";
                }
                else bodyImgSrc = "Sprites/Agent/Body/Body_" + currentSefira + "_S_00";
                break;
            default:
                throw new Exception("Make Portrait Part parameter exception");           
        }
    }

    public void OnFixedUpdate()
    {
        //make something in OfficeModel.ProcessAction() likewise ProcessAction
        //movableNode.ProcessMoveNode(movement);
        ProcessAction();
        if (state == OfficerCmdState.DOCUMENT){
            movableNode.ProcessMoveNode(movement / 2);
        }
        else    movableNode.ProcessMoveNode(movement);
    }

    private void ProcessAction() {
        if (currentPanicAction != null)
        {
            currentPanicAction.Execute();
        }
        else if (state == OfficerCmdState.IDLE && waitTimer <= 0 && !isMoving)
        {
            //make next status
            int randState = UnityEngine.Random.Range(0, 5);
            switch (randState) { 
                case 0:
                    state = OfficerCmdState.IDLE;
                    Debug.Log(this.name+"멍때림");
                    waitTimer = 1.5f + 5 * UnityEngine.Random.value;
                    break;
                case 1:
                    state = OfficerCmdState.MEMO;
                    this.target = SefiraManager.instance.getSefira(currentSefira).GetIdleCreature();
                    if (this.target == null) {
                        Debug.Log("메모할 곳 없음");
                        waitTimer = 1.5f + 5 * UnityEngine.Random.value;
                        state = OfficerCmdState.IDLE;
                        break;
                    }
                    MoveToCreatureRoom(this.target);
                    this.isMoving = true;
                    waitTimer = 90f;
                    break;
                case 2:
                    state = OfficerCmdState.CHAT;
                    Debug.Log(this.name + "Chatting");
                    waitTimer = 10f;
                    this.chatWaiting = true;
                    foreach (OfficerModel om in OfficeManager.instance.GetOfficerListBySefira(currentSefira)) {
                        if (om.Equals(this)) continue;
                        if (om.chatWaiting == true) {
                            Debug.Log(this.name + "있군");
                            this.chatWaiting = om.chatWaiting = false;
                            this.chatTarget = om;
                            om.chatTarget = this;
                            this.movableNode.MoveToNode(om.getConnectedNode());
                            this.isMoving = true;
                            om.isMoving = true;
                            break;
                        }
                    }
                    if (this.chatWaiting == true) {
                        Debug.Log(this.name + "없군");
                    }
                    break;
                case 3:
                    state = OfficerCmdState.DOCUMENT;
                    Debug.Log(this.name + "Document carrying");
                    waitTimer = 90f;
                    string str = MapGraph.instance.GetSefiraDeptNodes(currentSefira).GetId();
                    Debug.Log(str);
                    movableNode.MoveToNode(MapGraph.instance.GetSefiraDeptNodes(currentSefira));
                    
                    this.isMoving = true;
                    break;
                case 4:
                    state = OfficerCmdState.IDLE;
                    Debug.Log(this.name + "이동멍");
                    waitTimer = 1.5f + 5 * UnityEngine.Random.value;
                    movableNode.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
                    break;
            }
        }
        else if(state == OfficerCmdState.CHAT){
            if (chatWaiting && waitTimer <= 0) {
                chatWaiting = false;
                Debug.Log(this.name + "아무도없군");
                state = OfficerCmdState.IDLE;
            }
            else if (!chatWaiting){
                if (!movableNode.IsMoving() && isMoving) {
                    Debug.Log(this.name + "도착");
                    isMoving = false;
                    chatTarget.isMoving = false;
                    waitTimer = 10f;
                }
                else if (!isMoving && waitTimer<=0){
                    ReturnToSefiraFromWork();
                    state = OfficerCmdState.RETURN;
                    chatTarget.state = OfficerCmdState.IDLE;
                    this.isMoving = true;                    
                }
            }
        }
        else if (state == OfficerCmdState.DOCUMENT){
            if (!movableNode.IsMoving() && isMoving) {
                isMoving = false;
                waitTimer = 5f;
            }
            else if (!isMoving && waitTimer <= 0) {
                ReturnToSefiraFromWork();
                state = OfficerCmdState.RETURN;
                isMoving = true;   
            }
        }
        else if (state == OfficerCmdState.MEMO)
        {
            if (!movableNode.IsMoving() && isMoving) {
                isMoving = false;
                waitTimer = 10f;//can affected by Work Speed
            }
            else if(!isMoving && waitTimer <= 0){
                ReturnToSefiraFromWork();
                SefiraManager.instance.getSefira(currentSefira).EndCreatureWork(target);
                this.target = null;
                state = OfficerCmdState.RETURN;
                isMoving = true;
            }
        }
        else if(state == OfficerCmdState.RETURN){
            if (!movableNode.IsMoving() && isMoving) {
                isMoving = false;
                state  =OfficerCmdState.IDLE;
                waitTimer = 1.5f + 5 * UnityEngine.Random.value;
            }
        }
        waitTimer -= Time.deltaTime;
    }

    public void MoveToOtherDept() {
        MoveToNode(MapGraph.instance.GetSefiraDeptNodes(currentSefira).GetId());
    }

    public void ReturnToSefiraFromWork() {
        MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira).GetId());
    }

    public MovableObjectNode GetMovableNode() {
        return movableNode;
    }

    public Vector2 GetCurrentViewPosition() {
        return movableNode.GetCurrentViewPosition();
    }

    public MapNode GetCurrentNode() {
        return movableNode.GetCurrentNode();
    }

    public void SetCurrentNode(MapNode node) {
        movableNode.SetCurrentNode(node);
    }

    public MapEdge GetCurrentEdge() {
        return movableNode.GetCurrentEdge();
    }

    public int GetEdgeDirection() {
        return movableNode.GetEdgeDirection();
    }

    public OfficerCmdState GetState() {
        return state;
    }

    

    public void ReturnToSefira() {
        SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
    }

    public void MoveToNode(string targetNodeID) { 
        movableNode.MoveToNode(MapGraph.instance.GetNodeById(targetNodeID));
    }

    //may not used like agentModel. Otherwise, other implementaion may needed
    public void MoveToCreature() { 
    
    }

    public void MoveToCreatureRoom(CreatureModel target) {
        movableNode.MoveToNode(target.GetWorkspaceNode());
        
    }

    public bool isDead() {
        return hp <= 0;
    }

    public void Attacked() { 
        
    }

    public void WorkEscape() { 
    
    }

    public void Working() { 
        
    }

    public void FinishWorking() {
        state = OfficerCmdState.IDLE;
        this.target = null;
    }

    public MapNode getConnectedNode() {
        MapNode pos = this.movableNode.GetCurrentNode();
        MapNode connected = null;

        foreach (MapEdge me in pos.GetEdges()) {
            if (me.type == "door") continue;
            connected = me.ConnectedNode(pos);
        }

        return connected;
    }

    public void TakePhysicalDamage(int damgage){
        hp -= damgage;
        if (hp <= 0) { 
            //die
        }
    }

    public void TakeMentalDamage(int damage) {
        mental -= damage;
        if (mental <= 0) { 
            
        }
    }

    public void RecoverHP(int amount) {
        hp += amount;
        //make max hp
    }

    public void RecoverMental(int amount) {
        mental += amount;
        //make max mental
    }

    //not make setcurrentSefira 

    public void Panic() { 
    
    }

    public void Die() {
        this.hp = 0;
        this.state = OfficerCmdState.DEAD;
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.FixedUpdate)
        {
            OnFixedUpdate();
        }
    }
}
