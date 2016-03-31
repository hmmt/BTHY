using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class OfficerModel : WorkerModel {
    
    public bool activated;//may be not used
    public int deptNum { get; set; }
    //not saved
    private static string panic;
    private bool isMoving = false;
    private float elapsedTime;
    private static float recoveryTerm = 1f;
    public int mentalReturn;//정신이 돌아오는 기준
    public int recoveryRate;
    public bool chatWaiting = false;
    public OfficerModel chatTarget;
    private OfficerAIState _state = OfficerAIState.START;
	public OfficerAIState state {
		get{ return _state;}
		set{ _state = value; }
	}
    private OfficerUnit _unit;

    public OfficerModel(int id, string area) {
        commandQueue = new WorkerCommandQueue(this);

        instanceId = id;
        currentSefira = sefira = area;
        movableNode = new MovableObjectNode(this);
        movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area));
        recoveryRate = 2;
        elapsedTime = 0.0f;
        this.mentalReturn = SefiraManager.instance.getSefira(area).GetOfficerMentalRecoverValue();
    }

    public override void OnFixedUpdate()
    {
        //make something in OfficeModel.ProcessAction() likewise ProcessAction
        //MovableNode.ProcessMoveNode(movement);
        elapsedTime += Time.deltaTime;
        if (isDead()) return;

		if (moveDelay > 0)
			moveDelay -= Time.deltaTime;
		if(attackDelay > 0)
			attackDelay -= Time.deltaTime;
		if (stunTime > 0) {
			stunTime -= Time.deltaTime;
			return;
		}

        ProcessAction();

        if (elapsedTime > recoveryTerm)
        {
            elapsedTime = 0.0f;
            if (IsInSefira()) {
                //Debug.Log("안에있음");
                RecoverMental(recoveryRate * 2);
            }
            else RecoverMental(recoveryRate);
        }

		if (moveDelay > 0)
		{
			movableNode.ProcessMoveNode(0);
		}
		else if (state == OfficerAIState.DOCUMENT){
            movableNode.ProcessMoveNode(movement / 2);
        }
        else    movableNode.ProcessMoveNode(movement);

    }

    public IEnumerator<WaitForSeconds> StartAction() {
        yield return new WaitForSeconds(5);
        this.state = OfficerAIState.IDLE;
    }

    public override void ProcessAction()
    {
        commandQueue.Execute(this);

        if (CurrentPanicAction != null)
        {
            CurrentPanicAction.Execute();
        }
		else if(unconAction != null)
		{
			unconAction.Execute ();
		}
        else if (state == OfficerAIState.IDLE && waitTimer <= 0 && !isMoving)
        {
            //make next status
            int randState = UnityEngine.Random.Range(0, 5);
            //int randState = 1;
            switch (randState)
			{ 
            case 0:
                state = OfficerAIState.IDLE;
                //Debug.Log(this.name+"멍때림");
                waitTimer = 1.5f + 5 * UnityEngine.Random.value;
                break;
            case 1:
                state = OfficerAIState.MEMO_MOVE;
                this.target = SefiraManager.instance.getSefira(currentSefira).GetIdleCreature();
                if (this.target == null) {
                    //Debug.Log("메모할 곳 없음");
                    waitTimer = 1.5f + 5 * UnityEngine.Random.value;
                    state = OfficerAIState.IDLE;
                    break;
                }
                //this.MoveToCreatureRoom(this.target);
				MoveToNode(this.target.GetEntryNode());

                this.isMoving = true;
                waitTimer = 90f;
                break;
            case 2:
                state = OfficerAIState.CHAT;
                //Debug.Log(this.name + "Chatting");
                waitTimer = 10f;
                this.chatWaiting = true;
                foreach (OfficerModel om in OfficeManager.instance.GetOfficerListBySefira(currentSefira)) {
                    if (om.Equals(this)) continue;
                    if (om.chatWaiting == true) {
                        //Debug.Log(this.name + "있군");
                        this.chatWaiting = om.chatWaiting = false;
                        this.chatTarget = om;
                        om.chatTarget = this;
                        MapNode movetarget = om.GetConnectedNode(); 
                        if (movetarget == null) {
                            //Debug.Log("일단대기");
                            state = OfficerAIState.IDLE;
                            break;
                        }
                        
						//this.movableNode.MoveToNode(om.GetConnectedNode());
						MoveToNode(om.GetConnectedNode());


                        this.isMoving = true;
                        om.isMoving = true;
                        break;
                    }
                }
                if (this.chatWaiting == true) {
                    //Debug.Log(this.name + "없군");
                }
                break;
            case 3:
                state = OfficerAIState.DOCUMENT;
                //Debug.Log(this.name + "Document carrying");
                waitTimer = 90f;
                //MovableNode.MoveToNode(MapGraph.instance.GetSefiraDeptNodes(currentSefira));
                //movableNode.MoveToNode(SefiraManager.instance.getSefira(currentSefira).GetOtherDepartNode(deptNum));
				MoveToNode(SefiraManager.instance.getSefira(currentSefira).GetOtherDepartNode(deptNum));

                _unit.puppetAnim.SetBool("Document", true);
                this.isMoving = true;
                break;
            case 4:
                state = OfficerAIState.IDLE;
               // Debug.Log(this.name + "이동멍");
                waitTimer = 1.5f + 5 * UnityEngine.Random.value;
               //movableNode.MoveToNode(SefiraManager.instance.getSefira(currentSefira).GetDepartNodeByRandom(deptNum));
				MoveToNode(SefiraManager.instance.getSefira(currentSefira).GetDepartNodeByRandom(deptNum));

                ReturnToSefira();
                break;
            }
        }
        else if(state == OfficerAIState.CHAT){
            if (chatWaiting && waitTimer <= 0) {
                chatWaiting = false;
                //Debug.Log(this.name + "아무도없군");
                state = OfficerAIState.IDLE;
            }
            else if (!chatWaiting){
                if (!movableNode.IsMoving() && isMoving) {
                    //Debug.Log(this.name + "도착");
                    isMoving = false;
                    chatTarget.isMoving = false;
                    waitTimer = 10f;
                }
                else if (!isMoving && waitTimer<=0){
                    ReturnToSefiraFromWork();
                    state = OfficerAIState.RETURN;
                    chatTarget.state = OfficerAIState.IDLE;
                    this.isMoving = true;                    
                }
            }
        }
        else if (state == OfficerAIState.DOCUMENT){
            if (!movableNode.IsMoving() && isMoving) {
                isMoving = false;
                waitTimer = 5f;
                _unit.puppetAnim.SetBool("Document", false);
            }
            else if (!isMoving && waitTimer <= 0) {
                ReturnToSefiraFromWork();
                state = OfficerAIState.RETURN;
                isMoving = true;
            }
        }
        else if (state == OfficerAIState.MEMO_MOVE || state == OfficerAIState.MEMO_STAY)
        {
            if (!movableNode.IsMoving() && isMoving) {
                isMoving = false;
                waitTimer = 10f;//can affected by Work Speed
                _unit.puppetAnim.SetBool("Memo", true);
                state = OfficerAIState.MEMO_STAY;
            }
            else if(!isMoving && waitTimer <= 0){
                ReturnToSefiraFromWork();
                _unit.puppetAnim.SetBool("Memo", false);
                SefiraManager.instance.getSefira(currentSefira).EndCreatureWork(target);
                this.target = null;
                state = OfficerAIState.RETURN;
                isMoving = true;
            }
        }
        else if(state == OfficerAIState.RETURN){
            if (!movableNode.IsMoving() && isMoving) {
                isMoving = false;
                state  =OfficerAIState.IDLE;
                waitTimer = 1.5f + 5 * UnityEngine.Random.value;
            }
        }
        waitTimer -= Time.deltaTime;
    }

	public WorkerCommand GetCurrentCommand()
	{
		return commandQueue.GetCurrentCmd ();
	}

    public void MoveToOtherDept() {
        //자신이 속한 구역 이외의 장소로 이동하게끔 수정

        //MoveToNode(MapGraph.instance.GetSefiraDeptNodes(currentSefira).GetId());
		commandQueue.SetAgentCommand(WorkerCommand.MakeMove(MapGraph.instance.GetSefiraDeptNodes(currentSefira)));
    }

	public void PursueUnconAgent(WorkerModel agent)
	{
		commandQueue.SetAgentCommand (WorkerCommand.MakeUnconPursueAgent (agent));
	}

    public void ReturnToSefiraFromWork() {
        this.ReturnToSefira();
    }

    public OfficerAIState GetState() {
        return state;
    }

    public override void TakeMentalDamage(int damage)
    {
        base.TakeMentalDamage(damage);
        if (mental < 0)
        {
            state = OfficerAIState.PANIC;
            Panic();
        }
    }

    public override void RecoverMental(int amount)
    {
        base.RecoverMental(amount);
        if (mental > mentalReturn && state == OfficerAIState.PANIC) {
            state = OfficerAIState.IDLE;
            this.CurrentPanicAction = null;
            ReturnToSefira();
        }
    }

    public override void Panic()
    {
        this.CurrentPanicAction = new PanicReady(this);
    }

    public override void ReturnToSefira()
    {

        MoveToNode(SefiraManager.instance.getSefira(currentSefira).GetDepartNodeByRandom(deptNum).GetId());
    }

	public override void LoseControl()
	{
		state = OfficerAIState.CANNOT_CONTROLL;
		commandQueue.Clear ();
	}

	public override void GetControl()
	{
		if (state == OfficerAIState.CANNOT_CONTROLL)
		{
			Debug.Log ("get Control....");
			state = OfficerAIState.IDLE;
			commandQueue.Clear ();
		}
	}

	public override void ClearUnconCommand()
	{
		if (state == OfficerAIState.CANNOT_CONTROLL)
		{
			commandQueue.Clear ();
		}
	}

	public override void SetUncontrollableAction(UncontrollableAction uncon)
	{
		unconAction = uncon;

		if (unconAction != null)
			unconAction.Init ();
	}

    public override void PanicReadyComplete()
    {
        //
        //Debug.Log("complete");
        int i = UnityEngine.Random.Range(0, 4);

        switch (i) { 
            case 0:
                //Debug.Log("스테이");
                CurrentPanicAction = new PanicStay(this);
                return;
            case 1:
                //Debug.Log("방황");
                CurrentPanicAction = new PanicWander(this);
                return;
            case 2:
                //Debug.Log("돌아간다");
                CurrentPanicAction = new PanicReturn(this);
                return;
        }
    }

    public void SetUnit(OfficerUnit unit) {
        this._unit = unit;
    }

	public void OnClick()
	{
		if (unconAction != null) {
			unconAction.OnClick ();
		}
	}

	public override void OnDie()
	{
		if (unconAction != null) {
			unconAction.OnDie ();
		}
	}
}
