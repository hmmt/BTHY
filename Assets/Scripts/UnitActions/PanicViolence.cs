using UnityEngine;
using System.Collections.Generic;

public class PanicViolence : PanicAction {

	private AgentModel actor;

    public PanicViolence(AgentModel actor)
    {
		this.actor = actor;
    }

	public void Execute()
	{
        /*
        if (targetAgent.GetState() == AgentAIState.IDLE)
        {
            if (targetAgent.GetMovableNode().IsMoving() == false)
            {
                //MapNode roamingPoint = MapGraph.instance.GetRoamingPoint();
                //targetAgent.MoveToNode(roamingPoint.GetId());
            }
            else
            {
                AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(targetAgent.GetMovableNode());

                foreach(AgentModel detected in detectedAgents)
                {
                    if (detected.instanceId == targetAgent.instanceId)
                        continue;

                    // TODO : 
                    targetAgent.GetMovableNode().StopMoving();
                    //AttackAgentByAgent.Create(detected, targetAgent);
                    break;
                }
            }
        }
        */



		if (actor.GetState () == AgentAIState.IDLE)
		{
			AgentModel[] nears = AgentManager.instance.GetNearAgents (actor.GetMovableNode ());

			List<AgentModel> filteredAgents = new List<AgentModel> ();
			foreach (AgentModel nearAgent in nears) {
				if (nearAgent != actor)
					filteredAgents.Add (nearAgent);
			}

			if (filteredAgents.Count > 0) {
				actor.PursueAgent (filteredAgents [0]);
			}
		}


		if (actor.GetMovableNode ().IsMoving () == false && actor.GetState() == AgentAIState.IDLE) {

			actor.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (actor.currentSefira));

		}

	}
}
