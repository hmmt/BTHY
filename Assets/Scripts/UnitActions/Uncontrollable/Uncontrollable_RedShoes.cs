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
			}
		}
		else
		{
			if ((waitTimer <= 0 && target == null)
				|| ((OfficerModel)model).GetCurrentCommand() == null) {
				model.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (model.currentSefira));
				waitTimer = 1.5f + Random.value;
			}
		}

		waitTimer -= Time.deltaTime;

		if (target == null)
		{
			AgentModel[] nears = AgentManager.instance.GetNearAgents (model.GetMovableNode ());
			OfficerModel[] nearsO = OfficeManager.instance.GetNearOfficers (model.GetMovableNode ());

			List<WorkerModel> filteredAgents = new List<WorkerModel> ();
			foreach (AgentModel nearAgent in nears)
			{
				if (nearAgent != model)
					filteredAgents.Add (nearAgent);
			}

			foreach (OfficerModel nearOfficer in nearsO)
			{
				if (nearOfficer != model)
					filteredAgents.Add (nearOfficer);
			}

			if (filteredAgents.Count > 0)
			{
				target = filteredAgents [Random.Range(0, filteredAgents.Count)];
				if(model is AgentModel)
					((AgentModel)model).PursueUnconAgent (target);
				else
					((OfficerModel)model).PursueUnconAgent (target);
			}
		}
	}

	private void PurseAgent()
	{
	}

	private void Attack()
	{
	}

	public void OnKill()
	{
		if (target != null)
		{
			if (target is AgentModel)
			{
				AgentUnit agentView = AgentLayer.currentLayer.GetAgent (target.instanceId);

				AnimatorManager.instance.ResetAnimatorTransform (target.instanceId);
				AnimatorManager.instance.ChangeAnimatorByName (target.instanceId, AnimatorName.RedShoes_victim,
					agentView.puppetAnim, true, false);
			}
			else
			{
				OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (target.instanceId);

				AnimatorManager.instance.ResetAnimatorTransform (target.instanceId);
				AnimatorManager.instance.ChangeAnimatorByName (target.instanceId, AnimatorName.RedShoes_victim,
					officerView.puppetAnim, true, false);
			}

			model.Stun (4);
			killAnimationTime = 0.5f;

			if (model is AgentModel)
			{
				AgentUnit agentView = AgentLayer.currentLayer.GetAgent (model.instanceId);

				//agentView.animTarget.SetParameterOnce
				agentView.puppetAnim.SetBool("Kill", true);
			}
			else
			{
				OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (model.instanceId);

				officerView.puppetAnim.SetBool ("Kill", true);
			}
		}
		target = null;
	}

	public override void OnDie()
	{
		redShoesSkill.OnInfectedTargetTerminated ();

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
