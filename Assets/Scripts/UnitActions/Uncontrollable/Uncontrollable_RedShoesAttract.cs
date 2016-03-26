using UnityEngine;
using System.Collections;

public class Uncontrollable_RedShoesAttract : UncontrollableAction {

	private AgentModel model;
	private RedShoesSkill redShoesSkill;

	private int remainClicked;

	private float waitTimer = 3.0f;

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

	}

	public override void OnClick()
	{
		remainClicked--;

		if (remainClicked <= 0)
		{
			model.GetControl ();
		}
	}
}
