using UnityEngine;
using System.Collections;


public class PursueCreatureCommand : CreatureCommand
{
	private WorkerModel targetWorker;
	private float elapsedTime;

	public PursueCreatureCommand(WorkerModel target)
	{
		this.targetWorker = target;
	}

	public override void OnStart(CreatureModel creature)
	{
		base.OnStart(creature);
	}
	public override void Execute(CreatureModel creature)
	{
		base.Execute(creature);

		if (actor.state != CreatureState.ESCAPE)
			Finish ();

		MovableObjectNode movable = creature.GetMovableNode();

		if (!movable.IsMoving())
		{
			//Debug.Log ("asdfsdag");
			movable.MoveToMovableNode(targetWorker.GetMovableNode());
		}
		this.isMoving = movable.IsMoving ();

		CheckRange ();
	}
	public override void OnStop(CreatureModel creature)
	{
		base.OnStop(creature);

		MovableObjectNode movable = creature.GetMovableNode();
		movable.StopMoving();
	}

	void CheckRange()
	{
		//actor.GetMovableNode ().GetPassage ();

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetWorker.GetMovableNode ().GetCurrentViewPosition ();

		//dist.sqrMagnitude
		if(actor.GetMovableNode().GetPassage() == targetWorker.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 2)


			//return;
			//if (targetWorker.GetMovableNode().GetDistance(actor.GetMovableNode(), 500) < 1)
		{
			//detectedAgents [0].TakePhysicalDamage (1);
			//detectedAgents [0].TakePhysicalDamage (1);
			//actor.GetMovableNode().StopMoving();
			//Debug.Log ("Attack?");

			if (actor.attackDelay <= 0) {
				actor.AttackAction ();
				isMoving = false;

				// StopMoving
			}
		}
		else
		{

		}
		/*
		foreach(AgentModel model in detectedAgents)
		{
			
		}
*/
	}
}