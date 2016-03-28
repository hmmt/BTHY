using UnityEngine;
using System.Collections;

public class ReturnCreatureWorkerCommand : WorkerCommand {
	
	// only red shoes

	bool creatureGet = false;
	CreatureModel target;

	public ReturnCreatureWorkerCommand(CreatureModel target)
	{
		this.target = target;
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

		if (target.metaInfo.id == 100003)
		{
			RedShoes shoes = (RedShoes)target.script;
			Vector3 destination = shoes.droppedShoesPosition;

			if (shoes.owner != null && shoes.owner != agent) {
				Finish ();
				return;
			}

			if (!shoes.dropped && shoes.owner != agent) {
				Finish ();
				return;
			}

			MovableObjectNode movable = agent.GetMovableNode ();

			if (!movable.IsMoving ()) {
				if (creatureGet) {
					movable.MoveToNode (target.GetWorkspaceNode());
				} else {
					movable.MoveToMovableNode (shoes.droppedPositionNode);
				}

			}
			this.isMoving = movable.IsMoving ();

			CheckRanage ((AgentModel)agent);
		}
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

		AgentModel actor = (AgentModel)agent;
		if (actor.GetState () == AgentAIState.RETURN_CREATURE)
			actor.FinishReturnCreature ();
	}

	void CheckRanage(AgentModel actor)
	{

		if (target.metaInfo.id == 100003)
		{
			if (creatureGet)
			{
				if (actor.GetCurrentNode () == target.GetWorkspaceNode ())
				{
					RedShoes shoes = (RedShoes)target.script;
					shoes.ReturnFinish ();
					Finish ();
					return;
				}
			}
			else
			{
				RedShoes shoes = (RedShoes)target.script;
				Vector3 destination = shoes.droppedShoesPosition;
				Vector3 dist = actor.GetMovableNode ().GetCurrentViewPosition () - destination;

				if (shoes.owner != null)
					return;

				if (dist.sqrMagnitude <= 2) {
					shoes.ReturnShoesByAgent (actor);
					creatureGet = true;
				}
			}
		}
	}

	void GetCreature()
	{
	}
	
}
