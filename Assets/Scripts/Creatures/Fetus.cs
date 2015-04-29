using UnityEngine;
using System.Collections;

public class Fetus : CreatureBase
{
    private float feedTimer = 0;
    private float feedLimit = 180.0f;

    private float cryTimer = 0;
    private float cryLimit = 120.0f;

    public override void FixedUpdate(CreatureUnit creature)
    {
        feedTimer += Time.deltaTime;

        if(feedTimer > feedLimit)
        {
            feedTimer = 0;
            creature.SubFeeling(20);
        }

        if(creature.feeling <= 40)
        {
            cryTimer += Time.deltaTime;
        }
        else
        {
            cryTimer = 0;
        }

        if(cryTimer > cryLimit)
        {
            cryTimer = 0;

            // 전체공격!!!!
            foreach (AgentUnit agent in AgentFacade.instance.GetAgentList())
            {
                agent.mental -= 20;
                Notice.instance.Send("UpdateAgentState_" + agent.gameObject.GetInstanceID());
            }
        }
    }

    // temporary
    public override void SkillFailWorkTick(UseSkill skill)
    {
        ActivateSkill(skill);
    }

    public void ActivateSkill(UseSkill skill)
    {
        // show effect

        skill.PauseWorking();
        ///SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability.wav", skill.targetCreature.transform.position);

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 5.5f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 5.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 4.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.0f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.5f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);


    }

    public override void SkillTickUpdate(UseSkill skill)
    {
        //if()
        {
            feedTimer = 0;
        }
    }

    //

    public override void EnterRoom(UseSkill skill)
    {
        //if()
        {
            feedTimer = 0;
        }
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 6.0f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        /*
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 5.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);*/
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 5.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 5.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 4.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.0f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.5f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.0f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/fetus/nameLessFetus_EnterTypo_09", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.5f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
