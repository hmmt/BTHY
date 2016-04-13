using UnityEngine;
using System.Collections;

public class ReturnCreatureAction : ActionClassBase {

    private AgentModel agent;
    private CreatureModel creature;

    private MovableObjectNode creaturePosTarget;
    private MovableObjectNode[] agentMoveQueue;
    private int queueIndex = 0;
    private int queueSize = 0;

    public void FixedUpdate()
    {
        if (agent.GetCurrentNode() == creature.GetWorkspaceNode())
        {
            //state = CreatureState.WAIT;
            Finish();
        }
        else
        {
            /*
            if (creature.GetMovableNode().IsMoving() == false)
            {
                creature.GetMovableNode().MoveToNode(creature.GetWorkspaceNode());
            }
            */
            if (agent.GetMovableNode().IsMoving() == false)
            {
                agent.GetMovableNode().MoveToNode(creature.GetWorkspaceNode());
            }

            if (queueSize < 15)
            {
                int index = ++queueIndex % 15;
                agentMoveQueue[index].Assign(agent.GetMovableNode());
                queueSize++;
                queueIndex = queueIndex % 15;
            }
            else //if (agentMoveQueue[queueIndex].Equal(agent.GetMovableNode()) == false)
            {
                queueIndex = ++queueIndex % 15;
                creaturePosTarget.Assign(agentMoveQueue[queueIndex]);
                agentMoveQueue[queueIndex].Assign(agent.GetMovableNode());
                queueSize++;
            }
            creature.GetMovableNode().Assign(creaturePosTarget);
        }
    }

    public void Finish()
    {
        creature.state = CreatureState.WAIT;
        creature.GetMovableNode().MoveToNode(creature.GetWorkspaceNode());
        agent.UpdateStateIdle();

        Destroy(gameObject);
    }


    public static void Create(AgentModel agent, CreatureModel creature)
    {
        GameObject a = new GameObject();

        ReturnCreatureAction c = a.AddComponent<ReturnCreatureAction>();

        c.agent = agent;
        c.creature = creature;
        c.agentMoveQueue = new MovableObjectNode[50];
        // target은 movable이 아닌 듯
        c.creaturePosTarget = new MovableObjectNode(null);
        c.creaturePosTarget.Assign(agent.GetMovableNode());
        for(int i=0; i<50; i++)
        {
            c.agentMoveQueue[i] = new MovableObjectNode(null);
        }
        c.agentMoveQueue[c.queueIndex].Assign(agent.GetMovableNode());
        c.queueSize++;

        //creature.state = CreatureState.ESCAPE_RETURN;

        c.agent.ReturnCreature(creature);
        //c.creature.ReturnEscape();

        //agent.WorkEscape(creature);
        //agent.Attacked();
        //creature.state = CreatureState.ESCAPE_WO
    }
}
