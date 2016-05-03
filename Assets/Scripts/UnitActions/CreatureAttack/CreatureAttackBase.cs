using UnityEngine;
using System.Collections;

public class CreatureAttackBase : MonoBehaviour {
    public AgentModel agent;
    public CreatureModel creature;

    private float timer = 0.0f;

    private void proccess()
    {
        int agentDice = Random.Range(1, 4);
        int creatureDice = Random.Range(1, 5);

        if (creatureDice > agentDice)
        {
            if (agent.hp > 0)
            {
                agent.TakePhysicalDamageByCreature(creature.metaInfo.physicsDmg);
                agent.TakeMentalDamage(creature.metaInfo.mentalDmg);
            }

            if(agent.isDead())
            {
                Finish();
            }
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

    public static CreatureAttackBase Create(AgentModel agent, CreatureModel creature)
    {
        GameObject a = new GameObject();

        CreatureAttackBase c = a.AddComponent<CreatureAttackBase>();

        c.agent = agent;
        c.creature = creature;

        agent.AttackedByCreature();
        creature.state = CreatureState.ESCAPE_ATTACK;

        return c;
    }
}
