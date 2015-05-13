using UnityEngine;
using System.Collections;

public class SmilePierrot : CreatureBase {

    private const int skillPhysicalDmg = 4;

    public override void SkillTickUpdate(UseSkill skill)
    {
        // 직원 특성에 따라 칼빵!
        //skill.agent.
        if(true)
        {
            if (Random.value <= 0.7f)
            {
                ActivateSkill(skill);
            }
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        skill.agent.TakePhysicalDamage(skillPhysicalDmg);
    }

    public override void EnterRoom(UseSkill skill)
    {
        //skill.PauseWorking();
        //SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);
    }
}
