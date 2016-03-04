using UnityEngine;
using System.Collections;

public class PanicOpenRoom : PanicAction {

    private AgentModel targetAgent;

    private CreatureModel targetCreature;

    public PanicOpenRoom(AgentModel target)
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
    }
}
