using UnityEngine;
using System.Collections;

public class LadyLookingAtWall : CreatureBase {

    private const int skillPhysicsDmg = 5;
    private const int skillMentalDmg = 50;

    public override void SkillTickUpdate(UseSkill skill)
    {
        if (Random.value <= 0.3f)
        {
            ActivateSkill(skill);
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        // 스킬: 뒤를 돌아보지 마
        skill.agent.TakePhysicalDamage(skillPhysicsDmg);
        skill.agent.TakeMentalDamage(skillMentalDmg);
    }

    public override void EnterRoom(UseSkill skill)
    {
        //skill.PauseWorking();
        //SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);
    }
}
