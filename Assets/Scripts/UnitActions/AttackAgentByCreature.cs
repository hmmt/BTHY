using UnityEngine;
using System.Collections;

public class AttackAgentByCreature : ActionClassBase {
    public AgentModel targetAgent;


    public CreatureModel creature;

    private float timer = 0.0f;

    private void proccess()
    {
        int agentDice = Random.Range(1, 4);
        int creatureDice = Random.Range(1, 5);

        if (creatureDice > agentDice)
        {
            if (targetAgent.hp > 0)
            {
                targetAgent.TakePhysicalDamage(creature.metaInfo.physicsDmg);
                targetAgent.TakeMentalDamage(creature.metaInfo.mentalDmg);
            }

            if (targetAgent.isDead())
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
        targetAgent.FinishWorking();
        creature.lookAtTarget = null;

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

    public static void Create(AgentModel targetAgent, CreatureModel creature)
    {
        Debug.Log("AttackAgentByCreature");
        GameObject a = new GameObject();

        AttackAgentByCreature c = a.AddComponent<AttackAgentByCreature>();

        c.targetAgent = targetAgent;
        c.creature = creature;

        creature.lookAtTarget = targetAgent.GetMovableNode();

        targetAgent.AttackedByCreature();
        creature.state = CreatureState.ESCAPE_ATTACK;
    }
}
