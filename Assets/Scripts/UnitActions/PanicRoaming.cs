using UnityEngine;
using System.Collections;

public class PanicRoaming : PanicAction {

    private AgentModel targetAgent;

    public PanicRoaming(AgentModel target)
	{
		targetAgent = target;
	}

	public void Execute()
	{
        if (targetAgent.GetMovableNode().IsMoving() == false)
        {
            //MapNode roamingPoint = MapGraph.instance.GetRoamingPoint();
            //targetAgent.MoveToNode(roamingPoint.GetId());
        }
	}
}
