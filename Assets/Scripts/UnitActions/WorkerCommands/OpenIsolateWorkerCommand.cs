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
			if (targetCreature.IsEscapable ())
				targetCreature.Escape ();
			else
				targetCreature.SubFeeling (50);
			Finish ();
		}
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

        ((AgentModel)agent).FinishOpenIolateRoom();
	}
}
