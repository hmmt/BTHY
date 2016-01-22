using UnityEngine;
using System.Collections;

public class OldLady : CreatureBase {

    private int skillDamage = 5;

    public override void OnSkillGoalComplete(UseSkill skill)
    {
        //ActivatSkill(skill);
    }

    public override void OnSkillTickUpdate(UseSkill skill)
    {
        float prob = 0.8f;

        if (Random.Range(0, 100) < prob * 100)
        {
            ActivateSkill(skill);
        }
    }

    public void ActivateSkill(UseSkill skill)
    {
        Debug.Log("OldLady ActivateSkill()");
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 7.5f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        SoundEffectPlayer.PlayOnce("creature/old_lady/oldlady_special", skill.targetCreatureView.transform.position);

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 7.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 6.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 6.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 5.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.0f, 5.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.5f, 4.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.0f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_09", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.5f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_10", CreatureOutsideTextLayout.CENTER_BOTTOM, 5.0f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_11", CreatureOutsideTextLayout.CENTER_BOTTOM, 5.5f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_12", CreatureOutsideTextLayout.CENTER_BOTTOM, 6.0f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);

        for(int i=1; i<=5; i++)
        {
            int copiedI = i;
            TimerCallback.Create(i*1.2f, delegate() {
                skill.targetCreature.ShowNarrationText("special_ability"+copiedI, skill.agent.name);
            });
        }
        TimerCallback.Create(7.2f, delegate() {
            model.ShowNarrationText("special_ability6", skill.agent.name);
            //skill.agent.TakeMentalDamage(skillDamage);
            CreatureFeelingState feelingState = model.GetFeelingState();
            if (feelingState == CreatureFeelingState.BAD)
            {
                skill.agent.TakeMentalDamage(30);
            }
            else if (feelingState == CreatureFeelingState.NORM)
            {
                skill.agent.TakeMentalDamage(20);
            }
            else if (feelingState == CreatureFeelingState.GOOD)
            {
                skill.agent.TakeMentalDamage(10);
            }
        });
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 9);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });


        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 8)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 7)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 6)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 5, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 6, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 7, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
