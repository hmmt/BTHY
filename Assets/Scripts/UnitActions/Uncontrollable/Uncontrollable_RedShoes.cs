using UnityEngine;
using System.Collections.Generic;

public class Uncontrollable_RedShoes : UncontrollableAction {

	private WorkerModel model;
	private RedShoesSkill redShoesSkill;

	private float waitTimer = 0;

	private WorkerModel target = null;

	private float startWaitTimer = 10f;

	private int startType;

	private float killAnimationTime = 0;

    private bool moving = false;
    private bool killing = false;
    private bool dying = false;

    public CreatureBase.CreatureTimer timer = new CreatureBase.CreatureTimer();

	public Uncontrollable_RedShoes(WorkerModel model, RedShoesSkill redShoesSkill, int startType)
	{
		this.model = model;
		this.redShoesSkill = redShoesSkill;
		this.startType = startType;
	}

	public override void Init()
	{
		if (model is AgentModel)
		{
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (model.instanceId);
			agentView.puppetAnim.SetInteger ("Type", startType);
		}
		else
		{
			OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (model.instanceId);
			officerView.puppetAnim.SetInteger ("Type", startType);
		}
	}

	public override void Execute()
	{
        
        if (killing)
        {
            if (!model.GetMovableNode().IsMoving())
            {
                Killing();
            }
            return;
        }

        if (dying) {
            OnDie();
        }

		Notice.instance.Send (NoticeName.EscapeCreature);
		if (startWaitTimer > 0) {
			startWaitTimer -= Time.deltaTime;
			return;
		}
		if (killAnimationTime > 0)
		{
			killAnimationTime -= Time.deltaTime;
			if (killAnimationTime <= 0)
			{
				if (model is AgentModel)
				{
					AgentUnit agentView = AgentLayer.currentLayer.GetAgent (model.instanceId);
					agentView.puppetAnim.SetBool("Kill", false);
				}
				else
				{
					OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (model.instanceId);
					officerView.puppetAnim.SetBool ("Kill", false);
				}
			}
			return;
		}

		if(model is AgentModel)
		{	
			if ((waitTimer <= 0 && target == null)
				|| ((AgentModel)model).GetCurrentCommand() == null) {
				model.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (model.currentSefira));
				waitTimer = 1.5f + Random.value;

                AgentUnit agentView = AgentLayer.currentLayer.GetAgent(model.instanceId);
                agentView.showSpeech.showSpeech(AgentLyrics.instance.GetCreatureReaction(this.redShoesSkill.model.metadataId).action.GetActionDesc("walk").GetDescByRandom());
			}
		}
		else
		{
			if ((waitTimer <= 0 && target == null)
				|| ((OfficerModel)model).GetCurrentCommand() == null) {
				model.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (model.currentSefira));
				waitTimer = 1.5f + Random.value;

                OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
                officerView.showSpeech.showSpeech(AgentLyrics.instance.GetCreatureReaction(this.redShoesSkill.model.metadataId).action.GetActionDesc("walk").GetDescByRandom());
			}
		}

		waitTimer -= Time.deltaTime;


		if (target == null)
		{
			AgentModel[] nears = AgentManager.instance.GetNearAgents (model.GetMovableNode ());
			OfficerModel[] nearsO = OfficerManager.instance.GetNearOfficers (model.GetMovableNode ());

			List<WorkerModel> filteredAgents = new List<WorkerModel> ();
			foreach (AgentModel nearAgent in nears)
			{
				if (nearAgent != model)
					filteredAgents.Add (nearAgent);
			}

			foreach (OfficerModel nearOfficer in nearsO)
			{
                if (nearOfficer != model)
                {
                    if (nearOfficer.state == OfficerAIState.SPECIALACTION) continue;
                    filteredAgents.Add(nearOfficer);
                }
			}

			if (filteredAgents.Count > 0)
			{
				target = filteredAgents [Random.Range(0, filteredAgents.Count)];
				if(model is AgentModel)
					((AgentModel)model).PursueUnconAgent (target);
				else
					((OfficerModel)model).PursueUnconAgent (target);

                this.model.ShowCreatureActionSpeech(this.redShoesSkill.model.metadataId, "attack");
			}
		}

	}

	private void PurseAgent()
	{
	}

	private void Attack()
	{
	}

    public void StartSettingPos() {
        if(target == null) return;
        killing = true;
        /*
        MapNode current = target.GetMovableNode().GetCurrentNode();
        if (current == null)
        {
            MapEdge currentEdge = target.GetMovableNode().GetCurrentEdge();
            MapNode left, right;

            if (currentEdge.node1.GetPosition().x < currentEdge.node2.GetPosition().x)
            {
                left = currentEdge.node1;
                right = currentEdge.node2;
            }
            else
            {
                left = currentEdge.node2;
                right = currentEdge.node1;
            }

            if (target.GetMovableNode().GetDirection() == UnitDirection.RIGHT)
            {
                current = right;
            }
            else
            {
                current = left;
            }
        }
        Debug.Log(current.GetId());*/
        this.model.GetMovableNode().MoveToMovableNode(target.GetMovableNode());
        
    }

    void Killing() {
        if (target != null)
        {
            Transform targetTransform;
            Transform modelTransform;
            if (target is AgentModel)
            {
                AgentUnit agentView = AgentLayer.currentLayer.GetAgent(target.instanceId);

                AnimatorManager.instance.ResetAnimatorTransform(target.instanceId);
                AnimatorManager.instance.ChangeAnimatorByName(target.instanceId, AnimatorName.RedShoes_victim,
                    agentView.puppetAnim, true, false);
                targetTransform = agentView.puppetNode.transform;
            }
            else
            {
                OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(target.instanceId);

                AnimatorManager.instance.ResetAnimatorTransform(target.instanceId);
                AnimatorManager.instance.ChangeAnimatorByName(target.instanceId, AnimatorName.RedShoes_victim,
                    officerView.puppetAnim, true, false);

                //officerView.blockDir = true;
                //target.HaltUpdate();

                targetTransform = officerView.puppetNode.transform;

            }

            model.Stun(4);
            killAnimationTime = 0.5f;

            model.ShowCreatureActionSpeech(this.redShoesSkill.model.metadataId, "kill");

            if (model is AgentModel)
            {
                AgentUnit agentView = AgentLayer.currentLayer.GetAgent(model.instanceId);
                agentView.puppetAnim.SetBool("Kill", true);
                modelTransform = agentView.puppetNode.transform;
            }
            else
            {
                OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
                officerView.puppetAnim.SetBool("Kill", true);
                modelTransform = officerView.puppetNode.transform;
            }
            /*
            if (targetTransform.position.x < modelTransform.position.x)
            {
                Vector3 targetscale = targetTransform.localScale;
                if (targetTransform.localScale.x > 0)
                {
                    targetscale.x = -targetscale.x;
                    Debug.Log("뒤집는다");
                }

                targetTransform.localScale = targetscale;
            }
            else {
                Vector3 targetscale = targetTransform.localScale;
                if (targetTransform.localScale.x < 0) {
                    targetscale.x = -targetscale.x;
                    Debug.Log("뒤집는다");
                }

                targetTransform.localScale = targetscale;
            }
            */
        }
        target = null;
        killing = false;
    }

	public void OnKill()
	{
        StartSettingPos();
	}

	public override void OnDie()
	{
        dying = true;
        if (this.killing) {
            return;
        }
        dying = false;
		redShoesSkill.OnInfectedTargetTerminated ();

        model.ShowCreatureActionSpeech(this.redShoesSkill.model.metadataId, "dead");

		if (model is AgentModel)
		{
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (model.instanceId);

			agentView.SetParameterOnce ("Suppressed", true);
		}
		else
		{
			OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (model.instanceId);

			officerView.SetParameterOnce ("Suppressed", true);
		}
	}

	public override void OnClick()
	{
		if (model is AgentModel)
		{
			SuppressWindow.CreateWindow ((AgentModel)model);
		}
		else if (model is OfficerModel)
		{
			SuppressWindow.CreateWindow ((OfficerModel)model);
		}

	}
}
