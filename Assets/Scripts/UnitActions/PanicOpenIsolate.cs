using UnityEngine;
using System.Collections.Generic;

public class PanicOpenIsolate : PanicAction {

    private AgentModel targetAgent;

    public PanicOpenIsolate(AgentModel target)
    {
        targetAgent = target;
    }

    public void Execute()
    {
		/*
        if (targetAgent.GetState() == AgentCmdState.IDLE)
        {
            if (targetAgent.GetMovableNode().IsMoving() == false)
            {
                if (targetCreature != null)
                {
                    MapNode node = targetCreature.GetEntryNode();

                    if (targetAgent.GetMovableNode().EqualPosition(node))
                    {
                        // start
                        OpenIsolateRoom.Create(targetAgent, targetCreature);
                    }
                }
                else
                {
                    if (targetCreature == null)
                    {
                        CreatureModel[] creatureList = CreatureManager.instance.GetCrea	tureList();
                        if (creatureList.Length > 0)
                        {
                            targetCreature = creatureList[Random.Range(0, creatureList.Length)];
                        }
                    }
                    targetAgent.MoveToNode(targetCreature.GetEntryNode().GetId());
                }
            }
        }
        */

		if (targetAgent.GetMovableNode ().IsMoving () == false)
		{
			if (targetAgent.GetCurrentNode () != null && targetAgent.GetCurrentNode ().connectedCreature != null &&
                targetAgent.GetCurrentNode().connectedCreature.state != CreatureState.ESCAPE)
			{
				if (targetAgent.GetState () != AgentAIState.OPEN_ISOLATE)
				{
					Debug.Log ("OPEN...");
					targetAgent.OpenIsolateRoom (targetAgent.GetCurrentNode ().connectedCreature);
				}
			}
			else
			{
				Debug.Log ("Find creature....");
				MovableObjectNode movable = targetAgent.GetMovableNode ();

				MapNode node = movable.GetCurrentNode ();
				MapEdge edge = movable.GetCurrentEdge ();

				if (node != null)
				{

				}
				else if (edge != null)
				{
				}

				List<string> creatureEntryList = new List<string> ();
				List<float> creatureEntryCost = new List<float> ();
				foreach (CreatureModel creature in CreatureManager.instance.GetCreatureList()) {
                    if (creature.state == CreatureState.ESCAPE)
                        continue;
					creatureEntryList.Add (creature.entryNodeId);
					creatureEntryCost.Add (movable.GetDistance (MapGraph.instance.GetNodeById (creature.entryNodeId), 1000));
				}

				int maxIndex = 0;
				for (int i = 1; i < creatureEntryCost.Count; i++) {
					if (creatureEntryCost [i] < creatureEntryCost [maxIndex])
						maxIndex = i;
				}

                if(creatureEntryCost.Count > 0)
				    targetAgent.MoveToNode (creatureEntryList [maxIndex]);

				//targetAgent.MoveToNode
			}
				
		}
		else
		{
			
		}
    }
}
