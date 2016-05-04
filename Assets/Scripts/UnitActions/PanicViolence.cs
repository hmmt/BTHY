using UnityEngine;
using System.Collections.Generic;

public class PanicViolence : PanicAction {

	private AgentModel actor;

    public PanicViolence(AgentModel actor)
    {
		this.actor = actor;
    }


	public void Init()
	{
		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
		agentView.puppetAnim.SetBool ("Panic", true);
		agentView.puppetAnim.SetInteger ("PanicType", 1);
	}

	public void Execute()
	{

		if (actor.GetState () == AgentAIState.IDLE)
		{
			AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(actor.GetMovableNode ());

			if (detectedAgents.Length > 0) {
				//PursueWorker (detectedAgents [0]);

				AgentModel nearest = null;
				float nearestDist = 100000;
				foreach (AgentModel agent in detectedAgents)
				{
					if (agent.GetMovableNode ().GetPassage () == null)
						continue;

					if (agent == actor)
						continue;

					Vector3 v = agent.GetCurrentViewPosition () - actor.GetCurrentViewPosition ();

					float m = v.magnitude;

					if (nearestDist > m) {
						nearestDist = m;
						nearest = agent;
					}
				}

				if(nearest != null)
					actor.PursueAgent (nearest);
			}
		}


		if (actor.GetMovableNode ().IsMoving () == false && actor.GetState() == AgentAIState.IDLE) {

			actor.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (actor.currentSefira));

		}

	}
}
