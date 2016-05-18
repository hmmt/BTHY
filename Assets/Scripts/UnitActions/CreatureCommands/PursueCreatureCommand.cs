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
		{
			Finish ();
			return;
		}

		CheckPursueTarget ();

		if (CheckRange ())
			return;
		
		MovableObjectNode movable = creature.GetMovableNode();

		if (!movable.IsMoving())
		{
			movable.MoveToMovableNode(targetWorker.GetMovableNode());
		}
		this.isMoving = movable.IsMoving ();
	}
	public override void OnDestroy(CreatureModel creature)
	{
		base.OnStop(creature);

		MovableObjectNode movable = creature.GetMovableNode();
		movable.StopMoving();

		if (actor.state == CreatureState.ESCAPE_PURSUE)
			actor.state = CreatureState.ESCAPE;
	}

	void CheckPursueTarget()
	{
		AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(actor.GetMovableNode());

		if (detectedAgents.Length > 0) {
			//PursueWorker (detectedAgents [0]);

			AgentModel nearest = null;
			float nearestDist = 100000;
			foreach (AgentModel agent in detectedAgents)
			{
				Vector3 v = agent.GetCurrentViewPosition () - actor.GetCurrentViewPosition ();

				float m = v.magnitude;

				if (nearestDist > m) {
					nearestDist = m;
					nearest = agent;
				}
			}

			if (nearest != null && nearest != targetWorker)
			{
				Debug.Log ("Change!");
				actor.PursueWorker (nearest);
			}
		}
	}

	bool CheckRange()
	{
		//actor.GetMovableNode ().GetPassage ();

		Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - targetWorker.GetMovableNode ().GetCurrentViewPosition ();

		//dist.sqrMagnitude
		if(actor.GetMovableNode().GetPassage() != null &&
			actor.GetMovableNode().GetPassage() == targetWorker.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 5)
		{
			MovableObjectNode movable = actor.GetMovableNode();
			movable.StopMoving();
			isMoving = false;

			Vector3 directionAdder;
			if (actor.GetDirection () == UnitDirection.RIGHT)
				directionAdder = new Vector3 (2, 0, 0);
			else
				directionAdder = new Vector3 (-2, 0, 0);
				
			if (actor.attackDelay <= 0) {
				actor.SendAnimMessage ("Attack");
				//HitObjectManager.AddHitbox (actor.GetCurrentViewPosition ()+directionAdder, 0.5f, 4.0f, 3);

                targetWorker.RecentlyAttackedCreature(actor);
                targetWorker.TakePhysicalDamageByCreature(1);
				actor.ResetAttackDelay ();
			}

			return true;
		}
		else
		{

		}
		/*
		foreach(AgentModel model in detectedAgents)
		{
			
		}
*/
		return false;
	}
}