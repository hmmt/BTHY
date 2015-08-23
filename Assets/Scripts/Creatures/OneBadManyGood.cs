using UnityEngine;
using System.Collections;

public class OneBadManyGood : CreatureBase {

    private int skillDamage = 20;

    // temporary
    public override void OnSkillFailWorkTick(UseSkill skill)
    {
        if (skill.skillTypeInfo.id == 40001)
        {
            ActivateSkill(skill);
        }
    }

    public override void OnSkillSucceedWorkTick(UseSkill skill)
    {
        skill.targetCreature.AddFeeling(100);
    }

    public void ActivateSkill(UseSkill skill)
    {
        Debug.Log("OneBadManyGood ActivateSkill()");
        // show effect

        skill.PauseWorking();

        ///SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability.wav", skill.targetCreature.transform.position);

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 6.0f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        SoundEffectPlayer.PlayOnce("creature/onebad_good/oneBad_special", skill.targetCreatureView.transform.position);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 5.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 5.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 4.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.0f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.5f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_AttackTypo_09", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.0f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);

        skill.targetCreature.ShowNarrationText("special_ability1", skill.agent.name);

            TimerCallback.Create(3.0f, delegate() {
            skill.targetCreature.ShowNarrationText("special_ability2", skill.agent.name);
            skill.agent.mental -= skillDamage;

                // 죄책감 특성 사라져야 함.
        });

            TimerCallback.Create(6.0f, delegate() {
            skill.targetCreature.ShowNarrationText("special_ability3", skill.agent.name);
            skill.agent.mental -= skillDamage;
        });
    }

    //

    public override void OnEnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });


        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
