using UnityEngine;
using System.Collections;

public class PanicViolence : PanicAction {

    private AgentModel targetAgent;

    public PanicViolence(AgentModel target)
    {
        targetAgent = target;
    }

	public void Execute()
	{
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
	}
}
