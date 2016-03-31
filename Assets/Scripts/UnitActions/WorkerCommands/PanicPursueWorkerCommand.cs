using UnityEngine;
using System.Collections;

public class PanicPursueWorkerCommand : WorkerCommand {

	public float elapsedTime = 0;

	public PanicPursueWorkerCommand(AgentModel targetAgent)
	{
		//this.targetCreature = targetCreature;
		this.targetAgent = targetAgent;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);
	}

	public override void OnStart(WorkerModel agent)
	{
		base.OnStart (agent);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);

		MovableObjectNode movable = agent.GetMovableNode();

		if (!movable.IsMoving())
		{
			//Debug.Log ("asdfsdag");
			movable.MoveToMovableNode(targetAgent.GetMovableNode());
		}

		CheckRanage ((AgentModel) agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

		((AgentModel)agent).FinishOpenIolateRoom();
	}

	void CheckRanage(AgentModel actor)
	{

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetAgent.GetMovableNode ().GetCurrentViewPosition ();

		if(actor.GetMovableNode().GetPassage() != null &&
			actor.GetMovableNode().GetPassage() == targetAgent.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 2)
		{
			if (actor.attackDelay <= 0) {
				/*
				HitObjectManager.AddPanicHitbox (actor.GetCurrentViewPosition (), actor, (AgentModel)targetAgent);
				//ResetAttackDelay ();
				actor.SetAttackDelay(4.0f);
				isMoving = false;
				*/
				// StopMoving

				// 

				targetAgent.TakePhysicalDamage (1, DamageType.NORMAL);

				actor.SetMotionState (AgentMotion.ATTACK_MOTION);

				actor.SetMoveDelay (2.0f);
				actor.SetAttackDelay(3.0f);
				targetAgent.OnHitByWorker (actor);
				targetAgent.SetMoveDelay (3.5f);
			}
		}
		else
		{

		}
	}
}
