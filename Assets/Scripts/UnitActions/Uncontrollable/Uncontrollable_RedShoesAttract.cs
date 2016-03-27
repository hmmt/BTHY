using UnityEngine;
using System.Collections;

public class Uncontrollable_RedShoesAttract : UncontrollableAction {

	private AgentModel model;
	private RedShoesSkill redShoesSkill;

	private int remainClicked;

	private float waitTimer = 3.0f;

	private bool wakeUp = false;
	private float wakeUpTimer = 5.0f;

	public Uncontrollable_RedShoesAttract(AgentModel model, RedShoesSkill redShoesSkill)
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
	}

	public override void OnClick()
	{
		if (wakeUp)
			return;
		
		remainClicked--;

		model.movementMul = 0.8f;

		if (remainClicked <= 0)
		{
			//model.GetControl ();
			//redShoesSkill.FreeAttractedAgent (model);
			WakeUp();
		}


	}

	private void WakeUp()
	{
		wakeUp = true;
		AgentUnit agentView = AgentLayer.currentLayer.GetAgent(model.instanceId);
		agentView.puppetAnim.SetBool ("Cancel", true);
		//redShoesSkill.FreeAttractedAgent (model);

		model.movementMul = 0;
	}
}
