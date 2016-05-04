using UnityEngine;
using System.Collections.Generic;

public class PanicOpenIsolate : PanicAction {

	private AgentModel actor;

    public PanicOpenIsolate(AgentModel target)
    {
        actor = target;
    }

	public void Init()
	{
		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
		agentView.puppetAnim.SetBool ("Panic", true);
		agentView.puppetAnim.SetInteger ("PanicType", 2);
	}

    public void Execute()
    {

		if (actor.GetMovableNode ().IsMoving () == false)
		{
			if (actor.GetCurrentNode () != null && actor.GetCurrentNode ().connectedCreature != null &&
                actor.GetCurrentNode().connectedCreature.state != CreatureState.ESCAPE)
			{
				if (actor.GetState () != AgentAIState.OPEN_ISOLATE)
				{
					actor.OpenIsolateRoom (actor.GetCurrentNode ().connectedCreature);

					// set motion
				}
			}
			else
			{
				Debug.Log ("Find creature....");
				MovableObjectNode movable = actor.GetMovableNode ();


				List<string> creatureEntryList = new List<string> ();
				List<float> creatureEntryCost = new List<float> ();
				foreach (CreatureModel creature in CreatureManager.instance.GetCreatureList()) {
                    if (creature.state == CreatureState.ESCAPE)
                        continue;
					//if(creature.energyPoint < 
					creatureEntryList.Add (creature.entryNodeId);
					creatureEntryCost.Add (movable.GetDistance (MapGraph.instance.GetNodeById (creature.entryNodeId), 1000));
				}

				int maxIndex = 0;
				for (int i = 1; i < creatureEntryCost.Count; i++) {
					if (creatureEntryCost [i] < creatureEntryCost [maxIndex])
						maxIndex = i;
				}

                if(creatureEntryCost.Count > 0)
				    actor.MoveToNode (creatureEntryList [maxIndex]);

				//targetAgent.MoveToNode
			}
				
		}
		else
		{
			
		}
    }
}
