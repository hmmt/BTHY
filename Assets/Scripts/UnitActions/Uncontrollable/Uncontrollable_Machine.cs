using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Uncontrollable_Machine : UncontrollableAction {
    private WorkerModel model;
    public SingingMachineSkill machineSkill;

    private float waitTimer = 0;
    public WorkerModel target = null;
    private float startWaitTimer = 6f;
    private Animator puppetAnim;

    bool drag = false;
    bool listen = false;
    WorkerModel victim = null;
    Animator victimAnim = null;
    Animator creatureAnim = null;
    float listenDelay = 0f;
    float listenTime = 15f;

    public Uncontrollable_Machine(WorkerModel model, SingingMachineSkill machineSkill){
        this.model = model;
        this.machineSkill = machineSkill;
        this.creatureAnim = CreatureLayer.currentLayer.GetCreature(machineSkill.model.instanceId).creatureAnimator;
    }

    public override void Init()
    {
        if (model is AgentModel)
        {
            AgentUnit agentView = AgentLayer.currentLayer.GetAgent(this.model.instanceId);
            this.puppetAnim = agentView.puppetAnim;
            //agentView.puppetAnim.SetInteger("Type", startType);
        }
        else if (model is OfficerModel) {
            OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            this.puppetAnim = officerView.puppetAnim;
            //officerView.puppetAnim.SetInteger("Type", startType);
        }
    }

    public override void Execute()
    {
		Notice.instance.Send (NoticeName.EscapeCreature);
        if (startWaitTimer > 0) {
            startWaitTimer -= Time.deltaTime;
            return;
        }
        if (!this.drag && !this.listen)
        {
            if (model is AgentModel)
            {
                if ((waitTimer <= 0 && target == null)
                    || ((AgentModel)model).GetCurrentCommand() == null)
                {
                    model.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(model.currentSefira));
                    waitTimer = 1.5f + UnityEngine.Random.value;
                }
            }
            else
            {
                if ((waitTimer <= 0 && target == null)
                    || ((OfficerModel)model).GetCurrentCommand() == null)
                {
                    model.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(model.currentSefira));
                    waitTimer = 1.5f + UnityEngine.Random.value;
                }
            }

            waitTimer -= Time.deltaTime;

            if (target == null)
            {
				List<WorkerModel> detectedWorkers = new List<WorkerModel> (AgentManager.instance.GetNearAgents (model.GetMovableNode ()));
				detectedWorkers.AddRange (OfficerManager.instance.GetNearOfficers (model.GetMovableNode ()));

				if (detectedWorkers.Count > 0) {
					//PursueWorker (detectedAgents [0]);

					WorkerModel nearest = null;
					float nearestDist = 100000;
					foreach (WorkerModel worker in detectedWorkers)
					{
						if (worker.GetMovableNode ().GetPassage () == null)
							continue;

						if (worker == model)
							continue;

						if (worker.unconAction is Uncontrollable_Machine)
							continue;

						if (machineSkill.ContainsAttackTarget (worker))
							continue;

						Vector3 v = worker.GetCurrentViewPosition () - model.GetCurrentViewPosition ();

						float m = v.magnitude;

						if (nearestDist > m) {
							nearestDist = m;
							nearest = worker;
						}
					}

					if (nearest != null)
					{
						if (model is AgentModel)
							((AgentModel)model).PursueUnconAgent (nearest);
						else if (model is OfficerModel)
							((OfficerModel)model).PursueUnconAgent (nearest);
					}
				}
            }
        }
        else if(this.drag == true) {
            Drag();
        }
        else if (this.listen == true) {
            listenDelay += Time.deltaTime;
            if (listenDelay > listenTime) {
                listenDelay = 0f;
                this.listen = false;

				this.puppetAnim.SetBool("Kill", false);
            }
        }
    }


    public override void OnDie()
    {
        //machine remove this worker in lists
        //make dead animation
		if (this.drag)
		{
			FailDrag ();
		}
        machineSkill.OnAttractedTargetTerminated(this.model);
    }

    public override void OnClick()
    {
		/*
        if (model is OfficerModel) {
            Debug.Log("officer model is not ready");
            return;
        }
        */

		if(model is AgentModel)
        	SuppressWindow.CreateWindow((AgentModel)model);
		else
			SuppressWindow.CreateWindow((OfficerModel)model);
    }

    float moveDelayTimer = 0;
	float underattackTimer = 0;
    public void Drag() { 
        //끌고가서 드랍하고
        //반복
        
        //격리실도착하면Drop시전

		if (underattackTimer > 0)
		{
			model.StopAction ();
			victim.StopAction ();
			underattackTimer -= Time.deltaTime;
			return;
		}

        if (moveDelayTimer > 0)
        {
            moveDelayTimer -= Time.deltaTime;
        }

        if (moveDelayTimer <= 0)
        {
			if (creatureAnim.GetBool("Kill"))
				model.MoveToNode (machineSkill.model.GetEntryNode ());
			else
				model.MoveToNode (machineSkill.model.GetWorkspaceNode ());

			victim.StopStun ();
			//victim.MoveToMovable (model.GetMovableNode ());
			victim.FollowMovable(model.GetMovableNode());

            moveDelayTimer = 0.5f;
        }

		if (model.GetCurrentNode() == machineSkill.model.GetWorkspaceNode() && !creatureAnim.GetBool("Kill"))
        {
            Drop();
        }
    }

    public void Drop() {
        //드랍하면 기계가 갈기 시작
        //노래또만들고

        Debug.Log("Dropped");

        if (this.model is AgentModel) {
            AgentUnit agentView = AgentLayer.currentLayer.GetAgent(model.instanceId);

            agentView.SetParameterOnce("Drop", true);
			agentView.animTarget.FlipDirection (false);
        }
        else if (this.model is OfficerModel) {
            OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            officerView.SetParameterOnce("Drop", true);
			officerView.animTarget.FlipDirection (false);
        }

		if (victim is AgentModel) {
			AgentUnit unit = AgentLayer.currentLayer.GetAgent (victim.instanceId);
			unit.animTarget.FlipDirection (false);
		}
		else if (victim is OfficerModel) {
			OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer (victim.instanceId);
			unit.animTarget.FlipDirection (false);
		}


		//AnimatorManager.instance.ChangeAnimatorByID(this.victim.instanceId, AnimatorName.id_Machine_victim, victimAnim, true, false);
		//victimAnim.SetInteger("Type", 1);
		victimAnim.SetBool("Drop", true);
		this.victim.SetInvincible (false);
		//this.victim.TakePhysicalDamageByCreature (this.victim.maxHp);
		this.victim.Die();
		this.victim.ClearUnconCommand ();


        this.machineSkill.AttractSkillActivate(this.victim);
        this.puppetAnim.SetBool("Kill", true);
        this.drag = false;

		machineSkill.RemoveAttackTarget (victim);
        this.victim = null;
        this.listen = true;
    }

    public void StartDrag(WorkerModel victim) {
        this.victim = victim;
		machineSkill.AddAttackTarget (victim);

        this.drag = true;
        this.moveDelayTimer = 0;

		this.victim.movementMul = (float)this.model.movement / (float)this.victim.movement * 1.1f;
		Debug.Log ("set movementMul : " + victim.movementMul);
        
        //격리실로 이동 시작
        if (this.model is AgentModel) {
            AgentUnit agentView = AgentLayer.currentLayer.GetAgent(model.instanceId);

            agentView.SetParameterOnce("Drag", true);
			agentView.animTarget.FlipDirection (true);
        }
        else if (this.model is OfficerModel) {
            OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            officerView.SetParameterOnce("Drag", true);
			officerView.animTarget.FlipDirection (true);
        }

        if (victim is AgentModel) {
			(victim as AgentModel).ResetAnimator ();
			AgentUnit unit = AgentLayer.currentLayer.GetAgent (victim.instanceId);
			this.victimAnim = unit.puppetAnim;
			unit.animTarget.FlipDirection (true);
        }
        else if (victim is OfficerModel) {
			OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer (victim.instanceId);
            this.victimAnim = OfficerLayer.currentLayer.GetOfficer(victim.instanceId).puppetAnim;
			unit.animTarget.FlipDirection (true);
        }

        AnimatorManager.instance.ChangeAnimatorByName(victim.instanceId, "Machine_victim", victimAnim, true, false);
		victimAnim.SetInteger ("Type", 2);
    }

	public void FailDrag()
	{
		if (!drag) {
			return;
		}

		victim.movementMul = 1;

		if (victim is AgentModel) {
			AgentUnit unit = AgentLayer.currentLayer.GetAgent (victim.instanceId);
			unit.animTarget.FlipDirection (false);
		}
		else if (victim is OfficerModel) {
			OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer (victim.instanceId);
			unit.animTarget.FlipDirection (false);
		}
		victim.ResetAnimator ();
		victim.GetControl ();
		machineSkill.RemoveAttackTarget (victim);
		victim = null;
		drag = false;
	}

	public SingingMachineSkill GetMachineSkill()
	{
		return machineSkill;
	}

	public override void UnderAttack()
	{
		if(this.drag)
		{
			underattackTimer = 3.0f;
			Drag ();
		}
	}

}
