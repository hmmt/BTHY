using UnityEngine;
using System.Collections;

public class OpenIsolateWorkerCommand : WorkerCommand {

	private float openIsolateTime;

	public float elapsedTime = 0;

	public OpenIsolateWorkerCommand(CreatureModel targetCreature)
	{
		this.targetCreature = targetCreature;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);

		openIsolateTime = 5;
		//actor.SetMotionState (AgentMotion.PANIC_ATTACK_MOTION);

		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
		agentView.SetParameterForSecond ("Attack", true, 0.3f);
	}

	public override void OnStart(WorkerModel agent)
	{
		base.OnStart (agent);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);

		elapsedTime += Time.deltaTime;

		if (elapsedTime > openIsolateTime) {
			//targetCreature.SubEnergyPoint (10);
			targetCreature.SubFeeling(10);
			Finish ();
		}
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

        ((AgentModel)agent).FinishOpenIolateRoom();
	}
}
