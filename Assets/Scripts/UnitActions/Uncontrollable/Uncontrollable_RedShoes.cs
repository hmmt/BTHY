using UnityEngine;
using System.Collections.Generic;

public class Uncontrollable_RedShoes : UncontrollableAction {

	private AgentModel model;

	private float waitTimer = 0;

	private AgentModel target = null;

	public Uncontrollable_RedShoes(AgentModel model)
	{
		this.model = model;
	}

	public override void Execute()
	{
		if ((waitTimer <= 0 && target == null)
			|| model.GetCurrentCommand() == null) {
			model.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (model.currentSefira));
			waitTimer = 1.5f + Random.value;
		}

		waitTimer -= Time.deltaTime;

		if (target == null)
		{
			AgentModel[] nears = AgentManager.instance.GetNearAgents (model.GetMovableNode ());

			List<AgentModel> filteredAgents = new List<AgentModel> ();
			foreach (AgentModel nearAgent in nears)
			{
				if (nearAgent != model)
					filteredAgents.Add (nearAgent);
			}

			if (filteredAgents.Count > 0)
			{
				target = filteredAgents [0];
				model.PursueUnconAgent (target);
			}
		}
	}

	private void PurseAgent()
	{
	}

	private void Attack()
	{
	}

	public override void OnClick()
	{
		SuppressWindow.CreateWindow (model);
	}
}
