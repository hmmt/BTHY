using UnityEngine;
using System.Collections;

public class UnconPursueWorkerCommand : WorkerCommand {

	public float elapsedTime = 0;

	public UnconPursueWorkerCommand(WorkerModel targetAgent)
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

		CheckRanage (agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

		((AgentModel)agent).FinishOpenIolateRoom();
	}

	void CheckRanage(WorkerModel actor)
	{

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetAgent.GetMovableNode ().GetCurrentViewPosition ();

		if(actor.GetMovableNode().GetPassage() == targetAgent.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 2)
		{
			if (actor.attackDelay <= 0)
			{
				//actor.
				targetAgent.TakePhysicalDamage (5);

				actor.SetMotionState (AgentMotion.ATTACK_MOTION);

				actor.SetAttackDelay(4.0f);

				MovableObjectNode movable = actor.GetMovableNode();
				movable.StopMoving ();
				isMoving = false;
			}
		}
		else
		{
			MovableObjectNode movable = actor.GetMovableNode();

			if (!movable.IsMoving())
			{
				//Debug.Log ("asdfsdag");
				movable.MoveToMovableNode(targetAgent.GetMovableNode());
			}
			this.isMoving = movable.IsMoving ();
		}
	}
}
