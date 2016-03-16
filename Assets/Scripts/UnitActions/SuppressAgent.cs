using UnityEngine;
using System.Collections;

// unused

public class SuppressAgent : ActionClassBase {

    public AgentModel actorAgent;
    public AgentModel targetAgent;

    private int actorStamina;
    private int targetStamina;

    private float timer = 0.0f;

    int noticeId1;
    int noticeId2;

    /// <summary>
    /// 다음과 같이 사용 : dmg = staminaDmgList[level-1];
    /// </summary>
    private static int[] staminaDmgList = new int[] { 3, 5, 7, 9, 11 };
    private static int[] staminaList = new int[] { 10, 15, 20, 25, 30 };

    private static int[] hpDmgList = new int[] { 2, 3, 3, 4, 5 };
    private static float[] avoidabilityList = new float[] {0.1f, 0.12f, 0.15f, 0.19f, 0.24f};
    

    private int GetStaminaDmg(AgentModel agent)
    {
        return staminaDmgList[agent.level-1];
    }

    private int GetStamina(AgentModel agent)
    {
        return staminaList[agent.level-1];
    }

    private int GetHpDmg(AgentModel agent)
    {
        return hpDmgList[agent.level - 1];
    }

    private float GetAvoidability(AgentModel agent)
    {
        return avoidabilityList[agent.level - 1];
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.MakeName(NoticeName.ChangeAgentState, targetAgent.instanceId.ToString()), noticeId1);
        Notice.instance.Remove(NoticeName.MakeName(NoticeName.ChangeAgentState, actorAgent.instanceId.ToString()), noticeId2);
    }


    // TODO : 수식 변경해야 함
    private void proccess()
    {
        if (Random.value < 0.5f)
        {
            // 바꿔야 함
            targetStamina--;

            if (targetAgent.isDead() || targetStamina <= 0)
            {
                targetAgent.UpdateStateIdle();
                targetAgent.StopPanic();
                Finish();
            }
            Debug.Log("target stamina : "+targetStamina);
        }
        else
        {
            // 바꿔야 함
            actorStamina--;

            if (actorAgent.isDead() || actorStamina <= 0)
            {
                Finish();
            }
            Debug.Log("actor stamina : " + actorStamina);
        }
    }

    void Finish()
    {
        // TODO : 고쳐야 함
        //actorAgent.StopSuppress();
        targetAgent.UpdateStateIdle();

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (actorAgent.GetMovableNode().CheckInRange(targetAgent.GetMovableNode()))
        {
            if (targetAgent.GetState() != AgentAIState.PANIC_SUPPRESS_TARGET)
            {
                actorAgent.GetMovableNode().StopMoving();
                targetAgent.PanicSuppressed();
            }
            timer += Time.deltaTime;

            if (timer >= 1)
            {
                timer -= 1;
                proccess();
            }
        }
    }

    public void OnChangeTargetAgentState()
    {
        if (targetAgent.GetState() != AgentAIState.PANIC_SUPPRESS_TARGET)
        {
            if (actorAgent.GetState() == AgentAIState.SUPPRESS_WORKER)
            {
                //actorAgent.StopSuppress();
            }

            Destroy(gameObject);
        }
    }

    public void OnChangeActorAgentState()
    {
        if (actorAgent.GetState() != AgentAIState.SUPPRESS_WORKER)
        {
            if (targetAgent.GetState() == AgentAIState.PANIC_SUPPRESS_TARGET)
            {
                targetAgent.UpdateStateIdle();
            }

            Destroy(gameObject);
        }
    }

    public static void Create(AgentModel targetAgent, AgentModel actorAgent)
    {
        //Debug.Log(
        GameObject a = new GameObject();

        SuppressAgent c = a.AddComponent<SuppressAgent>();

        c.targetAgent = targetAgent;
        c.actorAgent = actorAgent;

        c.targetStamina = 10;
        c.actorStamina = 10;

        actorAgent.StartSuppressAgent(targetAgent, null);
        //targetAgent.PanicSuppressed();

        c.noticeId1 = Notice.instance.Observe(NoticeName.MakeName(NoticeName.ChangeAgentState, c.targetAgent.instanceId.ToString()),
            delegate(object[] param) { c.OnChangeTargetAgentState(); });
        c.noticeId2 = Notice.instance.Observe(NoticeName.MakeName(NoticeName.ChangeAgentState, c.actorAgent.instanceId.ToString()),
            delegate(object[] param) { c.OnChangeActorAgentState(); });
    }
}
