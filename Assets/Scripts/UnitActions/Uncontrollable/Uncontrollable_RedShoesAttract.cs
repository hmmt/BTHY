using UnityEngine;
using System.Collections;

public class Uncontrollable_RedShoesAttract : UncontrollableAction {

	private WorkerModel model;
	private RedShoesSkill redShoesSkill;

	private int remainClicked;

	private float waitTimer = 3.0f;

	private bool wakeUp = false;
	private float wakeUpTimer = 2.0f;

	private float slowTimer = 0;

	public Uncontrollable_RedShoesAttract(WorkerModel model, RedShoesSkill redShoesSkill)
	{
		this.model = model;
		this.redShoesSkill = redShoesSkill;

		remainClicked = 5;
	}

	public override void Execute()
	{
		if (waitTimer <= 0 && 
			!model.GetMovableNode ().IsMoving ())
		{
			MapNode destination = redShoesSkill.model.GetWorkspaceNode ();
			model.MoveToNode (destination);
		}

		if (waitTimer > 0)
			waitTimer -= Time.deltaTime;

		if (wakeUp)
		{
			wakeUpTimer -= Time.deltaTime;
			if (wakeUpTimer <= 0)
			{
				model.movementMul = 1;
				redShoesSkill.FreeAttractedAgent (model);
			}
		}
		else
		{
			if (slowTimer > 0)
				slowTimer -= Time.deltaTime;
			else
				model.movementMul = 1;
		}
	}

	public override void OnClick()
	{
		if (wakeUp)
			return;
		
		remainClicked--;

		model.movementMul = 0.2f;
		slowTimer = 1;

		if (remainClicked <= 0)
		{
			WakeUp();
		}
		else
		{
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (model.instanceId);
			agentView.CharRecoilInput (1);
		}

	}

	public override void OnDie()
	{
		//redShoesSkill.OnInfectedTargetTerminated ();
	}

	private void WakeUp()
	{
		wakeUp = true;

		if (model is AgentModel) {
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (model.instanceId);
			agentView.puppetAnim.SetBool ("Cancel", true);
		} else {

			OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (model.instanceId);
			officerView.puppetAnim.SetBool ("Cancel", true);
		}
		//redShoesSkill.FreeAttractedAgent (model);

		model.movementMul = 0;
	}
}
