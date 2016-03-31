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

		if (targetAgent.isDead ()) {
			OnDieTarget (agent);
			return;
		}

		CheckRanage (agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);
	}

	void OnDieTarget(WorkerModel actor)
	{
		if (actor.unconAction is Uncontrollable_RedShoes) {
			// blabla
			Finish();
			(actor.unconAction as Uncontrollable_RedShoes).OnKill ();
		}

        if (actor.unconAction is Uncontrollable_Machine)
        {
			targetAgent.SetInvincible (true);
			targetAgent.LoseControl ();
            Finish();
            //끌고가는걸 시작
            (actor.unconAction as Uncontrollable_Machine).StartDrag(targetAgent);

        }
	}

	void CheckRanage(WorkerModel actor)
	{

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetAgent.GetMovableNode ().GetCurrentViewPosition ();

		if(actor.GetMovableNode().GetPassage() != null &&
			actor.GetMovableNode().GetPassage() == targetAgent.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 2)
		{
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

			if (actor.attackDelay <= 0)
			{
				//actor.
				targetAgent.TakePhysicalDamage (250, DamageType.NORMAL);

				actor.SetMotionState (AgentMotion.ATTACK_MOTION);

				actor.SetMoveDelay (2.0f);
				actor.SetAttackDelay(4.0f);
				targetAgent.OnHitByWorker (actor);
				targetAgent.SetMoveDelay (1.5f);

				MovableObjectNode movable = actor.GetMovableNode();
				movable.StopMoving ();
			}
		}
		else
		{
			if (actor.moveDelay > 0)
				return;
			MovableObjectNode movable = actor.GetMovableNode();

			if (!movable.IsMoving())
			{
				//Debug.Log ("asdfsdag");
				movable.MoveToMovableNode(targetAgent.GetMovableNode());
			}
		}
	}
}
