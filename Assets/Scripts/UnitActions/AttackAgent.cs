using UnityEngine;
using System.Collections;

public class AttackAgent : MonoBehaviour {
    public AgentModel agent;
    public CreatureModel creature;

    private float timer = 0.0f;

    private void proccess()
    {
        int agentDice = Random.Range(1, 4);
        int creatureDice = Random.Range(1, 5);

        if (creatureDice > agentDice)
        {
            agent.hp -= creature.metaInfo.physicsDmg;
            agent.mental -= creature.metaInfo.mentalDmg;
            Debug.Log("attack");
        }
        else
        {
            Debug.Log("Finish");
            Finish();
        }
    }

    void Finish()
    {
        creature.StopEscapeAttack();
        agent.FinishWorking();

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= 1)
        {
            timer -= 1;
            proccess();
        }
    }

    public static void Create(AgentModel agent, CreatureModel creature)
    {

        GameObject a = new GameObject();

        AttackAgent c = a.AddComponent<AttackAgent>();

        c.agent = agent;
        c.creature = creature;

        agent.Attacked();
        creature.state = CreatureState.ESCAPE_ATTACK;
    }
}
