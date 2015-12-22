using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class OfficerPanicType {
    public static string Stop = "Stop";
    public static string MovetoSefira = "MovetoSefira";
    public static string Wander = "Wander";
}

public class OfficerModel : WorkerModel {
    
    public bool activated;//may be not used

    //not saved
    private static string panic;
    private bool isMoving = false;
    public bool chatWaiting = false;
    public OfficerModel chatTarget;
    private OfficerCmdState state = OfficerCmdState.IDLE;

    public OfficerModel(int id, string area) {
        instanceId = id;
        currentSefira = sefira = area;
        MovableNode = new MovableObjectNode();
        MovableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area));
    }

    public void OnFixedUpdate()
    {
        //make something in OfficeModel.ProcessAction() likewise ProcessAction
        //MovableNode.ProcessMoveNode(movement);
        ProcessAction();
        if (state == OfficerCmdState.DOCUMENT){
            MovableNode.ProcessMoveNode(movement / 2);
        }
        else    MovableNode.ProcessMoveNode(movement);
    }

    public override void ProcessAction()
    {
        if (CurrentPanicAction != null)
        {
            CurrentPanicAction.Execute();
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
                            this.MovableNode.MoveToNode(om.GetConnectedNode());
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
                    MovableNode.MoveToNode(MapGraph.instance.GetSefiraDeptNodes(currentSefira));
                    
                    this.isMoving = true;
                    break;
                case 4:
                    state = OfficerCmdState.IDLE;
                    Debug.Log(this.name + "이동멍");
                    waitTimer = 1.5f + 5 * UnityEngine.Random.value;
                    MovableNode.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
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
                if (!MovableNode.IsMoving() && isMoving) {
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
            if (!MovableNode.IsMoving() && isMoving) {
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
            if (!MovableNode.IsMoving() && isMoving) {
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
            if (!MovableNode.IsMoving() && isMoving) {
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

    public OfficerCmdState GetState() {
        return state;
    }

    public void FinishWorking() {
        state = OfficerCmdState.IDLE;
        this.target = null;
    }
    

}
