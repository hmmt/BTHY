using UnityEngine;
using System.Collections;

public class SmilePierrot : CreatureBase {

    private const int skillPhysicalDmg = 4;

    public override void OnSkillStart(UseSkill skill)
    {
        // 직원 특성에 따라 칼빵!
        //skill.agent.

        // 헤픈 웃음, 쾌활함
        if (skill.agent.HasTrait(10017) == false &&
            skill.agent.HasTrait(10018) == false)
        {
            if (Random.value <= 0.7f)
            {
                ActivateSkill(skill);
            }
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        Debug.Log("SmilePierrot ActivateSkill");
		skill.agent.TakePhysicalDamage(skillPhysicalDmg, DamageType.CUSTOM);
    }

    public override void OnEnterRoom(UseSkill skill)
    {
		/*
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/smilePierrot/smilePierrot_enterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 7);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/smilePierrot/smilePierrot_enterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 6)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/smilePierrot/smilePierrot_enterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/smilePierrot/smilePierrot_enterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/smilePierrot/smilePierrot_enterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/smilePierrot/smilePierrot_enterTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        */
    }
}
