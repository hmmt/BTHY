using UnityEngine;
using System.Collections;

public class Reaper : CreatureBase {

    public override void OnSkillTickUpdate(UseSkill skill)
    {
        // 혼자 투입 시

        if (true)
        {
            ActivateSkill(skill);
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        Debug.Log("Reaper ActivateSkill");
        // 스킬 : <I am MISSIN’ U>

        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_attackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 6);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_attackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 5)
    .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_attackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_attackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        skill.targetCreature.ShowNarrationText("special_ability1", skill.agent.name);
        TimerCallback.Create(3.0f, delegate() { skill.targetCreature.ShowNarrationText("special_ability2", skill.agent.name); });
        TimerCallback.Create(3.0f, delegate() {skill.targetCreature.ShowNarrationText("special_ability3", skill.agent.name);  });
        TimerCallback.Create(3.0f, 
            delegate()
        {
            skill.targetCreature.ShowNarrationText("special_ability4", skill.agent.name);
                int damage = skill.agent.hp / 2;

                if (damage <= 0)
                    damage = 1;

                 // 단호박 특성
                if (skill.agent.HasTrait(10016) == true)
                    return;
                skill.agent.TakePhysicalDamage(damage);
                skill.agent.TakeMentalDamage(10);
            //skill.CheckLive();
        });
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 8);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });


        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 7)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 6)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 5, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/reaper/reaper_enterTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 6, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
