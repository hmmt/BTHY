using UnityEngine;
using System.Collections;

public class Reaper : CreatureBase {

    public override void SkillTickUpdate(UseSkill skill)
    {
        // 혼자 투입 시

        if (true)
        {
            ActivateSkill(skill);
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        // 스킬 : <I am MISSIN’ U>
        int damage = skill.agent.hp / 2;

        if (damage <= 0)
            damage = 1;

        skill.agent.TakePhysicalDamage(damage);
    }

    public override void EnterRoom(UseSkill skill)
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
