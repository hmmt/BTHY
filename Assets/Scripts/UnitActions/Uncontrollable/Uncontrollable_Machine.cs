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

    public Uncontrollable_Machine(WorkerModel model, SingingMachineSkill machineSkill){
        this.model = model;
        this.machineSkill = machineSkill;
        
    }

    public override void Init()
    {
        if (model is AgentModel)
        {
            AgentUnit agentView = AgentLayer.currentLayer.GetAgent(this.model.instanceId);
            //agentView.puppetAnim.SetInteger("Type", startType);
        }
        else if (model is OfficerModel) {
            OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            //officerView.puppetAnim.SetInteger("Type", startType);
        }
    }

    public override void Execute()
    {
        if (startWaitTimer > 0) {
            startWaitTimer -= Time.deltaTime;
            return;
        }

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
}
