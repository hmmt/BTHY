using UnityEngine;
using System.Collections;

public class FollowWorkerCommand : WorkerCommand {
	
	public float elapsedTime = 0;
	public MovableObjectNode targetMovable;

	public FollowWorkerCommand(MovableObjectNode target)
	{
		this.targetMovable = target;
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
		/*
		if (targetAgent.isDead ()) {
			OnDieTarget (agent);
			return;
		}*/

		MovableObjectNode movable = agent.GetMovableNode();

		if((movable.GetCurrentViewPosition() - targetMovable.GetCurrentViewPosition()).sqrMagnitude < 1)
		{
			movable.StopMoving ();
		}
		else if (!movable.IsMoving())
		{
			//Debug.Log ("asdfsdag");
			movable.MoveToMovableNode(targetMovable);

			/*
			if ((movable.GetCurrentViewPosition() - targetMovable.GetCurrentViewPosition()).sqrMagnitude < 1)
			{
				Finish(); 
			}
			*/
		}

		if (targetMovable.GetPassage () != agent.GetMovableNode ().GetPassage ())
		{
			agent.GetMovableNode ().Assign (targetMovable);
		}
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);
	}

	void OnDieTarget(WorkerModel actor)
	{
		//Finish ();
	}

}
