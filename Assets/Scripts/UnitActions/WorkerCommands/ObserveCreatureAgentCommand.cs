using UnityEngine;
using System.Collections;

public class ObserveCreatureAgentCommand : WorkerCommand
{
	private ObserveAction action;
	
	public ObserveCreatureAgentCommand(CreatureModel targetCreature)
	{
		this.targetCreature = targetCreature;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);
		targetCreature.AddTargetedCount ();
	}

	public override void OnStart(WorkerModel agent)
	{
		//ObserveCreature.Create ((AgentModel)agent, targetCreature);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);

		if (targetCreature.state == CreatureState.ESCAPE || targetCreature.state == CreatureState.ESCAPE_PURSUE) {
			Finish ();
			return;
		}
		if(action == null)
			CheckStarting ((AgentModel)agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		if (action != null && action.IsFinished () == false)
		{
			action.FinishForcely ();
		}
		targetCreature.ReleaseTargetedCount ();
	}

	private void CheckStarting(AgentModel agent)
	{
		

		action = ObserveAction.InitUseSkillAction (agent, targetCreature);

		if (action == null)
			Finish ();
	}

}