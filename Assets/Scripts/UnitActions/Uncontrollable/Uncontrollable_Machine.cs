using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Uncontrollable_Machine : UncontrollableAction {
    private WorkerModel model;
    private SingingMachineSkill machineSkill;

    private float waitTimer = 0;
    private WorkerModel target = null;
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
                AgentModel[] nears = AgentManager.instance.GetNearAgents(model.GetMovableNode());

                List<AgentModel> filteredAgents = new List<AgentModel>();
                foreach (AgentModel nearAgent in nears)
                {
                    if (nearAgent != model)
                        filteredAgents.Add(nearAgent);
                }

                if (filteredAgents.Count > 0)
                {
                    target = filteredAgents[0];
                    if (model is AgentModel)
                        ((AgentModel)model).PursueUnconAgent(target as AgentModel);
                    else
                        ((OfficerModel)model).PursueUnconAgent(target as AgentModel);
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
            }
        }
        


    }


    public override void OnDie()
    {
        //machine remove this worker in lists
        //make dead animation
        machineSkill.OnAttractedTargetTerminated(this.model);
    }

    public override void OnClick()
    {
        if (model is OfficerModel) {
            Debug.Log("officer model is not ready");
            return;
        }

        SuppressWindow.CreateWindow((AgentModel)model);
    }

    float moveDelayTimer = 0;
    public void Drag() { 
        //끌고가서 드랍하고
        //반복
        
        //격리실도착하면Drop시전

        if (moveDelayTimer > 0)
        {
            moveDelayTimer -= Time.deltaTime;
        }

        if (moveDelayTimer <= 0)
        {
            model.MoveToNode(machineSkill.model.GetWorkspaceNode());
			//victim.MoveToMovable (model.GetMovableNode ());
			victim.FollowMovable(model.GetMovableNode());

            moveDelayTimer = 1.5f;
        }

        if (model.GetCurrentNode() == machineSkill.model.GetWorkspaceNode())
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
        }
        else if (this.model is OfficerModel) {
            OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            officerView.SetParameterOnce("Drop", true);
        }

		AnimatorManager.instance.ChangeAnimatorByID(this.victim.instanceId, AnimatorName.id_Machine_victim, victimAnim, true, false);
		//victimAnim.SetInteger("Type", 1);
		victimAnim.SetBool("Drop", true);
		this.victim.invincible = false;
		this.victim.ClearUnconCommand ();


        this.machineSkill.AttractSkillActivate(this.victim);
        this.puppetAnim.SetBool("Kill", true);
        this.drag = false;
        this.victim = null;
        this.listen = true;
    }

    public void StartDrag(WorkerModel victim) {
        this.victim = victim;
        this.drag = true;
        this.moveDelayTimer = 0;
        
        //격리실로 이동 시작
        if (this.model is AgentModel) {
            AgentUnit agentView = AgentLayer.currentLayer.GetAgent(model.instanceId);

            agentView.SetParameterOnce("Drag", true);
        }
        else if (this.model is OfficerModel) {
            OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            officerView.SetParameterOnce("Drag", true);
        }

        if (victim is AgentModel) {
            this.victimAnim = AgentLayer.currentLayer.GetAgent(victim.instanceId).puppetAnim;
        }
        else if (victim is OfficerModel) {
            this.victimAnim = OfficerLayer.currentLayer.GetOfficer(victim.instanceId).puppetAnim;
        }

        AnimatorManager.instance.ChangeAnimatorByName(victim.instanceId, "Machine_victim", victimAnim, true, false);
    }
}
