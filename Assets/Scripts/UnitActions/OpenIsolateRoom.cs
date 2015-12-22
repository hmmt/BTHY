using UnityEngine;
using System.Collections;

public class OpenIsolateRoom : ActionClassBase {

    private AgentModel actorAgent;
    private CreatureModel targetCreature;

    private float timer = 0;

    private float cooldown = 0.2f;


    void proccess()
    {
        targetCreature.SubFeeling(1);
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            timer -= cooldown;
            proccess();
        }
    }


    public static void Create(AgentModel agent, CreatureModel creature)
    {
        new GameObject();

        GameObject a = new GameObject();

        OpenIsolateRoom c = a.AddComponent<OpenIsolateRoom>();

        agent.OpenIsolateRoom();

        c.actorAgent = agent;
        c.targetCreature = creature;
    }
}
