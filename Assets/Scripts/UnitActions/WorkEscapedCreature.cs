using UnityEngine;
using System.Collections;

public class WorkEscapedCreature : ActionClassBase {
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
    private float waitTimer = 0;
    
    private void proccess()
    {
        Success();
        return;
        int agentDice = Random.Range(1, 4+1);
        int creatureDice = Random.Range(1, 5+1 + 10);

        creatureStack--;

        if (creatureDice < agentDice)
        {
			creatureStack--;
        }
        else
        {
			agentStack--;
        }

        if (creatureStack <= 0)
        {
            Success();
            return;
        }

        if (agentStack <= 0)
        {
            Fail();
            return;
        }
    }

    void Success()
    {
        agent.FinishWorking();
        //creature.ReturnEscape();

        ReturnCreatureAction.Create(agent, creature);
        Destroy(gameObject);
    }

	void Fail()
	{
		agent.FinishWorking();
        creature.StopEscapeWork();
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
            else if (waitTimer <= 0)
            {
                agent.MoveToCreature(creature);
                waitTimer = 1f + Random.value;
            }
            waitTimer -= Time.deltaTime;
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
