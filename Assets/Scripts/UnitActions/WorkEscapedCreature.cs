using UnityEngine;
using System.Collections;

public class WorkEscapedCreature : MonoBehaviour {
    private enum WorkState
    {
        MOVING,
        WORKING
    };

    WorkState state = WorkState.MOVING;

    public AgentModel agent;
    public CreatureModel creature;

    public int agentStack;
    public int creatureStack;

    private float timer = 0.0f;
    
    private void proccess()
    {
        int agentDice = Random.Range(1, 4+1);
        int creatureDice = Random.Range(1, 5+1);

        creatureStack--;

        if (creatureDice < agentDice)
        {
            /*
            Debug.Log("creature stack down");
            creatureStack--;
            */

        }
        else
        {
            /*
            Debug.Log("agent stack down");
            agentStack--;
            */
        }

        if (creatureStack <= 0)
        {
            Success();
            return;
        }
    }

    void Success()
    {
        agent.FinishWorking();
        creature.ReturnEscape();
        Destroy(gameObject);
    }


    void FixedUpdate()
    {
        if (state == WorkState.MOVING)
        {
            if (creature.GetMovableNode().CheckInRange(agent.GetMovableNode(), 1.5f))
            {
                state = WorkState.WORKING;

                creature.StartEscapeWork();
            }
        }
        else if (state == WorkState.WORKING)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 1)
        {
            timer -= 1;
            proccess();
        }

    }

    public static void Create(AgentModel agent, CreatureModel creature)
    {
        Debug.Log("START escape work");
        GameObject a = new GameObject();

        WorkEscapedCreature c = a.AddComponent<WorkEscapedCreature>();

        c.agent = agent;
        c.creature = creature;

        c.agentStack = c.agent.level;
        c.creatureStack = c.creature.metaInfo.stackLevel;

        agent.WorkEscape(creature);
        //agent.Attacked();
        //creature.state = CreatureState.ESCAPE_WO
    }
}
