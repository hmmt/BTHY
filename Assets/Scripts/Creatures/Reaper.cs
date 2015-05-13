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
        //skill.PauseWorking();
        //SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);
    }
}
