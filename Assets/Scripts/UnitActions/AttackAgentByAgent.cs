using UnityEngine;
using System.Collections;

public class AttackAgentByAgent : ActionClassBase {
    public AgentModel targetAgent;


    public AgentModel panicAgent;

    private float timer = 0.0f;


    // TODO : 수식 변경해야 함
    private void proccess()
    {
        int targetDice = Random.Range(1, 4);
        int executorDice = Random.Range(1, 5);

        if (executorDice > targetDice)
        {
            if (targetAgent.hp > 0)
            {
                targetAgent.TakePhysicalDamage(1);
            }

            if (targetAgent.isDead())
            {
                Finish();
            }
            Debug.Log("attack target");
        }
        else if (executorDice < targetDice)
        {
            if (panicAgent.hp > 0)
            {
                panicAgent.TakePhysicalDamage(1);
            }

            if (panicAgent.isDead())
            {
                Finish();
            }
            Debug.Log("attack panic");
        }
    }

    void Finish()
    {
        panicAgent.StopPanicAttackAgent();
        targetAgent.FinishWorking();

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

    public void OnChangeTargetAgentState()
    {
        if (targetAgent.GetState() != AgentCmdState.CAPTURE_BY_AGENT)
        {
            if (panicAgent.GetState() == AgentCmdState.PANIC_VIOLENCE)
            {
                panicAgent.StopPanicAttackAgent();
            }

            Destroy(gameObject);
        }
    }

    public void OnChangePanicAgentState()
    {
        if (panicAgent.GetState() != AgentCmdState.PANIC_VIOLENCE)
        {
            if (targetAgent.GetState() == AgentCmdState.CAPTURE_BY_AGENT)
            {
                targetAgent.UpdateStateIdle();
            }

            Destroy(gameObject);
        }
    }

    public static void Create(AgentModel targetAgent, AgentModel panicAgent)
    {
        //Debug.Log(
        GameObject a = new GameObject();

        AttackAgentByAgent c = a.AddComponent<AttackAgentByAgent>();

        c.targetAgent = targetAgent;
        c.panicAgent = panicAgent;

        targetAgent.AttackedByCreature();
        panicAgent.StartPanicAttackAgent();

        Notice.instance.Observe(NoticeName.MakeName(NoticeName.ChangeAgentState, c.targetAgent.instanceId.ToString()),
            delegate(object[] param){c.OnChangeTargetAgentState();});
        Notice.instance.Observe(NoticeName.MakeName(NoticeName.ChangeAgentState, c.panicAgent.instanceId.ToString()),
            delegate(object[] param) { c.OnChangePanicAgentState(); });
    }
}
