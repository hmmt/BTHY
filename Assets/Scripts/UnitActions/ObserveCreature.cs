using UnityEngine;
using System.Collections;

public class ObserveCreature : MonoBehaviour
{

    public AgentModel agent;
    public CreatureModel creature;

    public int agentStack;
    public int creatureStack;

    public int observeCompleteLevel;

    private int current = 0;
    private int maximum = 0;
    private float timer = 0.0f;


    public static void Create(AgentModel agent, CreatureModel creature)
    {

        GameObject a = new GameObject();

        ObserveCreature c = a.AddComponent<ObserveCreature>();

        c.agent = agent;
        c.creature = creature;

        c.agentStack = c.agent.level;
        c.creatureStack = c.creature.metaInfo.stackLevel;

       
        agent.Working(creature);
        creature.state = CreatureState.OBSERVE;
        Debug.Log("OBSERVE");

        Notice.instance.Send("UpdateCreatureState_" + creature.instanceId);
    }

    private void observeAction()
    {
        int agentDice = Random.Range(1, 6);
        int creatureDice = Random.Range(1, 7);
        //int agentDice = 2;
       // int creatureDice = 1;

        Debug.Log("관찰시작");
        Debug.Log("Creature Stack : "+creatureStack );
        Debug.Log("Agent Stack : " + agentStack);

        //직원 스택이 0이되면 관찰 실패 || 환상체 스택이 0이되면 관찰 성공
        if (agentStack <= 0)
        {
            Debug.Log("관찰실패");
            agent.TakePhysicalDamage(creature.metaInfo.physicsDmg);
            agent.TakeMentalDamage(creature.metaInfo.mentalDmg);
            agent.FinishWorking();
            creature.state = CreatureState.WAIT;
            Notice.instance.Send("UpdateCreatureState_" + creature.instanceId);
            Destroy(gameObject);
            return;
        }

        if (creatureStack <= 0)
        {
            Debug.Log("관찰성공");
            creature.observeProgress++;
            agent.FinishWorking();
            creature.state = CreatureState.WAIT;
            Notice.instance.Send("UpdateCreatureState_" + creature.instanceId);
            Destroy(gameObject);
            return;
        }

        //각 직원 환상체 등급을 사용하여 주사위굴림
        if (agentDice > creatureDice)
        {
            creatureStack--;
        }

        else if (agentDice < creatureDice)
        {
            agentStack--;
        }
    }

    public void CheckLive()
    {
        if (agent.mental <= 0)
        {
            string speech;
            if (agent.speechTable.TryGetValue("panic", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
            }

            creature.ShowNarrationText("panic", agent.name);

            // FinshWork();
            agent.Panic();
            string narration = this.name + " (이)가 공황에 빠져  관찰작업에 실패하였습니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
        if (agent.hp <= 0)
        {
            string speech;
            if (agent.speechTable.TryGetValue("dead", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
            }

            creature.ShowNarrationText("dead", agent.name);
            string narration = this.name + " (이)가 사망하여 안타깝게도 관찰 작업에 실패하였습니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        timer += Time.deltaTime;

        if (timer >= 6.0f)
        {
            timer -= 6.0f;
            if (agent.GetCurrentNode() != null && agent.GetCurrentNode().GetId() == creature.GetWorkspaceNode().GetId())
            {
                observeAction();
            }
        }
    }
}

