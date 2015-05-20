using UnityEngine;
using System.Collections;

public class MagicalGirl : CreatureBase {

    // 기분상태에 따라 마취약 투여가 가능해야 함.

    private bool isDark = false;

    // 역변
    //public override void OnFeelingUpdate(CreatureUnit creature)
    public override void  OnFixedUpdate(CreatureUnit creature)
    {
        if (creature.feeling <= 50)
        {
            if(isDark == false)
                Debug.Log("MagicalGirl.. darkness");
            isDark = true;
        }
        else
        {
            if(isDark == true)
                Debug.Log("MagicalGirl.. status ok");
            isDark = false;
        }
    }

    private void SkillDarkAttack(UseSkill skill)
    {
        Debug.Log("MagicalGirl dark attack");
        skill.agent.TakePhysicalDamage(4);
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        if (isDark)
        {
            SkillDarkAttack(skill);
        }

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/magicalGirl/magicalGirl_enterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 6);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.room, "typo/magicalGirl/magicalGirl_enterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/magicalGirl/magicalGirl_enterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/magicalGirl/magicalGirl_enterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/magicalGirl/magicalGirl_enterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
