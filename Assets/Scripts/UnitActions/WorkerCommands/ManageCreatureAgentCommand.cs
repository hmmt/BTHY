using UnityEngine;
using System.Collections;


public class ManageCreatureAgentCommand : WorkerCommand
{
	private AgentModel[] coopAgents;
	private SkillTypeInfo skill;

	private UseSkill useSkill;

	private bool waiting = true;

	public ManageCreatureAgentCommand(CreatureModel targetCreature, AgentModel self, SkillTypeInfo skill)
	{
		this.targetCreature = targetCreature;
		this.skill = skill;
		this.coopAgents = new AgentModel[]{ self };
	}

	public ManageCreatureAgentCommand(CreatureModel targetCreature, AgentModel[] coopAgents, SkillTypeInfo skill)
	{
		this.targetCreature = targetCreature;
		this.skill = skill;
		this.coopAgents = coopAgents;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);
		targetCreature.AddTargetedCount ();
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);
		if (targetCreature.state == CreatureState.ESCAPE || targetCreature.state == CreatureState.ESCAPE_PURSUE) {
			Finish ();
			return;
		}
		if(useSkill == null)
			CheckStarting ((AgentModel)agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		if (useSkill != null && useSkill.IsFinished () == false)
		{
			useSkill.FinishForcely ();
		}
		targetCreature.ReleaseTargetedCount ();
	}

	public void Cancle()
	{
	}

	private void CheckStarting(AgentModel agent)
	{
		if (!waiting)
			return;
		int count = 0;
		/*
		foreach (AgentModel otherAgent in coopAgents)
		{
			if (otherAgent.GetCurrentCommandType() != AgentCmdType.MANAGE_CREATURE)
			{
				count++;
			}
		}*/

		if (count == 0)
		{
			/*
			foreach (AgentModel otherAgent in coopAgents)
			{
				AgentCommand otherCmd = otherAgent.GetCurrentCommand ();
				ManageCreatureAgentCommand cmd = (ManageCreatureAgentCommand)otherCmd;

				cmd.waiting = false;
			}*/


			useSkill = UseSkill.InitUseSkillAction (skill, agent, targetCreature);

			if (useSkill == null)
				Finish ();
			waiting = false;

			//Finish ();
		}
	}
}