﻿using UnityEngine;
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
		this.isMoving = movable.IsMoving ();

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

		if(actor.GetMovableNode().GetPassage() == targetAgent.GetMovableNode().GetPassage() &&
			dist.sqrMagnitude <= 2)
		{
			if (actor.attackDelay <= 0) {
				actor.AttackAction ((AgentModel)targetAgent);
				isMoving = false;

				// StopMoving
			}
		}
		else
		{

		}
	}
}
