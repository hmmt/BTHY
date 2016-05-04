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

		CheckPursueTarget ();

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

		((AgentModel)agent).FinishPursueAgent();
	}

	void CheckRanage(AgentModel actor)
	{

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetAgent.GetMovableNode ().GetCurrentViewPosition ();

		if(actor.GetMovableNode().GetPassage() != null &&
			actor.GetMovableNode().GetPassage() == targetAgent.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 5)
		{
			MovableObjectNode movable = actor.GetMovableNode();
			movable.StopMoving();
			isMoving = false;

			float actorX = actor.GetCurrentViewPosition ().x;
			float targetX = targetAgent.GetCurrentViewPosition ().x;
			if (actorX > targetX)
			{
				actor.GetMovableNode ().SetDirection (UnitDirection.LEFT);
			}
			if (actorX < targetX)
			{
				actor.GetMovableNode ().SetDirection (UnitDirection.RIGHT);
			}

			if (actor.attackDelay <= 0) {

				targetAgent.TakePhysicalDamage (1, DamageType.NORMAL);

				//actor.SetMotionState (AgentMotion.ATTACK_MOTION);

				actor.SetMoveDelay (2.0f);
				actor.SetAttackDelay(3.0f);
				targetAgent.OnHitByWorker (actor);
				targetAgent.SetMoveDelay (3.5f);

				AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
				agentView.SetParameterForSecond ("Attack", true, 0.3f);
			}
		}
		else
		{

		}
	}

	void CheckPursueTarget()
	{
		AgentModel agentActor = (AgentModel)actor;

		AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(actor.GetMovableNode());

		if (detectedAgents.Length > 0) {
			//PursueWorker (detectedAgents [0]);

			AgentModel nearest = null;
			float nearestDist = 100000;
			foreach (AgentModel agent in detectedAgents)
			{
				if (agent == agentActor)
					continue;
				
				Vector3 v = agent.GetCurrentViewPosition () - actor.GetCurrentViewPosition ();

				float m = v.magnitude;

				if (nearestDist > m) {
					nearestDist = m;
					nearest = agent;
				}
			}

			if (nearest != null && nearest != targetAgent)
			{
				Debug.Log ("Change!");
				agentActor.PursueAgent (nearest);
			}
		}
	}
}
