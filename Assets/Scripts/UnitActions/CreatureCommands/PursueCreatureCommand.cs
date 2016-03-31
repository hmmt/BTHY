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

		if (actor.state != CreatureState.ESCAPE && actor.state != CreatureState.ESCAPE_PURSUE)
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
	public override void OnDestroy(CreatureModel creature)
	{
		base.OnStop(creature);

		MovableObjectNode movable = creature.GetMovableNode();
		movable.StopMoving();

		if (actor.state == CreatureState.ESCAPE_PURSUE)
			actor.state = CreatureState.ESCAPE;
	}

	void CheckRange()
	{
		//actor.GetMovableNode ().GetPassage ();

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetWorker.GetMovableNode ().GetCurrentViewPosition ();

		//dist.sqrMagnitude
		if(actor.GetMovableNode().GetPassage() != null &&
			actor.GetMovableNode().GetPassage() == targetWorker.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 2)


			//return;
			//if (targetWorker.GetMovableNode().GetDistance(actor.GetMovableNode(), 500) < 1)
		{
			//detectedAgents [0].TakePhysicalDamage (1);
			//detectedAgents [0].TakePhysicalDamage (1);
			//actor.GetMovableNode().StopMoving();
			//Debug.Log ("Attack?");
			MovableObjectNode movable = actor.GetMovableNode();
			movable.StopMoving();

			Vector3 directionAdder;
			if (actor.GetDirection () == UnitDirection.RIGHT)
				directionAdder = new Vector3 (2, 0, 0);
			else
				directionAdder = new Vector3 (-2, 0, 0);
				
			if (actor.attackDelay <= 0) {
				actor.SendAnimMessage ("Attack");
				HitObjectManager.AddHitbox (actor.GetCurrentViewPosition ()+directionAdder, 0.5f, 4.0f, 3);
				actor.ResetAttackDelay ();

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